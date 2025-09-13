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
            CC.Validate.ArgumentNull(parameters, nameof(parameters));

            return FindTypesInternal(parameters).ToArray();
        }

        /// <exception cref="CannotResolvedException"></exception>
        /// <exception cref="TypeNotFoundException"></exception>
        public static Type FindTypeInAppDomain(TypeFinderParameters parameters,
                                               bool throwOnError = true)
        {
            CC.Validate.ArgumentNull(parameters, nameof(parameters));
            CC.Validate.StringArgumentNested(parameters.TypeName,
                                          nameof(parameters),
                                          nameof(parameters.TypeName));

            Type[] foundTypes = FindTypesInternal(parameters).ToArray();

            if (foundTypes.IsEmpty())
            {
                TryThrowNotFound();

                return null!;
            }

            if (foundTypes.Length > 1)
            {
                FilterFounds();

                if (foundTypes.Length > 1)
                    FilterFoundsStrict();

                if (foundTypes.Length > 1)
                    throw new AmbiguousMatchException("More than one type matches. Try to specify a more precise name.");

                if (foundTypes.IsEmpty())
                {
                    TryThrowNotFound();

                    return null!;
                }
            }

            return foundTypes[0];

            void FilterFounds()
            {
                foundTypes = foundTypes.Where(
                    x => x.GetName(TypeNameConvertingAttributes.None)
                          .Equals(parameters.TypeName, StringComparison.Ordinal)).ToArray();
            }

            void FilterFoundsStrict()
            {
                if (foundTypes.Count() > 1)
                    foundTypes = foundTypes.Where(
                        x => x.GetName(TypeNameConvertingAttributes.IncludeGenericArguments)
                              .Equals(parameters.TypeName, StringComparison.Ordinal)).ToArray();
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

            Type[] foundTypes = FindTypesInternal(parameters).ToArray();

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
                   where !parameters.HasAssemblyName
                   ||
                   assembly.GetName().Name.ContainsOrdinal(parameters.AssemblyName, parameters.IgnoreCase)
                   select assembly.GetTypes() into types
                   from type in types
                   where type is not null
                   where !parameters.HasNamespace
                   || 
                   (type.Namespace ?? string.Empty).ContainsOrdinal(parameters.Namespace, parameters.IgnoreCase)
                   where !parameters.HasTypeName
                   || 
                   parameters.TypeName.ContainsOrdinal(type.GetName(), parameters.IgnoreCase)
                   select type;
        }
    }
}
