#nullable enable
using CCEnvs.Diagnostics;
using System;
using System.Linq;
using System.Reflection;
using CCEnvs.Collections;

namespace CCEnvs.Reflection
{
    public static class TypeFinder
    {
        /// <exception cref="TypeNotFoundException"></exception>
        public static Type FindTypeInAppDomain(TypeFinderParameters parameters,
                                               bool throwOnError = true)
        {
            if (parameters is null)
                throw new ArgumentNullException(nameof(parameters));
            if (parameters.TypeName.IsNullOrEmpty())
                throw new StringArgumentException(nameof(parameters.TypeName), parameters.TypeName);

            Type[] foundTypes = (from x in AppDomain.CurrentDomain.GetAssemblies()
                                 select FindType(x, parameters, throwOnError: false)
                                 into t
                                 where t is not null
                                 select t).ToArray();

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

            Type[] foundTypes = (from x in assembly.GetModules()
                                 select x.FindTypes(TypeFilter, parameters) into types
                                 where types.IsNotEmpty()
                                 from t in types
                                 select t)
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

        private static bool TypeFilter(Type type, object criteria)
        {
            var searchingParameters = (TypeFinderParameters)criteria;

            return searchingParameters.IsMatch(type);
        }
    }
}
