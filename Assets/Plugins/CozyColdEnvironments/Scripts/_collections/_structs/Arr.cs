using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

#nullable enable
namespace CCEnvs.Collections
{
    public struct arr<T> : IEquatable<arr<T>>
    {
        private T[]? value;

        private bool hasValue;

        public T[] Value {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                if (!hasValue && value is null)
                {
                    value = Array.Empty<T>();
                    hasValue = true;
                }

                return value!;
            }
        }

        public arr(int size)
        {
            value = new T[size];
            hasValue = true;
        }

        public arr(IEnumerable<T> items)
            :
            this()
        {
            if (items is T[] array)
                value = array;
            else
                value = items.ToArray();


            hasValue = true;
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
            return arr.Value;
        }

        public override bool Equals(object? obj)
        {
            return obj is arr<T> arr && Equals(arr);
        }

        public bool Equals(arr<T> other)
        {
            return value == other.Value
                   &&
                   hasValue == other.hasValue;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(value, hasValue);
        }
    }
}
