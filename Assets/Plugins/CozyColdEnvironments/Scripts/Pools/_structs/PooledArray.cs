using CCEnvs.Collections;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

#nullable enable
namespace CCEnvs.Pools
{
    public struct PooledArray<T> : IList<T>, IDisposable
    {
        private const string COLLECTION_IS_READONLY = "Collection is read only";

        private readonly PooledHandle<T[]> pooledInternal;
        private readonly ArraySegment<T> value;
        private readonly T[] rawArray;

        readonly public ArraySegment<T> Value {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => value;
        }

        readonly public int Length {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => value.Count;
        }

        readonly int ICollection<T>.Count => Length;
        readonly bool ICollection<T>.IsReadOnly => true;

        readonly public T this[int index] {

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => value[index];

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set => this.value[index] = value;
        }

        public PooledArray(PooledHandle<T[]> pooled, int count, int offset = 0)
            :
            this()
        {
            pooledInternal = pooled;

            value = pooled.Value.GetArraySegment(count, offset);
            rawArray = pooled.Value;
        }

        private bool disposed;
        public void Dispose()
        {
            if (disposed)
                return;

            pooledInternal.Dispose();

            disposed = true;
        }

        readonly public int IndexOf(T item)
        {
            return rawArray.IndexOf(item);
        }

        readonly public bool Contains(T item)
        {
            return IndexOf(item) > -1;
        }

        readonly public void CopyTo(T[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        readonly public IEnumerator<T> GetEnumerator() => value.GetEnumerator();

        readonly IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        void IList<T>.Insert(int index, T item)
        {
            throw new NotSupportedException(COLLECTION_IS_READONLY);
        }

        void IList<T>.RemoveAt(int index)
        {
            throw new NotSupportedException(COLLECTION_IS_READONLY);
        }

        void ICollection<T>.Add(T item)
        {
            throw new NotSupportedException(COLLECTION_IS_READONLY);
        }

        void ICollection<T>.Clear()
        {
            throw new NotSupportedException(COLLECTION_IS_READONLY);
        }

        bool ICollection<T>.Remove(T item)
        {
            throw new NotSupportedException(COLLECTION_IS_READONLY);
        }
    }
}
