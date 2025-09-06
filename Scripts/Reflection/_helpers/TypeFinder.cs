#nullable enable
using CCEnvs.Diagnostics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace CCEnvs.Reflection
{
    public static class TypeFinder
    {
        public static Type[] FindTypesInAppDomain(TypeFinderParameters parameters)
        {
            Validate.ArgumentNull(parameters, nameof(parameters));

            return FindTypesInternal(parameters).ToArray();
        }

        /// <exception cref="CannotResolvedException"></exception>
        /// <exception cref="TypeNotFoundException"></exception>
        public static Type FindTypeInAppDomain(TypeFinderParameters parameters,
                                               bool throwOnError = true)
        {
            Validate.ArgumentNull(parameters, nameof(parameters));
            Validate.StringArgumentNested(parameters.TypeName,
                                          nameof(parameters),
                                          nameof(parameters.TypeName));

            IEnumerable<Type> foundTypes = FindTypesInternal(parameters);

            if (foundTypes.IsEmpty())
            {
                TryThrowNotFound();

                return null!;
            }

            if (foundTypes.Count() > 1 && throwOnError)
            {
                FilterFounds();

                if (foundTypes.Count() > 1)
                    FilterFoundsStrict();

                if (foundTypes.Count() > 1)
                    throw new CannotResolvedException("More than one type matches. Try to specify a more precise name.");

                if (foundTypes.IsEmpty())
                {
                    TryThrowNotFound();

                    return null!;
                }
            }

            return foundTypes.First();

            void FilterFounds()
            {
                foundTypes = foundTypes.Where(
                    x => x.GetName()
                          .Equals(parameters.TypeName, StringComparison.Ordinal));
            }

            void FilterFoundsStrict()
            {
                if (foundTypes.Count() > 1)
                    foundTypes = foundTypes.Where(
                        x => x.GetName(TypeNameConvertingAttributes.ShortName)
                              .Equals(parameters.TypeName, StringComparison.Ordinal));
            }

            void TryThrowNotFound()
            {
                if (throwOnError)
                    throw new TypeNotFoundException(parameters);
            }
        }

        public static bool TryFindTypeInAppDomain(TypeFinderParameters parameters,
                                                  out Type? result)
        {
            result = FindTypeInAppDomain(parameters, throwOnError: false);

            return result != null;
        }

        public static Type FindType(Assembly assembly,
                                    TypeFinderParameters parameters,
                                    bool throwOnError)
        {

            Type[] foundTypes = (from module in assembly.GetModules()
                                 select module.FindTypes(TypeFilter, parameters) into types
                                 where types.IsNotEmpty()
                                 from type in types
                                 select type)
                                 .ToArray();

            if (foundTypes.IsEmpty())
            {
                if (throwOnError)
                    throw new TypeNotFoundException(parameters);
                else
                    return null!;
            }

            if (foundTypes.Length > 1 && throwOnError)
                throw new CannotResolvedException(nameof(Type));

            return foundTypes[0];
        }

        private static IEnumerable<Type> FindTypesInternal(TypeFinderParameters parameters)
        {
            return from assembly in AppDomain.CurrentDomain.GetAssemblies()
                   select FindType(assembly, parameters, throwOnError: false) into type
                   where type is not null
                   select type;
        }

        private static bool TypeFilter(Type type, object criteria)
        {
            var searchingParameters = (TypeFinderParameters)criteria;

            return searchingParameters.IsMatch(type);
        }
    }
}
