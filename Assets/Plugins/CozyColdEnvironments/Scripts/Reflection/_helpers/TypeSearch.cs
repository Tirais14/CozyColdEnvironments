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
        public static Type[] FindTypesInAppDomain(TypeSearchArguments parameters)
        {
            CC.Guard.NullArgument(parameters, nameof(parameters));

            return FindTypesInternal(parameters);
        }

        /// <exception cref="CannotResolvedException"></exception>
        /// <exception cref="TypeNotFoundException"></exception>
        public static Type FindTypeInAppDomain(TypeSearchArguments parameters,
                                               bool throwOnError = true)
        {
            CC.Guard.NullArgument(parameters, nameof(parameters));
            CC.Guard.StringArgument(parameters.TypeName,
                Syntax.Chain(nameof(parameters), nameof(parameters.TypeName)));

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
                          .EqualsOrdinal(parameters.TypeName)).ToArray();
            }

            void FilterFoundsStrict()
            {
                if (foundTypes.Count() > 1)
                    foundTypes = foundTypes.Where(
                        x => x.GetName(TypeNameConvertingAttributes.IncludeGenericArguments)
                              .EqualsOrdinal(parameters.TypeName)).ToArray();
            }

            void TryThrowNotFound()
            {
                if (throwOnError)
                    throw new TypeNotFoundException(parameters);
            }
        }

        public static bool TryFindTypeInAppDomain(TypeSearchArguments parameters,
                                                  out Type? result)
        {
            result = FindTypeInAppDomain(parameters, throwOnError: false);

            return result != null;
        }

        private static Type[] FindTypesInternal(TypeSearchArguments parameters)
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

            bool hasNamespaceFilter = parameters.HasNamespaceName;
            bool hasTypeNameFilter = parameters.HasTypeName;

            IEnumerable<Type> filteredTypes = allTypes.SelectMany(x => x);

            if (hasNamespaceFilter)
                filteredTypes = filteredTypes.Where(
                    type => type.Namespace is not null
                    && 
                    type.Namespace.ContainsOrdinal(parameters.NamespaceName, parameters.IgnoreCase));

            if (hasTypeNameFilter)
                filteredTypes = filteredTypes.Where(
                    type => type.GetName().ContainsOrdinal(parameters.TypeName, parameters.IgnoreCase));

            return filteredTypes.ToArray();
        }
    }
}
