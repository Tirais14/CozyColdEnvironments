using System;
using UnityEngine;
using UTIRLib.Diagnostics;
using UTIRLib.Utils;

#nullable enable
namespace UTIRLib.Unity.Serialization
{
    [Serializable]
    public struct SerializedType : IUnitySerialized<Type>
    {
        private Type? value;

        [SerializeField]
        private SerializedAssemblies assemblies;

        [SerializeField]
        private string typeName;

        public Type Value {
            get
            {
                if (typeName.IsNullOrEmpty())
                    throw new StringException(typeName);
                if (value == null)
                    FindTypeByName();

                return value!;
            }
        }

        public SerializedType(string typeName) : this()
        {
            this.typeName = typeName;
            FindTypeByName();
        }

        public readonly override string ToString() => typeName ?? string.Empty;

        private void FindTypeByName()
        {
            if (typeName.IsNullOrEmpty())
                throw new StringException(typeName);

            value = TypeSearch.FindTypeInAppDomain(typeName, ignoreCase: false);
        }

        public static implicit operator Type(SerializedType value)
        {
            return value.Value;
        }
    }
}
