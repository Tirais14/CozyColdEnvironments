using CCEnvs.Collections;
using R3;
using System;
using System.Buffers;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

#nullable enable
namespace CCEnvs.Pools
{
    public struct PooledArray<T> : IList<T>, IReadOnlyList<T>, IDisposable, IEquatable<PooledArray<T>>
    {
        private readonly IDisposable handle;

        private readonly T[] array;

        private readonly ArraySegment<T> value;

        readonly public ArraySegment<T> Value {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                if (value == default)
                    return ArraySegment<T>.Empty;

                return value;
            }
        }

        readonly public int Length {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => Value.Count;
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

        public PooledArray(PooledObject<T[]> pooled, int count, int offset = 0)
            :
            this()
        {
            handle = pooled;

            value = pooled.Value.GetArraySegment(count, offset);
            array = pooled.Value;
        }

        public PooledArray(IDisposable poolReturnHandle, T[] array, int count, int offset = 0)
            :
            this()
        {
            this.handle = poolReturnHandle;

            value = array.GetArraySegment(count, offset);
            this.array = array;
        }

        public PooledArray(int count)
        {
            disposed = false;

            array = ArrayPool<T>.Shared.Rent(count);

            handle = Disposable.Create(array,
                static array =>
                {
                    ArrayPool<T>.Shared.Return(array);
                });

            value = array.GetArraySegment(count);
        }

        public static bool operator ==(PooledArray<T> left, PooledArray<T> right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(PooledArray<T> left, PooledArray<T> right)
        {
            return !(left == right);
        }

        private bool disposed;
        public void Dispose()
        {
            if (disposed
                ||
                this == default)
            {
                return;
            }

            handle.Dispose();

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

        public readonly override bool Equals(object? obj)
        {
            return obj is PooledArray<T> array && Equals(array);
        }

        public readonly bool Equals(PooledArray<T> other)
        {
            return EqualityComparer<IDisposable>.Default.Equals(handle, other.handle)
                   &&
                   EqualityComparer<T[]>.Default.Equals(array, other.array)
                   &&
                   EqualityComparer<ArraySegment<T>>.Default.Equals(value, other.value)
                   &&
                   Length == other.Length
                   &&
                   disposed == other.disposed;
        }

        public readonly override int GetHashCode()
        {
            return HashCode.Combine(handle, array, value, Length, disposed);
        }

        public readonly override string ToString()
        {
            if (this == default)
                return StringHelper.EMPTY_OBJECT;

            return $"({nameof(array)}: {array}; {nameof(Length)}: {Length})";
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
