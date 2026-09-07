using CCEnvs.Collections;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

#nullable enable
namespace CCEnvs.Pools
{
    public struct PooledList<TValue>
        :
        IList<TValue>,
        IReadOnlyList<TValue>,
        IDisposable, 
        IEquatable<PooledList<TValue>>
    {
        private PooledObject<List<TValue>> handle;

        public readonly TValue this[int index] {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => handle.Value[index];
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set => handle.Value[index] = value;
        }

        public readonly int Count {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => handle.Value.Count;
        }

        public readonly int Capacity {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => handle.Value.Capacity;
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set => handle.Value.Capacity = value;
        }

        public readonly bool IsInitialized { get; }

        public readonly List<TValue> Value {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => handle.Value;
        }

        readonly bool ICollection<TValue>.IsReadOnly => false;

        public PooledList(int? capacity)
        {
            handle = ListPool<TValue>.Shared.Get();

            if (capacity.HasValue)
                handle.Value.TryIncreaseCapacity(capacity.Value);

            IsInitialized = true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(PooledList<TValue> left, PooledList<TValue> right)
        {
            return left.Equals(right);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(PooledList<TValue> left, PooledList<TValue> right)
        {
            return !(left == right);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator List<TValue>(PooledList<TValue> instance)
        {
            return instance.Value;   
        }

        public static PooledList<TValue> Create(int? capacity = null)
        {
            return new PooledList<TValue>(capacity);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly void Add(TValue item) => handle.Value.Add(item);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly void Clear() => handle.Value.Clear();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly bool Contains(TValue item) => handle.Value.Contains(item);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly void CopyTo(TValue[] array, int arrayIndex)
        {
            handle.Value.CopyTo(array, arrayIndex);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly int IndexOf(TValue item) => handle.Value.IndexOf(item);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly void Insert(int index, TValue item) => handle.Value.Insert(index, item);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly bool Remove(TValue item) => handle.Value.Remove(item);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly void RemoveAt(int index) => handle.Value.RemoveAt(index);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly TValue[] ToArray() => handle.Value.ToArray();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Dispose()
        {
            handle.Dispose();
            handle = default;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly IEnumerator<TValue> GetEnumerator()
        {
            if (!handle.IsValid)
                return Array.Empty<TValue>().GetEnumeratorT();

            return handle.Value.GetEnumerator();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        readonly IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public readonly override bool Equals(object? obj)
        {
            return obj is PooledList<TValue> list && Equals(list);
        }

        public readonly bool Equals(PooledList<TValue> other)
        {
            return handle.Equals(other.handle) &&
                   IsInitialized == other.IsInitialized;
        }

        public readonly override int GetHashCode()
        {
            return HashCode.Combine(handle, IsInitialized);
        }
    }
}
