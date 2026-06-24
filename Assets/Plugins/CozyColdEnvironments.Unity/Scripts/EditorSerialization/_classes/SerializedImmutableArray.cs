using CCEnvs.Linq;
using System;
using System.Collections.Immutable;
using System.Linq;

#nullable enable
namespace CCEnvs.UnityX.EditorSerialization
{
    [Serializable]
    public sealed class SerializedImmutableArray<T> : EditorSerialized<ImmutableArray<T>>
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

        protected override ImmutableArray<T> CreateValue()
        {
            return items.ToImmutableArray();
        }
    }

    [Serializable]
    public sealed class SerializedImmutableArray<T, TConverted> : EditorSerialized<ImmutableArray<T>, ImmutableArray<TConverted>>
    {
        [UnityEngine.SerializeField]
        private T[] items = null!;

        public SerializedImmutableArray(Func<T, TConverted> converter)
            :
            base((source) => source.Select(converter, static (item, converter) => converter(item)).ToImmutableArray())
        {
        }

        public SerializedImmutableArray(ImmutableArray<TConverted> defaultValue)
            :
            base(defaultValue)
        {
        }

        protected override ImmutableArray<T> CreateValue()
        {
            return items.ToImmutableArray();
        }
    }
}