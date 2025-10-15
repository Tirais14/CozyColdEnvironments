using System;
using UnityEngine;

#nullable enable
namespace CCEnvs.Unity.EditorSerialization
{
    [Serializable]
    public struct TypeInfo
    {
        [field: SerializeField]
        public string Assembly { get; private set; }

        [field: SerializeField]
        public string Namespace { get; private set; }

        [field: SerializeField]
        public string TypeName { get; private set; }

        [field: SerializeField]
        public bool IgnoreCase { get; private set; }

        public TypeInfo(string typeName,
                        string? assembly = null,
                        string? namespaceName = null,
                        bool ignoreCase = false)
        {
            Assembly = assembly ?? string.Empty;
            Namespace = namespaceName ?? string.Empty;
            TypeName = typeName;
            IgnoreCase = ignoreCase;
        }
    }
}
