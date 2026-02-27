using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using CCEnvs.Attributes.Serialization;
using CCEnvs.Reflection;
using Newtonsoft.Json;

#nullable enable
namespace CCEnvs.Snapshots
{
    [Serializable]
    [TypeSerializationDescriptor("Snapshots.ValueSnapshot", "1a8143b1-606a-465b-9e88-654179587eb5")]
    public readonly struct ValueSnapshot : ISnapshot, IEquatable<ValueSnapshot>
    {
        [JsonIgnore]
        public readonly Type TargetType { get; }

        [JsonProperty("value")]
        public object Value { get; }

        [JsonConstructor]
        public ValueSnapshot(object value)
        {
            CC.Guard.IsNotNull(value, nameof(value));

            TargetType = value.GetType();
            Value = value;
        }

        public static bool operator ==(ValueSnapshot left, ValueSnapshot right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(ValueSnapshot left, ValueSnapshot right)
        {
            return !(left == right);
        }

        public ValueSnapshot<T> Cast<T>()
        {
            return new ValueSnapshot<T>((T)Value);
        }

        public ValueSnapshot WithValue(object value)
        {
            return new ValueSnapshot(value);
        }

        public readonly bool CanRestore(object? target)
        {
            return TargetType.IsValueType || target.IsNotNull();
        }

        public readonly bool TryRestore(object? target, [NotNullWhen(true)] out object? restored)
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
            return obj is ValueSnapshot snapshot && Equals(snapshot);
        }

        public readonly bool Equals(ValueSnapshot other)
        {
            return EqualityComparer<Type>.Default.Equals(TargetType, other.TargetType)
                   &&
                   EqualityComparer<object?>.Default.Equals(Value, other.Value);
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

    [Serializable]
    [TypeSerializationDescriptor("Snapshots.ValueSnapshot<>", "aaad0351-d678-4058-b208-d3d8fbdf4a3b")]
    public readonly struct ValueSnapshot<T> : ISnapshot<T>, IEquatable<ValueSnapshot<T>>
    {
        [JsonIgnore]
        public readonly Type TargetType => TypeofCache<T>.Type;

        [JsonProperty("value")]
        public T Value { get; }

        [JsonConstructor]
        public ValueSnapshot(T value)
        {
            CC.Guard.IsNotNull(value, nameof(value));

            Value = value;
        }

        public static implicit operator ValueSnapshot(ValueSnapshot<T> instance)
        {
            return new ValueSnapshot(instance.Value!);
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
