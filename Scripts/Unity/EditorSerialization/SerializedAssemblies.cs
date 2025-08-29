using System;
using System.Reflection;
using UnityEngine;

#nullable enable
namespace CCEnvs.Unity.EditorSerialization
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
