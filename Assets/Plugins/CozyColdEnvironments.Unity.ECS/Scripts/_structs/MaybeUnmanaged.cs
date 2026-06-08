using System;
using Unity.Burst;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Mathematics;

#nullable enable
namespace CCEnvs.UnityX.ECS
{
    public struct MaybeUnmanaged<T> : IEquatable<MaybeUnmanaged<T>> where T : unmanaged
    {
        public static MaybeUnmanaged<T> Null => new();

        public readonly bool HasValue;

        public T Value;

        public MaybeUnmanaged(T value)
        {
            Value = value;
            HasValue = true;
        }

        public static implicit operator MaybeUnmanaged<T>(T value)
        {
            return new MaybeUnmanaged<T>(value);
        }

        public static implicit operator T?(MaybeUnmanaged<T> instance)
        {
            if (!instance.HasValue)
                return default;

            return instance.Value;
        }

        public static explicit operator T(MaybeUnmanaged<T> instance)
        {
            return instance.Value;
        }

        public static bool operator ==(MaybeUnmanaged<T> left, MaybeUnmanaged<T> right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(MaybeUnmanaged<T> left, MaybeUnmanaged<T> right)
        {
            return !(left == right);
        }

        public MaybeUnmanaged<TInterpret> Reinterpret<TInterpret>()
            where TInterpret : unmanaged
        {
            if (!HasValue)
                return MaybeUnmanaged<TInterpret>.Null;

            return UnsafeUtility.As<T, TInterpret>(ref Value);
        }

        public readonly override bool Equals(object? obj)
        {
            return obj is MaybeUnmanaged<T> unmanaged && Equals(unmanaged);
        }

        [BurstCompile]
        public readonly bool Equals(MaybeUnmanaged<T> other)
        {
            return GetHashCode() == other.GetHashCode();
        }

        [BurstCompile]
        public readonly override int GetHashCode()
        {
            return (int)math.hash(new int2(HasValue.GetHashCode(), Value.GetHashCode()));
        }
    }

    public static class NullableUnmanagedExtensions
    {
        public static MaybeUnmanaged<T> ToUnmanaged<T>(this T? source)
            where T : unmanaged
        {
            if (!source.HasValue)
                return default;

            return new MaybeUnmanaged<T>(source.Value);
        }

        [BurstCompile]
        public static bool EqualsUnmanaged<T>(this MaybeUnmanaged<T> left, MaybeUnmanaged<T> right)
            where T : unmanaged, IEquatable<T>
        {
            return left.HasValue == right.HasValue
                   &&
                   left.Value.Equals(right.Value);
        }

        [BurstCompile]
        public static bool NotEqualsUnmanaged<T>(this MaybeUnmanaged<T> left, MaybeUnmanaged<T> right)
            where T : unmanaged, IEquatable<T>
        {
            return !left.EqualsUnmanaged(right);
        }
    }
}
