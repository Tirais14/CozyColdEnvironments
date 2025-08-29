#nullable enable
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace CCEnvs.Collections.Generic
{
    [JsonArray]
    [Serializable]
    [Obsolete("Use ReadOnlyCollection<T> instead,")]
    public sealed class ReadOnlyArray<T> 
        :
        IReadOnlyList<T>, 
        IEquatable<T[]>,
        IEquatable<ReadOnlyArray<T>>
    {
        [SerializeField]
        private T[] values;

        public T this[int index] {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => values[index];
        }
        public int Length {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => values?.Length ?? 0;
        }

        int IReadOnlyCollection<T>.Count => values.Length;

        public ReadOnlyArray(T[]? values)
        {
            this.values = values ?? Array.Empty<T>();
        }
        public ReadOnlyArray(IEnumerable<T>? values)
        {
            values ??= Array.Empty<T>();

            this.values = values.ToArray();
        }

        public static explicit operator T[](ReadOnlyArray<T> rdArray)
        {
            return (T[])rdArray.values.Clone();
        }

        public static bool operator ==(ReadOnlyArray<T> left, ReadOnlyArray<T> right)
        {
            return left.Equals(right);
        }

        public static bool operator ==(ReadOnlyArray<T> left, T[] right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(ReadOnlyArray<T> left, ReadOnlyArray<T> right)
        {
            return !left.Equals(right);
        }

        public static bool operator !=(ReadOnlyArray<T> left, T[] right)
        {
            return !left.Equals(right);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Equals(T[] other)
        {
            return other == values;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Equals(ReadOnlyArray<T> other)
        {
            return Equals(other.values);
        }

        public override bool Equals(object obj)
        {
            return obj is ReadOnlyArray<T> rdArray
                &&
                Equals(rdArray)
                || 
                obj is T[] array 
                &&
                Equals(array);
        }

        public override int GetHashCode()
        {
            return values.GetHashCode();
        }

        public override string ToString()
        {
            return values.ToString();
        }

        public IEnumerator<T> GetEnumerator()
        {
            return (IEnumerator<T>)(values?.GetEnumerator()
                    ?? 
                    Array.Empty<T>().GetEnumerator());
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return values.GetEnumerator();
        }
    }
}
