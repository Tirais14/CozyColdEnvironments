using System;
using UnityEngine;

#nullable enable
#pragma warning disable S1144
namespace CCEnvs.Unity.Serialization
{
    [Serializable]
    public struct AssemblyInfo
    {
        [field: SerializeField]
        public string AssemblyName { get; private set; }
    }
}
