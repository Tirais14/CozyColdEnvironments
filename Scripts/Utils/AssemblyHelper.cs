#nullable enable

using Cysharp.Threading.Tasks;
using System;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UTIRLib.Diagnostics;

namespace UTIRLib
{
    public static class AssemblyHelper
    {
        public static Assembly[] FindAssemblies(string partialName, bool throwIfNotFound = true)
        {
            if (partialName.IsNullOrWhiteSpace())
                throw new StringArgumentException(nameof(partialName), partialName);

            Assembly[] assemblies = AppDomain.CurrentDomain.ReflectionOnlyGetAssemblies();

            assemblies = assemblies.Where(x => x.FullName.Contains(partialName))
                                   .ToArray();

            if (throwIfNotFound && assemblies.IsEmpty())
                throw new Exception($"Assembly {partialName} not found.");

            return assemblies;
        }

        public static Assembly GetAssembly(string fullName, bool throwIfNotFound = true)
        {
            if (fullName.IsNullOrWhiteSpace())
                throw new StringArgumentException(nameof(fullName), fullName);

            Assembly[] assemblies = AppDomain.CurrentDomain.ReflectionOnlyGetAssemblies();

            Assembly? assembly = assemblies.SingleOrDefault(x => x.FullName == fullName);

            if (throwIfNotFound && assembly is null)
                throw new Exception($"Assembly {fullName} not found.");

            return assembly;
        }
    }
}
