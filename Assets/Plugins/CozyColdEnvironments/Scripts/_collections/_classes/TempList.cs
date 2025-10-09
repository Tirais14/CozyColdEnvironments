using CCEnvs.Collections.ObjectModel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

#nullable enable
namespace CCEnvs.Collections
{
    /// <summary>
    /// Same as the <see cref="List{T}"/>, but return in result internal array, for non-allocating materializing
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public readonly struct TempList<T> : IList<T>
    {
        private readonly ListBase<T> inner;

        public int Capacity {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => inner.Capacity;
        }

        public int Count {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => inner.Count;
        }

        public T[] ArrayInternal {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => inner?.ArrayInternal ?? Array.Empty<T>();
        }

        public bool IsReadOnly => throw new NotImplementedException();

        T IList<T>.this[int index] { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public T this[int index] {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => inner[index];
        }

        public TempList(int length)
        {
            inner = new ListBase<T>(length);
        }

        public TempList(IEnumerable<T> collection)
        {
            inner = new ListBase<T>(collection);
        }


        public TempList(IEnumerable<T> collection, int length)
        {
            inner = new ListBase<T>(collection, length);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator T[](TempList<T> source)
        {
            return source.inner.ArrayInternal;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Add(T item) => inner.Add(item);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int IndexOf(T item) => inner.IndexOf(item);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Contains(T item) => inner.Contains(item);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Insert(int index, T item) => inner.Insert(index, item);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Remove(T item) => inner.Remove(item);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void RemoveAt(int index) => inner.RemoveAt(index);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Clear() => inner.Clear();

        /// <returns><see cref="ArrayInternal"/></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T[] ToArray() => inner.ArrayInternal;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IEnumerator<T> GetEnumerator() => inner.GetEnumerator();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void CopyTo(T[] array, int arrayIndex)
        {
            inner.CopyTo(array, arrayIndex);
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
