#nullable enable

using Cysharp.Threading.Tasks;
using System;
using System.Linq;
using System.Reflection;
using UnityEngine;
using CozyColdEnvironments.Diagnostics;

namespace CozyColdEnvironments
{
    public static class AssemblyHelper
    {
        public static Assembly[] FindAssemblies(string partialName, bool throwIfNotFound = true)
        {
            if (partialName.IsNullOrWhiteSpace())
                throw new StringArgumentException(nameof(partialName), partialName);

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
                throw new StringArgumentException(nameof(fullName), fullName);

            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies().Concat(AppDomain.CurrentDomain.ReflectionOnlyGetAssemblies()).Distinct().ToArray();

            Assembly? assembly = assemblies.SingleOrDefault(x => x.GetName().Name == fullName);

            if (throwIfNotFound && assembly is null)
                throw new Exception($"Assembly {fullName} not found.");

            return assembly;
        }
    }
}
