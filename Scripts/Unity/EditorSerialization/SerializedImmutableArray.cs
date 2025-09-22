using System;
using System.Collections.Immutable;
using UnityEngine;

#nullable enable
namespace CCEnvs.Unity.EditorSerialization
{
    [Serializable]
    public struct SerializedImmutableArray<T> : IConvertibleCC<ImmutableArray<T>>
    {
        [SerializeField]
        private T[] values;

        public static implicit operator ImmutableArray<T>(SerializedImmutableArray<T> source)
        {
            return source.Convert();
        }

        public readonly ImmutableArray<T> Convert()
        {
            return values?.ToImmutableArray()
                   ??
                   Array.Empty<T>().ToImmutableArray();
        }
    }
}
