#nullable enable
using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Threading.Tasks;

namespace UTIRLib.Utils
{
    public static class TypeSearch
    {
        public static bool TryFindTypeInAppDomain(string typeNamePart,
                                                  [NotNullWhen(true)] out Type? result,
                                                  bool ignoreCase = false,
                                                  bool byFullName = false)
        {
            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();

            int assembliesCount = assemblies.Length;
            for (int i = 0; i < assembliesCount; i++)
                if (TryFindType(assemblies[i], typeNamePart, out result, ignoreCase, byFullName))
                    return true;

            result = null;
            return false;
        }

        /// <exception cref="TypeNotFoundException"></exception>
        public static Type FindTypeInAppDomain(string typeNamePart,
                                               bool ignoreCase = false,
                                               bool byFullName = false)
        {
            if (!TryFindTypeInAppDomain(typeNamePart, out Type? result, ignoreCase, byFullName))
            {
                throw new TypeNotFoundException(typeNamePart);
            }

            return result;
        }

        public static bool TryFindType(Assembly assembly,
                                       string typeNamePart,
                                       [NotNullWhen(true)] out Type? result,
                                       bool ignoreCase = false,
                                       bool byFullName = false)
        {
            Type[] types = assembly.GetTypes();

            StringComparison stringComparison = ignoreCase ? StringComparison.InvariantCultureIgnoreCase
                                                           : StringComparison.InvariantCulture;

            if (types.Length > 299)
            {
                result = ParallelSearch(types, typeNamePart, stringComparison, byFullName);

                return result != null;
            }
            else
            {
                string typeName;
                int typesCount = types.Length;
                for (int i = 0; i < typesCount; i++)
                {

                    typeName = byFullName ? types[i].FullName : types[i].Name;
                    if (typeName.Contains(typeNamePart, stringComparison))
                    {
                        result = types[i];
                        return true;
                    }
                }
            }

            result = null;
            return false;
        }

        /// <exception cref="TypeNotFoundException"></exception>
        public static Type FindType(Assembly assembly,
                                    string typeNamePart,
                                    bool ignoreCase = false,
                                    bool byFullName = false)
        {
            if (!TryFindType(assembly, typeNamePart, out Type? result, ignoreCase, byFullName))
            {
                throw new TypeNotFoundException(typeNamePart);
            }

            return result;
        }

        private static Type? ParallelSearch(Type[] types,
                                            string typeNamePart,
                                            StringComparison stringComparison,
                                            bool byFullName)
        {
            bool isFound = false;
            object lockObject = new();
            Type? result = null;

            int typesCount = types.Length;
            Parallel.For(0, typesCount, (i, state) =>
            {
                string typeName = byFullName ? types[i].FullName : types[i].Name;
                if (typeName.Contains(typeNamePart, stringComparison))
                {
                    lock (lockObject)
                    {
                        if (!isFound)
                        {
                            isFound = true;
                            result = types[i];
                            state.Stop();
                        }
                    }
                }
            });

            return result;
        }
    }
}
