using CCEnvs.Collections;
using R3;
using System;
using System.Buffers;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;

#nullable enable
namespace CCEnvs.Pools
{
    public struct PooledArray<T> : IList<T>, IReadOnlyList<T>, IDisposable, IEquatable<PooledArray<T>>
    {
        public static PooledArray<T> Empty = new(0);

        private readonly IDisposable handle;

        private readonly T[] array;

        private readonly ArraySegment<T> value;

        public readonly ArraySegment<T> Value {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                if (value == default)
                    return ArraySegment<T>.Empty;

                return value;
            }
        }

        public readonly int Length {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => Value.Count;
        }

        public readonly T[] Raw => array;

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
            :
            this()
        {
            if (count <= 0)
            {
                array = new arr<T>();
                handle = Disposable.Empty;
                value = ArraySegment<T>.Empty;

                return;
            }

            array = ArrayPool<T>.Shared.Rent(count);

            handle = Disposable.Create(array,
                static array =>
                {
                    ArrayPool<T>.Shared.Return(array);
                });

            value = array.GetArraySegment(count);
        }

        public static PooledArray<T> FromRange(T item)
        {
            var array = new PooledArray<T>(1);

            array[0] = item;

            return array;
        }

        public static PooledArray<T> FromRange(T item, T item1)
        {
            var array = new PooledArray<T>(2);

            array[0] = item;
            array[1] = item1;

            return array;
        }

        public static PooledArray<T> FromRange(
            T item,
            T item1,
            T item2
            )
        {
            var array = new PooledArray<T>(3);

            array[0] = item;
            array[1] = item1;
            array[2] = item2;

            return array;
        }

        public static PooledArray<T> FromRange(
            T item,
            T item1,
            T item2,
            T item3
            )
        {
            var array = new PooledArray<T>();

            array[0] = item;
            array[1] = item1;
            array[2] = item2;
            array[3] = item3;

            return array;
        }

        public static PooledArray<T> FromRange(
            T item,
            T item1,
            T item2,
            T item3,
            T item4
            )
        {
            var array = new PooledArray<T>(5);

            array[0] = item;
            array[1] = item1;
            array[2] = item2;
            array[3] = item3;
            array[4] = item4;

            return array;
        }

        public static PooledArray<T> FromRange(
            T item,
            T item1,
            T item2,
            T item3,
            T item4,
            T item5
            )
        {
            var array = new PooledArray<T>(6);

            array[0] = item;
            array[1] = item1;
            array[2] = item2;
            array[3] = item3;
            array[4] = item4;
            array[5] = item5;

            return array;
        }

        public static PooledArray<T> FromRange(
            T item,
            T item1,
            T item2,
            T item3,
            T item4,
            T item5,
            T item6
            )
        {
            var array = new PooledArray<T>(7);

            array[0] = item;
            array[1] = item1;
            array[2] = item2;
            array[3] = item3;
            array[4] = item4;
            array[5] = item5;
            array[6] = item6;

            return array;
        }

        public static PooledArray<T> FromRange(
            T item,
            T item1,
            T item2,
            T item3,
            T item4,
            T item5,
            T item6,
            T item7
            )
        {
            var array = new PooledArray<T>(8);

            array[0] = item;
            array[1] = item1;
            array[2] = item2;
            array[3] = item3;
            array[4] = item4;
            array[5] = item5;
            array[6] = item6;
            array[7] = item7;

            return array;
        }

        public static PooledArray<T> FromRange(
            T item,
            T item1,
            T item2,
            T item3,
            T item4,
            T item5,
            T item6,
            T item7,
            T item8
            )
        {
            var array = new PooledArray<T>(9);

            array[0] = item;
            array[1] = item1;
            array[2] = item2;
            array[3] = item3;
            array[4] = item4;
            array[5] = item5;
            array[6] = item6;
            array[7] = item7;
            array[8] = item8;

            return array;
        }

        public static PooledArray<T> FromRange(
            T item,
            T item1,
            T item2,
            T item3,
            T item4,
            T item5,
            T item6,
            T item7,
            T item8,
            T item9
            )
        {
            var array = new PooledArray<T>(10);

            array[0] = item;
            array[1] = item1;
            array[2] = item2;
            array[3] = item3;
            array[4] = item4;
            array[5] = item5;
            array[6] = item6;
            array[7] = item7;
            array[8] = item8;
            array[9] = item9;

            return array;
        }

        public static implicit operator Span<T>(PooledArray<T> instance)
        {
            return instance.AsSpan();
        }

        public static implicit operator ReadOnlySpan<T>(PooledArray<T> instance)
        {
            return instance.AsSpan();
        }

        public static implicit operator Memory<T>(PooledArray<T> instance)
        {
            return instance.AsMemory();
        }

        public static implicit operator ReadOnlyMemory<T>(PooledArray<T> instance)
        {
            return instance.AsMemory();
        }

        public static bool operator ==(PooledArray<T> left, PooledArray<T> right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(PooledArray<T> left, PooledArray<T> right)
        {
            return !(left == right);
        }

        public readonly void Clear()
        {
            Array.Clear(Raw, 0, Raw.Length);
        }

        private int disposed;
        public void Dispose()
        {
            if (this == default
                ||
                Interlocked.Exchange(ref disposed, 1) != 0)
            {
                return;
            }

            handle.Dispose();
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
            array.CopyTo(array, arrayIndex);
        }

        public readonly Span<T> AsSpan(int start = 0, int? length = null)
        {
            return new Span<T>(array, start, length ?? value.Count);
        }

        public readonly Memory<T> AsMemory(int start = 0, int? length = null)
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
                   Length == other.Length;
        }

        public readonly override int GetHashCode()
        {
            return HashCode.Combine(handle, array, value, Length);
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
