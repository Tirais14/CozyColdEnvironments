using CCEnvs.Collections;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

#nullable enable
namespace CCEnvs.Pools
{
    public struct PooledArray<T> : IList<T>, IReadOnlyList<T>, IDisposable
    {
        private readonly IDisposable poolReturnHandle;

        private readonly T[] array;

        private readonly ArraySegment<T> value;

        readonly public ArraySegment<T> Value {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => value;
        }

        readonly public int Length {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => value.Count;
        }

        readonly int ICollection<T>.Count => Length;
        readonly int IReadOnlyCollection<T>.Count => Length;
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
            poolReturnHandle = pooled;

            value = pooled.Value.GetArraySegment(count, offset);
            array = pooled.Value;
        }

        public PooledArray(IDisposable poolReturnHandle, T[] array, int count, int offset = 0)
            :
            this()
        {
            this.poolReturnHandle = poolReturnHandle;

            value = array.GetArraySegment(count, offset);
            this.array = array;
        }

        private bool disposed;
        public void Dispose()
        {
            if (disposed)
                return;

            poolReturnHandle.Dispose();

            disposed = true;
        }

        public readonly int IndexOf(T item)
        {
            return array.IndexOf(item);
        }

        public readonly bool Contains(T item)
        {
            return IndexOf(item) > -1;
        }

        public readonly void CopyTo(T[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public readonly Span<T> GetSpan(int start = 0, int? length = null)
        {
            return new Span<T>(array, start, length ?? value.Count);
        }

        public readonly Memory<T> GetMemory(int start = 0, int? length = null)
        {
            return new Memory<T>(array, start, length ?? value.Count);
        }

        public readonly IEnumerator<T> GetEnumerator() => value.GetEnumerator();

        readonly IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        readonly void IList<T>.Insert(int index, T item)
        {
            throw CC.ThrowHelper.CollectionIsReadOnly(GetType());
        }

        readonly void IList<T>.RemoveAt(int index)
        {
            throw CC.ThrowHelper.CollectionIsReadOnly(GetType());
        }

        readonly void ICollection<T>.Add(T item)
        {
            throw CC.ThrowHelper.CollectionIsReadOnly(GetType());
        }

        readonly void ICollection<T>.Clear()
        {
            throw CC.ThrowHelper.CollectionIsReadOnly(GetType());
        }

        readonly bool ICollection<T>.Remove(T item)
        {
            throw CC.ThrowHelper.CollectionIsReadOnly(GetType());
        }
    }
}
