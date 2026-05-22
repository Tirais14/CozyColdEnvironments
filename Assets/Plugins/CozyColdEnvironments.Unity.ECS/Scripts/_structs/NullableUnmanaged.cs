using System;
using Unity.Burst;

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
            return GetHashCode() == other.GetHashCode();
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

        [BurstCompile]
        public static bool EqualsUnmanaged<T>(this NullableUnmanaged<T> left, NullableUnmanaged<T> right)
            where T : unmanaged, IEquatable<T>
        {
            return left.HasValue == right.HasValue
                   &&
                   left.Value.Equals(right.Value);
        }

        [BurstCompile]
        public static bool NotEqualsUnmanaged<T>(this NullableUnmanaged<T> left, NullableUnmanaged<T> right)
            where T : unmanaged, IEquatable<T>
        {
            return !left.EqualsUnmanaged(right);
        }
    }
}
