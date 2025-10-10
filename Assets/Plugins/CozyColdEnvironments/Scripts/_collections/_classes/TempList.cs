using CCEnvs.Collections.ObjectModel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

#nullable enable
namespace CCEnvs.Collections
{
    [Obsolete("Cause issues", true)]
    /// <summary>
    /// Same as the <see cref="List{T}"/>, but return in result internal array, for non-allocating materializing
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public struct TempList<T> : IList<T>
    {
        private ListBase<T> _inner;

        private ListBase<T> Inner {
            get
            {
                _inner ??= new ListBase<T>();

                return _inner;
            }
        }

        public int Capacity {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => Inner.Capacity;
        }

        public int Count {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => Inner.Count;
        }

        public T[] ArrayInternal {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => Inner?.ArrayInternal ?? Array.Empty<T>();
        }

        public T this[int index] {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => Inner[index];
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set => Inner[index] = value;
        }

        readonly bool ICollection<T>.IsReadOnly => false;

        public TempList(int length)
        {
            _inner = new ListBase<T>(length);
        }

        public TempList(IEnumerable<T> collection)
        {
            _inner = new ListBase<T>(collection);
        }


        public TempList(IEnumerable<T> collection, int length)
        {
            _inner = new ListBase<T>(collection, length);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator T[](TempList<T> source)
        {
            return source.Inner.ArrayInternal;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Add(T item) => Inner.Add(item);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int IndexOf(T item) => Inner.IndexOf(item);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Contains(T item) => Inner.Contains(item);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Insert(int index, T item) => Inner.Insert(index, item);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Remove(T item) => Inner.Remove(item);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void RemoveAt(int index) => Inner.RemoveAt(index);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Clear() => Inner.Clear();

        /// <returns><see cref="ArrayInternal"/></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T[] ToArray() => Inner.ArrayInternal;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IEnumerator<T> GetEnumerator() => Inner.GetEnumerator();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void CopyTo(T[] array, int arrayIndex)
        {
            Inner.CopyTo(array, arrayIndex);
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
