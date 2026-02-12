using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

#nullable enable
namespace CCEnvs.Collections
{
    public struct StructuralArray<T> : IList<T>, IEquatable<StructuralArray<T>>
    {
        public static StructuralArray<T> Empty => new(Array.Empty<T>());

        private readonly T[] array;

        private int? hashCode;

        public readonly T this[int index] {

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => array[index];  
        }

        public readonly int Length => array.Length;

        readonly int ICollection<T>.Count => array.Length;

        readonly bool ICollection<T>.IsReadOnly => true;

        readonly T IList<T>.this[int index] {

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => array[index];

            set => CC.ThrowHelper.ReadOnlyCollection(this);
        }

        public StructuralArray(IEnumerable<T> items)
        {
            CC.Guard.IsNotNull(items, nameof(items));

            array = items.ToArray();
            hashCode = null;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(StructuralArray<T> left, StructuralArray<T> right)
        {
            return left.Equals(right);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(StructuralArray<T> left, StructuralArray<T> right)
        {
            return !(left == right);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator T[] (StructuralArray<T> instance)
        {
            return instance.ToArray();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly bool Contains(T item)
        {
            if (array is null)
                return false;

            return array.Contains(item);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly void CopyTo(T[] array, int arrayIndex)
        {
            if (array is null)
                return;

            array.CopyTo(array, arrayIndex);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly int IndexOf(T item)
        {
            if (array is null)
                return -1;

            return array.IndexOf(item);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly T[] ToArray()
        {
            if (array is null)
                return new arr<T>();

            return array.ToArray();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly override bool Equals(object? obj)
        {
            return obj is StructuralArray<T> array && Equals(array);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly bool Equals(StructuralArray<T> other)
        {
            return Length == other.Length
                   &&
                   EqaulsByElements(other.array);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override int GetHashCode()
        {
            hashCode ??= HashCode.Combine(array, Length);

            return hashCode.Value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly IEnumerator<T> GetEnumerator()
        {
            return array.GetEnumeratorT();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        readonly IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private readonly bool EqaulsByElements(T[] other)
        {
            if (array is null)
            {
                if (other is not null)
                    return false;

                return true;
            }

            if (other is null)
                return false;

            return array.EqualsByElements(other);
        }

        readonly void IList<T>.Insert(int index, T item)
        {
            throw CC.ThrowHelper.ReadOnlyCollection(this);
        }

        readonly bool ICollection<T>.Remove(T item)
        {
            throw CC.ThrowHelper.ReadOnlyCollection(this);
        }

        readonly void IList<T>.RemoveAt(int index)
        {
            throw CC.ThrowHelper.ReadOnlyCollection(this);
        }

        readonly void ICollection<T>.Add(T item)
        {
            throw CC.ThrowHelper.ReadOnlyCollection(this);
        }

        readonly void ICollection<T>.Clear()
        {
            throw CC.ThrowHelper.ReadOnlyCollection(this);
        }
    }
}
