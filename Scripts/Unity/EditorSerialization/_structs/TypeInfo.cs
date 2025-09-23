using System;
using UnityEngine;

#nullable enable
namespace CCEnvs.Unity.EditorSerialization
{
    [Serializable]
    public struct TypeInfo
    {
        [field: SerializeField]
        public string Namespace { get; private set; }

        [field: SerializeField]
        public string TypeName { get; private set; }

        public TypeInfo(string @namespace, string typeName)
        {
            Namespace = @namespace;
            TypeName = typeName;
        }
    }
}
