using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

#nullable enable
namespace CCEnvs.Collections
{
    public readonly struct arr<T> : IEquatable<arr<T>>
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

        public arr(int size)
        {
            value = new T[size];
            IsDefault = false;
        }

        public arr(IEnumerable<T> items)
            :
            this()
        {
            if (items is T[] array)
            {
                value = array;
                return;
            }

            value = items.ToArray();

            IsDefault = false;
        }

        public static bool operator ==(arr<T> left, arr<T> right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(arr<T> left, arr<T> right)
        {
            return !(left == right);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator T[](arr<T> arr)
        {
            return arr.value;
        }

        public override bool Equals(object? obj)
        {
            return obj is arr<T> arr && Equals(arr);
        }

        public bool Equals(arr<T> other)
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
