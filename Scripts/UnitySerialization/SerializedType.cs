using System;
using System.Linq;
using UnityEngine;
using UTIRLib.Diagnostics;
using UTIRLib.Utils;

#nullable enable
#pragma warning disable S1117
namespace UTIRLib.Unity.Serialization
{
    [Serializable]
    public struct SerializedType : IUnitySerialized<Type>
    {
        private Type? value;

        [SerializeField]
        private string namespacePart;

        [SerializeField]
        private string typeName;

        [SerializeField]
        private bool isNamespacedFullName;

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

        public SerializedType(string typeName, string? namespacePart = null) : this()
        {
            this.namespacePart = namespacePart ?? string.Empty;
            this.typeName = typeName;

            FindTypeByName();
        }

        public readonly override string ToString() => typeName ?? string.Empty;

        private void FindTypeByName()
        {
            if (typeName.IsNullOrEmpty())
                throw new StringException(typeName);

            value = TypeSearch.FindTypeInAppDomain(new TypeSearchingParameters
            {
                NamepsacePart = namespacePart,
                TypeName = typeName,
                SearchByFullName = isNamespacedFullName
            });
        }

        public static implicit operator Type(SerializedType value)
        {
            return value.Value;
        }
    }
}
