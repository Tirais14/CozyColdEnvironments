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
        IDictionary<TKey, TValue>,
        IDisposable,
        IEquatable<PooledDictionary<TKey, TValue>>
    {
        private readonly PooledObject<Dictionary<TKey, TValue>> handle;

        public TValue this[TKey key] {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => handle.Value[key];

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set => handle.Value[key] = value;
        }

        public Dictionary<TKey, TValue> Dictionary {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => handle.Value;
        }

        public ICollection<TKey> Keys {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => handle.Value.Keys;
        }

        public ICollection<TValue> Values {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => handle.Value.Values;
        }

        public int Count {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => handle.Value.Count;
        }

        public readonly bool IsInitialized { get; }

        bool ICollection<KeyValuePair<TKey, TValue>>.IsReadOnly => false;

        public PooledDictionary(int? capacity)
        {
            if (capacity.HasValue)
                handle = DictionaryPool<TKey, TValue>.Shared.Get(capacity.Value);
            else
                handle = DictionaryPool<TKey, TValue>.Shared.Get();

            IsInitialized = true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(PooledDictionary<TKey, TValue> left, PooledDictionary<TKey, TValue> right)
        {
            return left.Equals(right);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(PooledDictionary<TKey, TValue> left, PooledDictionary<TKey, TValue> right)
        {
            return !(left == right);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int EnsureCapacity(int capacity)
        {
            return handle.Value.EnsureCapacity(capacity);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Add(TKey key, TValue value) => handle.Value.Add(key, value);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Add(KeyValuePair<TKey, TValue> item) => handle.Value.Add(item);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Clear() => handle.Value.Clear();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Contains(KeyValuePair<TKey, TValue> item) => handle.Value.Contains(item);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool ContainsKey(TKey key) => handle.Value.ContainsKey(key);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            ((ICollection<KeyValuePair<TKey, TValue>>)handle.Value).CopyTo(array, arrayIndex);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Remove(TKey key) => handle.Value.Remove(key);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Remove(KeyValuePair<TKey, TValue> item)
        {
            return ((IDictionary<TKey, TValue>)handle.Value).Remove(item);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TryGetValue(TKey key, out TValue value)
        {
            return handle.Value.TryGetValue(key, out value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Dispose() => handle.Dispose();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override bool Equals(object obj)
        {
            return obj is PooledDictionary<TKey, TValue> typed && Equals(typed);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly bool Equals(PooledDictionary<TKey, TValue> other)
        {
            return handle.Equals(other);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override int GetHashCode()
        {
            return handle.GetHashCode();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return handle.Value.GetEnumerator();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        IEnumerator IEnumerable.GetEnumerator() => handle.Value.GetEnumerator();
    }

}
