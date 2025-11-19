using System;
using System.Collections.Immutable;

#nullable enable
namespace CCEnvs.Unity.Serialization
{
    [Serializable]
    public sealed class SerializedImmutableArray<T> : Serialized<ImmutableArray<T>>
    {
        [UnityEngine.SerializeField]
        private T[] items = null!;

        public SerializedImmutableArray()
        {
        }

        public SerializedImmutableArray(ImmutableArray<T> defaultValue) 
            :
            base(defaultValue)
        {
        }

        protected override ImmutableArray<T> ValueFactory()
        {
            return items.ToImmutableArray();
        }
    }
}