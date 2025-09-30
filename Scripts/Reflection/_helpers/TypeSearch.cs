#nullable enable
using CCEnvs.Diagnostics;
using CCEnvs.Linq;
using SuperLinq;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace CCEnvs.Reflection
{
    public static class TypeSearch
    {
        public static Type[] FindTypesInAppDomain(TypeFinderParameters parameters)
        {
            CC.Validate.ArgumentNull(parameters, nameof(parameters));

            return FindTypesInternal(parameters);
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

            Type[] foundTypes = FindTypesInternal(parameters);

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

        private static Type[] FindTypesInternal(TypeFinderParameters parameters)
        {
            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();

            bool hasAssemblyNameFilter = parameters.HasAssemblyName;
            var allTypes = new ConcurrentBag<IEnumerable<Type>>();
            Parallel.ForEach(assemblies, (assembly) =>
            {
                if (hasAssemblyNameFilter)
                {
                    var assemblyName = assembly.GetName();

                    if (assemblyName.Name.IsNullOrEmpty()
                        ||
                        !assembly.GetName().Name.ContainsOrdinal(parameters.AssemblyName, parameters.IgnoreCase)
                        )
                        return;
                }

                try
                {
                    allTypes.Add(assembly.GetTypes());
                }
                catch (ReflectionTypeLoadException ex)
                {
                    allTypes.Add(ex.Types.Where(type => type is not null));
                    typeof(TypeSearch).PrintException(ex);
                }
            });

            bool hasNamespaceFilter = parameters.HasNamespace;
            bool hasTypeNameFilter = parameters.HasTypeName;

            IEnumerable<Type> filteredTypes = allTypes.SelectMany(x => x);

            if (hasNamespaceFilter)
                filteredTypes = filteredTypes.Where(
                    type => type.Namespace is not null
                    && 
                    type.Namespace.ContainsOrdinal(parameters.Namespace, parameters.IgnoreCase));

            if (hasTypeNameFilter)
                filteredTypes = filteredTypes.Where(
                    type => type.GetName().ContainsOrdinal(parameters.TypeName, parameters.IgnoreCase));

            return filteredTypes.ToArray();
        }
    }
}
