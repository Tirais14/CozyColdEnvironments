using CCEnvs.Collections;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

#nullable enable
namespace CCEnvs.Pools
{
    public readonly struct PooledDictionary<TKey, TValue>
        : 
        IDisposable,
        IDictionary<TKey, TValue>, IEquatable<PooledDictionary<TKey, TValue>>
    {
        private readonly PooledObject<Dictionary<TKey, TValue>> handle;

        public readonly TValue this[TKey key] {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => handle.Value[key];

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set => handle.Value[key] = value;
        }

        public readonly ICollection<TKey> Keys {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => handle.Value.Keys;
        }

        public readonly ICollection<TValue> Values {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => handle.Value.Values;
        }

        public readonly int Count {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => handle.Value.Count;
        }

        readonly bool ICollection<KeyValuePair<TKey, TValue>>.IsReadOnly => false;

        public PooledDictionary(int? capacity)
        {
            if (!capacity.HasValue || capacity.Value <= 0)
                handle = default;
            else
            {
                handle = DictionaryPool<TKey, TValue>.Shared.Get();
                handle.Value.EnsureCapacity(capacity.Value);
            }
        }

        public static PooledDictionary<TKey, TValue> Create()
        {
            return new PooledDictionary<TKey, TValue>(null);
        }
        public static PooledDictionary<TKey, TValue> Create(int capacity)
        {
            return new PooledDictionary<TKey, TValue>(capacity);
        }

        public static bool operator ==(PooledDictionary<TKey, TValue> left, PooledDictionary<TKey, TValue> right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(PooledDictionary<TKey, TValue> left, PooledDictionary<TKey, TValue> right)
        {
            return !(left == right);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly void Add(TKey key, TValue value)
        {
            handle.Value.Add(key, value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly void Add(KeyValuePair<TKey, TValue> item)
        {
            handle.Value.Add(item);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly void Clear() => handle.Value.Clear();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly bool Contains(KeyValuePair<TKey, TValue> item)
        {
            return handle.Value.Contains(item);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly bool ContainsKey(TKey key)
        {
            return handle.Value.ContainsKey(key);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly bool Remove(TKey key)
        {
            return handle.Value.Remove(key);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly bool Remove(KeyValuePair<TKey, TValue> item)
        {
            return handle.Value.Remove(item.Key);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly bool TryGetValue(TKey key, out TValue value)
        {
            return handle.Value.TryGetValue(key, out value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly void Dispose() => handle.Dispose();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int EnsureCapacity(int capacity)
        {
            return handle.Value.EnsureCapacity(capacity);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return handle.Value.GetEnumerator();
        }
        readonly IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        readonly void ICollection<KeyValuePair<TKey, TValue>>.CopyTo(
            KeyValuePair<TKey, TValue>[] array,
            int arrayIndex
            )
        {
            ((ICollection<KeyValuePair<TKey, TValue>>)handle.Value).CopyTo(array, arrayIndex);
        }

        public override bool Equals(object? obj)
        {
            return obj is PooledDictionary<TKey, TValue> dictionary && Equals(dictionary);
        }

        public bool Equals(PooledDictionary<TKey, TValue> other)
        {
            return handle.Equals(other.handle);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(handle);
        }
    }
}
