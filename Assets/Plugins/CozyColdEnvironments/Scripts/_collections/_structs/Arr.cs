using System;
using System.Runtime.CompilerServices;

#nullable enable
namespace CCEnvs.Collections
{
    public readonly struct Arr<T> : IEquatable<Arr<T>>
    {
        private readonly T[] value;

        public readonly T[] Value {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                if (IsDefault)
                    return Array.Empty<T>();

                return value;
            }
        }

        public bool IsDefault { get; }

        public Arr(int size)
        {
            value = new T[size];
            IsDefault = false;
        }

        public static bool operator ==(Arr<T> left, Arr<T> right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Arr<T> left, Arr<T> right)
        {
            return !(left == right);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator T[](Arr<T> arr)
        {
            return arr.value;
        }

        public override bool Equals(object? obj)
        {
            return obj is Arr<T> arr && Equals(arr);
        }

        public bool Equals(Arr<T> other)
        {
            return value == other.Value
                   &&
                   IsDefault == other.IsDefault;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(value, IsDefault);
        }
    }
}
