#if UNITY_2017_1_OR_NEWER
using System;
using System.Collections.Immutable;

#nullable enable
namespace CCEnvs.UnityX.EditorSerialization
{
    [Serializable]
    public sealed class SerializedImmutableArray<T> : EditorSerialized<ImmutableArray<T>>
    {
        [UnityEngine.SerializeField]
        private T[] items = Array.Empty<T>();

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
    public sealed class SerializedImmutableArray<T, TConverted> : EditorSerialized<T[], ImmutableArray<TConverted>>
    {
        [UnityEngine.SerializeField]
        private T[] items = Array.Empty<T>();

        public SerializedImmutableArray(Func<T, TConverted> converter)
            :
            base((source) =>
            {
                ImmutableArray<TConverted> result = Array.ConvertAll(source, (item) => converter(item)).ToImmutableArray();

                if (result.IsDefault)
                    return ImmutableArray<TConverted>.Empty;

                return result;
            })
        {
        }

        public SerializedImmutableArray(ImmutableArray<TConverted> defaultValue)
            :
            base(defaultValue)
        {
        }

        protected override T[] CreateValue() => items;
    }
}
#endif