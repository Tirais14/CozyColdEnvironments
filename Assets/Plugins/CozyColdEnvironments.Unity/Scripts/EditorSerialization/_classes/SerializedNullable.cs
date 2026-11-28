using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;

#nullable enable
namespace CCEnvs.UnityX.EditorSerialization
{
    [Serializable, DataContract]
    public struct SerializedNullable<T> :
        IEditorSerialized<T?>,
        IEquatable<SerializedNullable<T>>,
        ISerializationCallbackReceiver
        where T : struct
    {
        [SerializeField]
        [HideInInspector]
        private T @default;

        [SerializeField]
        [DataMember(Name = "value")]
        private T value;

        [SerializeField]
        [DataMember(Name = "hasValue")]
        private bool hasValue;

        public readonly T? Data {
            get
            {
                if (!hasValue)
                    return null;

                return value;
            }
        }

        public SerializedNullable(T? initialValue)
            :
            this()
        {
            value = initialValue.GetValueOrDefault();
            hasValue = initialValue.HasValue;
        }

        public static implicit operator SerializedNullable<T>(T instance)
        {
            return new SerializedNullable<T>(instance);
        }

        public static implicit operator SerializedNullable<T>(T? instance)
        {
            return new SerializedNullable<T>(instance);
        }

        public static implicit operator T?(SerializedNullable<T> instance)
        {
            return instance.Data;
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
            return ToStringBuilder.CreatePooled()
                .AddProperty("Value", value)
                .AddProperty("HasValue", hasValue)
                .ToStringAndDispose();
        }

        void ISerializationCallbackReceiver.OnBeforeSerialize()
        {
            if (!value.Equals(@default))
                hasValue = true;
        }

        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            if (!value.Equals(@default))
                hasValue = true;
        }
    }
}
