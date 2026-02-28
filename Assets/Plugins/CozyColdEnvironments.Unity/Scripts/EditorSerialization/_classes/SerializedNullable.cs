using CCEnvs.Attributes.Serialization;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using UnityEngine;

#nullable enable
namespace CCEnvs.Unity.EditorSerialization
{
    [Serializable]
    [TypeSerializationDescriptor("SerializedNullable<>", "4495c4c5-17b8-4bee-b4c8-8de70cb2554d")]
    public struct SerializedNullable<T> : IEditorSerialized<T?>, IEquatable<SerializedNullable<T>> where T : struct
    {
        [SerializeField]
        [JsonProperty("value")]
        private T value;

        [SerializeField]
        [JsonProperty("hasValue")]
        private bool hasValue;

        [JsonIgnore]
        public readonly T? Deserialized {
            get
            {
                if (!hasValue)
                    return null;

                return value;
            }
        }

        public SerializedNullable(T? defaultValue)
        {
            value = defaultValue.GetValueOrDefault();
            hasValue = defaultValue.HasValue;
        }

        public static implicit operator SerializedNullable<T>(T instance)
        {
            return new SerializedNullable<T>(instance);
        }

        public static implicit operator T?(SerializedNullable<T> instance)
        {
            return instance.Deserialized;
        }

        public static explicit operator T(SerializedNullable<T> instance)
        {
            return instance.value;
        }

        public static bool operator ==(SerializedNullable<T> left, SerializedNullable<T> right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(SerializedNullable<T> left, SerializedNullable<T> right)
        {
            return !(left == right);
        }

        public readonly override bool Equals(object? obj)
        {
            return obj is SerializedNullable<T> nullable && Equals(nullable);
        }

        public readonly bool Equals(SerializedNullable<T> other)
        {
            return EqualityComparer<T>.Default.Equals(value, other.value)
                   &&
                   hasValue == other.hasValue;
        }

        public readonly override int GetHashCode()
        {
            return HashCode.Combine(value, hasValue);
        }

        public readonly override string ToString()
        {
            if (this == default)
                return StringHelper.EMPTY_OBJECT;

            return $"({nameof(value)}: {value}; {nameof(hasValue)}: {hasValue})";
        }
    }
}
