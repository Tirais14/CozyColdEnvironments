using System;
using System.Collections.Generic;
using UnityEngine;

#nullable enable
namespace CCEnvs.Unity.EditorSerialization
{
    [Serializable]
    public struct SerializedKeyValuePair<TKey, TValue>
        :
        IEquatable<SerializedKeyValuePair<TKey, TValue>>,
        IEditorSerialized<KeyValuePair<TKey, TValue>>
    {
        [field: SerializeField]
        public TKey Key { get; private set; }

        [field: SerializeField]
        public TValue Value { get; private set; }

        public readonly KeyValuePair<TKey, TValue> Deserialized => new(Key, Value);

        public SerializedKeyValuePair(TKey key, TValue value)
        {
            Key = key;
            Value = value;
        }

        public static bool operator ==(SerializedKeyValuePair<TKey, TValue> left, SerializedKeyValuePair<TKey, TValue> right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(SerializedKeyValuePair<TKey, TValue> left, SerializedKeyValuePair<TKey, TValue> right)
        {
            return !(left == right);
        }

        public readonly bool Equals(SerializedKeyValuePair<TKey, TValue> other)
        {
            return EqualityComparer<TKey>.Default.Equals(Key, other.Key)
                   &&
                   EqualityComparer<TValue>.Default.Equals(Value, other.Value);
        }

        public readonly override bool Equals(object? obj)
        {
            return obj is SerializedKeyValuePair<TKey, TValue> pair && Equals(pair);
        }

        public readonly override int GetHashCode()
        {
            return HashCode.Combine(Key, Value);
        }

        public readonly override string ToString()
        {
            if (this == default)
                return StringHelper.EMPTY_OBJECT;

            return $"({nameof(Key)}: {nameof(Key)}; {nameof(Value)}: {Value})";
        }
    }
}
