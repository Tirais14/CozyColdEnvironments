using System;
using System.Collections.Generic;

#nullable enable
namespace CCEnvs.UnityX.ECS
{
    public struct NullableUnmanaged<T> : IEquatable<NullableUnmanaged<T>> where T : unmanaged
    {
        public readonly bool HasValue;

        public T Value;

        public NullableUnmanaged(T value)
        {
            Value = value;
            HasValue = true;
        }

        public static implicit operator NullableUnmanaged<T>(T value)
        {
            return new NullableUnmanaged<T>(value);
        }

        public static implicit operator T?(NullableUnmanaged<T> instance)
        {
            if (!instance.HasValue)
                return default;

            return instance.Value;
        }

        public static explicit operator T(NullableUnmanaged<T> instance)
        {
            return instance.Value;
        }

        public static bool operator ==(NullableUnmanaged<T> left, NullableUnmanaged<T> right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(NullableUnmanaged<T> left, NullableUnmanaged<T> right)
        {
            return !(left == right);
        }

        public readonly override bool Equals(object? obj)
        {
            return obj is NullableUnmanaged<T> unmanaged && Equals(unmanaged);
        }

        public readonly bool Equals(NullableUnmanaged<T> other)
        {
            return EqualityComparer<T>.Default.Equals(Value, other.Value) &&
                   HasValue == other.HasValue;
        }

        public readonly override int GetHashCode()
        {
            return HashCode.Combine(Value, HasValue);
        }
    }

    public static class NullableUnmanagedExtensions
    {
        public static NullableUnmanaged<T> ConvertToUnmanaged<T>(this T? source)
            where T : unmanaged
        {
            if (!source.HasValue)
                return default;

            return new NullableUnmanaged<T>(source.Value);
        }
    }
}
