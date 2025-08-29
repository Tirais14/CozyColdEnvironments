using System;
using System.Reflection;
using UnityEngine;
using CozyColdEnvironments.Reflection;

#nullable enable
namespace CozyColdEnvironments.Unity.Serialization
{
    [Serializable]
    public struct SerializedAssemblies : IUnitySerialized<Assembly[]>
    {
        private Assembly[]? assemblies;

        [SerializeField]
        private string assemblyName;

        public Assembly[] Value {
            get
            {
                if (assemblies.IsNullOrEmpty())
                    assemblies = AssemblyHelper.FindAssemblies(assemblyName, throwIfNotFound: true);

                return assemblies;
            }
        }

        public SerializedAssemblies(string assemblyName)
        {
            assemblies = null;

            this.assemblyName = assemblyName;
        }

        public static implicit operator Assembly[](SerializedAssemblies serialized)
        {
            return serialized.Value;
        }
    }
}
