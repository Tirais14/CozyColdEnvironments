using CCEnvs.FuncLanguage;
using CCEnvs.Reflection;
using System;

#nullable enable
#pragma warning disable S1117

using UnityEngine;
namespace CCEnvs.Unity.Serialization
{
    [Serializable]
    public sealed class SerializedType : Serialized<Type>
    {
        [SerializeField]
        private bool throwIfNotFound = true;

        [field: SerializeField]
        public Maybe<string> Assembly { get; private set; } = default!;

        [field: SerializeField]
        public Maybe<string> Namespace { get; private set; } = default!;

        [field: SerializeField]
        public Maybe<string> TypeName { get; private set; } = default!;

        [field: SerializeField]
        public bool IgnoreCase { get; private set; } = default!;

        public SerializedType()
        {
        }

        public SerializedType(Type defaultValue) : base(defaultValue)
        {
        }

        protected override Type ValueFactory()
        {
            return TypeSearch.FindTypeInAppDomain(new TypeSearchArguments
            {
                Assembly = Assembly.Raw,
                Namespace = Namespace,
                TypeName = TypeName,
                IgnoreCase = IgnoreCase,  
            },
            throwIfNotFound);
        }
    }
}
