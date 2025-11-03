using CCEnvs.Reflection;
using System;

#nullable enable
#pragma warning disable S1117

using UnityEngine;
namespace CCEnvs.Unity.EditorSerialization
{
    [Serializable]
    public sealed class SerializedType : Serialized<TypeInfo, Type>
    {
        [SerializeField]
        private bool throwIfNotFound = true;

        public SerializedType()
        {
        }

        public SerializedType(bool throwIfNotFound)
        {
            this.throwIfNotFound = throwIfNotFound;
        }

        public SerializedType(Type defaultValue) : base(defaultValue)
        {
        }

        protected override Type ConvertToOutput(TypeInfo input)
        {
            return TypeQuery.FindTypeInAppDomain(new TypeSearchArguments
            {
                AssemblyName = input.Assembly,
                NamespaceName = input.Namespace,
                TypeName = input.TypeName,
                IgnoreCase = input.IgnoreCase,  
            },
            throwIfNotFound);
        }

        protected override TypeInfo ConvertToInput(Type output)
        {
            return new TypeInfo(output.Assembly.GetName().Name,
                                output.GetName(),
                                output.Namespace);
        }
    }
}
