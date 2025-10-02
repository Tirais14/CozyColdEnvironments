#nullable enable

using CCEnvs.Diagnostics;
using System;
using System.Linq;
using System.Reflection;

namespace CCEnvs
{
    public static class AssemblyHelper
    {
        public static Assembly[] FindAssemblies(string partialName, bool throwIfNotFound = true)
        {
            if (partialName.IsNullOrWhiteSpace())
                throw new EmptyStringArgumentException(nameof(partialName), partialName);

            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies().Concat(AppDomain.CurrentDomain.ReflectionOnlyGetAssemblies()).Distinct().ToArray();

            assemblies = assemblies.Where(x => x.GetName().Name.Contains(partialName))
                                   .ToArray();

            if (throwIfNotFound && assemblies.IsEmpty())
                throw new Exception($"Assembly {partialName} not found.");

            return assemblies;
        }

        public static Assembly GetAssembly(string fullName, bool throwIfNotFound = true)
        {
            if (fullName.IsNullOrWhiteSpace())
                throw new EmptyStringArgumentException(nameof(fullName), fullName);

            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies().Concat(AppDomain.CurrentDomain.ReflectionOnlyGetAssemblies()).Distinct().ToArray();

            Assembly? assembly = assemblies.SingleOrDefault(x => x.GetName().Name == fullName);

            if (throwIfNotFound && assembly is null)
                throw new Exception($"Assembly {fullName} not found.");

            return assembly;
        }
    }
}
