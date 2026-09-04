using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using ZLinq;

#nullable enable
namespace CCEnvs.Pools
{
    public readonly struct PooledHashSet<TValue> 
        : 
        ICollection<TValue>,
        IDisposable,
        IEquatable<PooledHashSet<TValue>>
    {
        private readonly PooledObject<HashSet<TValue>> handle;

        public readonly HashSet<TValue> Value => handle.Value;

        public readonly int Count {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => handle.Value.Count;
        }

        public readonly bool IsInitialized { get; }

        readonly bool ICollection<TValue>.IsReadOnly => false;

        public PooledHashSet(int? capacity)
        {
            handle = HashSetPool<TValue>.Shared.Get();

            if (capacity.HasValue)
                handle.Value.EnsureCapacity(capacity.Value);

            IsInitialized = true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(PooledHashSet<TValue> left, PooledHashSet<TValue> right)
        {
            return left.Equals(right);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(PooledHashSet<TValue> left, PooledHashSet<TValue> right)
        {
            return !(left == right);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator HashSet<TValue>(PooledHashSet<TValue> instance)
        {
            return instance.Value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static PooledHashSet<TValue> Create(int? capacity = null)
        {
            return new PooledHashSet<TValue>(capacity);
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
        public readonly IEnumerator<TValue> GetEnumerator()
        {
            return handle.Value.GetEnumerator();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly bool Remove(TValue item)
        {
            return handle.Value.Remove(item);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly void Dispose() => handle.Dispose();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        readonly IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public override bool Equals(object? obj)
        {
            return obj is PooledHashSet<TValue> set && Equals(set);
        }

        public bool Equals(PooledHashSet<TValue> other)
        {
            return handle.Equals(other.handle) &&
                   IsInitialized == other.IsInitialized;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(handle, IsInitialized);
        }
    }
}
