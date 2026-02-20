using CCEnvs.Attributes.Serialization;
using CCEnvs.Reflection;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

#nullable enable
namespace CCEnvs.Snapshots
{
    [Serializable]
    [TypeSerializationDescriptor("Snapshots.ValueSnapshot<>", "aaad0351-d678-4058-b208-d3d8fbdf4a3b")]
    public readonly struct ValueSnapshot<T> :  ISnapshot<T>, IEquatable<ValueSnapshot<T>>
    {
        [JsonIgnore]
        public readonly Type TargetType => TypeofCache<T>.Type;

        [JsonProperty("value")]
        public T Value { get; }

        [JsonConstructor]
        public ValueSnapshot(T value)
        {
            Value = value;
        }

        public static bool operator ==(ValueSnapshot<T> left, ValueSnapshot<T> right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(ValueSnapshot<T> left, ValueSnapshot<T> right)
        {
            return !(left == right);
        }

        public ValueSnapshot<T> WithValue(T value)
        {
            return new ValueSnapshot<T>(value);
        }

        public readonly bool CanRestore(T? target)
        {
            return TargetType.IsValueType || target.IsNotNull();
        }

        public readonly bool TryRestore(T? target, [NotNullWhen(true)] out T? restored)
        {
            if (!CanRestore(target))
            {
                target = Value;

                if (!CanRestore(target))
                {
                    restored = default;
                    return false;
                }
            }

            restored = target!;
            return true;
        }

        public readonly override bool Equals(object? obj)
        {
            return obj is ValueSnapshot<T> snapshot && Equals(snapshot);
        }

        public readonly bool Equals(ValueSnapshot<T> other)
        {
            return EqualityComparer<Type>.Default.Equals(TargetType, other.TargetType)
                   &&
                   EqualityComparer<T>.Default.Equals(Value, other.Value);
        }

        public readonly override int GetHashCode()
        {
            return HashCode.Combine(TargetType, Value);
        }

        public readonly override string ToString()
        {
            if (Equals(default))
                return StringHelper.EMPTY_OBJECT;

            return $"({nameof(Value)}: {Value}; {nameof(TargetType)}: {TargetType})";
        }
    }
}
