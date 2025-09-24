#nullable enable
using CCEnvs.Attributes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace CCEnvs.Collections.Immutable
{
    public readonly struct ImmutableArray<T>
        : IReadOnlyList<T>,
        IList<T>,
        IEquatable<ImmutableArray<T>>
    {
        public static ImmutableArray<T> Empty { get; } = new(Array.Empty<T>(), empty: true);

        private readonly static NotSupportedException readonlyCollectionException = new("Collection is readonly.");
        private readonly T[] inner;

        public T this[int index] {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => inner[index];
        }
        public int Length {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => inner.Length;
        }

        int ICollection<T>.Count => Length;
        int IReadOnlyCollection<T>.Count => Length;
        bool ICollection<T>.IsReadOnly => true;

        T IList<T>.this[int index] {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => inner[index];
            set => throw readonlyCollectionException; }

        private ImmutableArray(T[] array, bool empty)
        {
            if (array.IsNotEmpty())
                throw new InvalidOperationException("Array must be empty.");

            inner = array;
            _ = empty;
        }

        public ImmutableArray(IEnumerable<T> collection)
        {
            inner = collection.ToArray();
        }

        public static bool operator ==(ImmutableArray<T> left, ImmutableArray<T> right)
        {
            return left.Equals(right);
        }
        public static bool operator !=(ImmutableArray<T> left, ImmutableArray<T> right)
        {
            return !(left == right);
        }

        [Converter]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T[] ToArray() => inner.ToArray();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int IndexOf(T item) => Array.IndexOf(inner, item);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Contains(T item) => Array.IndexOf(inner, item) > -1;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void CopyTo(T[] array, int arrayIndex)
        {
            inner.CopyTo(array, arrayIndex);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Equals(ImmutableArray<T> other)
        {
            return inner.Equals(other.inner);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override bool Equals(object obj)
        {
            return obj is ImmutableArray<T> typed && Equals(typed);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override int GetHashCode()
        {
            return HashCode.Combine(inner);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IEnumerator<T> GetEnumerator()
        {
            return inner?.GetEnumeratorT() ?? Enumerable.Empty<T>().GetEnumerator();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        void IList<T>.Insert(int index, T item)
        {
            throw readonlyCollectionException;
        }

        void IList<T>.RemoveAt(int index)
        {
            throw readonlyCollectionException;
        }

        void ICollection<T>.Add(T item)
        {
            throw readonlyCollectionException;
        }

        void ICollection<T>.Clear()
        {
            throw readonlyCollectionException;
        }

        bool ICollection<T>.Remove(T item)
        {
            throw readonlyCollectionException;
        }
    }
}
