using CCEnvs.Diagnostics;
using CCEnvs.Reflection;
using CCEnvs.Utils;
using System;
using UnityEngine;

#nullable enable
#pragma warning disable S1117
namespace CCEnvs.Unity.EditorSerialization
{
    [Serializable]
    public struct SerializedType : IUnitySerialized<Type?>
    {
        private Type? value;

        [SerializeField]
        private string _namespace;

        [SerializeField]
        private string typeName;

        public Type? Value {
            get
            {
                if (value == null)
                    FindTypeByName();

                return value!;
            }
        }

        public SerializedType(string typeName, string? namespacePart = null) : this()
        {
            this._namespace = namespacePart ?? string.Empty;
            this.typeName = typeName;

            FindTypeByName();
        }

        public readonly override string ToString() => typeName ?? string.Empty;

        private void FindTypeByName()
        {
            if (typeName.IsNullOrEmpty())
                throw new StringException(typeName);

            value = TypeFinder.FindTypeInAppDomain(new TypeFinderParameters
            {
                Namepsace = _namespace,
                TypeName = typeName,
            });
        }

        public static implicit operator Type?(SerializedType value)
        {
            return value.Value;
        }
    }
}
