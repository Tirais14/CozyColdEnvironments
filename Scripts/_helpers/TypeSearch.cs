#nullable enable
using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using UTIRLib.Diagnostics;
using UTIRLib.Reflection;

namespace UTIRLib.Utils
{
    public static class TypeSearch
    {
        /// <exception cref="TypeNotFoundException"></exception>
        public static Type FindTypeInAppDomain(TypeSearchingParameters parameters,
                                               bool throwIfNotFound = true)
        {
            if (parameters is null)
                throw new ArgumentNullException(nameof(parameters));

            Assembly[] assemblies = AppDomain.CurrentDomain.ReflectionOnlyGetAssemblies();
            int assembliesCount = assemblies.Length;
            for (int i = 0; i < assembliesCount; i++)
            {
                if (TryFindType(assemblies[i], parameters, out Type? type))
                    return type;
            }

            if (throwIfNotFound)
                throw new TypeNotFoundException(parameters);

            return null!;
        }

        public static bool TryFindTypeInAppDomain(TypeSearchingParameters parameters,
                                                  out Type? result)
        {
            result = FindTypeInAppDomain(parameters, throwIfNotFound: false);

            return result != null;
        }

        public static Type FindType(Assembly assembly,
                                    TypeSearchingParameters parameters,
                                    bool throwIfNotFound = true)
        {
            if (assembly is null)
                throw new ArgumentNullException(nameof(assembly));
            if (parameters is null)
                throw new ArgumentNullException(nameof(parameters));

            Type[] types = assembly.GetTypes();

            StringComparison stringComparison = parameters.IgnoreCase 
                ?
                StringComparison.InvariantCultureIgnoreCase
                :
                StringComparison.InvariantCulture;

            Type? result;

            if (parameters.SearchByFullName)
            {
                string fullTypeName = parameters.FullTypeName;
                result = types.SingleOrDefault(x => x.FullName.Equals(fullTypeName,
                                                                      stringComparison));
            }
            else
            {
                string namespacePart = parameters.NamepsacePart;
                string typeNamePart = parameters.TypeName;

                result = types.SingleOrDefault(x =>
                {
                    return x.Namespace.Contains(namespacePart, stringComparison)
                           &&
                           x.GetName().Contains(typeNamePart, stringComparison);
                });
            }

            if (result == null && throwIfNotFound)
                throw new TypeNotFoundException(parameters);

            return result!;
        }

        public static bool TryFindType(Assembly assembly,
                                       TypeSearchingParameters parameters,
                                       [NotNullWhen(true)] out Type? result)
        {
            result = FindType(assembly, parameters, throwIfNotFound: false);

            return result != null;
        }

        //private static Type? ParallelSearch(Type[] types,
        //                                    string typeNamePart,
        //                                    StringComparison stringComparison,
        //                                    bool byFullName)
        //{
        //    bool isFound = false;
        //    object lockObject = new();
        //    Type? result = null;

        //    int typesCount = types.Length;
        //    Parallel.For(0, typesCount, (i, state) =>
        //    {
        //        string typeName = byFullName ? types[i].FullName : types[i].Name;
        //        if (typeName.Contains(typeNamePart, stringComparison))
        //        {
        //            lock (lockObject)
        //            {
        //                if (!isFound)
        //                {
        //                    isFound = true;
        //                    result = types[i];
        //                    state.Stop();
        //                }
        //            }
        //        }
        //    });

        //    return result;
        //}
    }
}
