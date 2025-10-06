using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

#nullable enable
namespace CCEnvs.Collections.Immutable
{
    public class ImmutableDictionary<TKey, TValue> : IReadOnlyDictionary<TKey, TValue>, IDictionary<TKey, TValue>
    {
        private readonly Dictionary<TKey, TValue> inner = new();

        public TValue this[TKey key] {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => inner[key];
        }
        public IEnumerable<TKey> Keys {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => inner.Keys;
        }
        public IEnumerable<TValue> Values {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => inner.Values;
        }
        public int Count {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => inner.Count;
        }

        ICollection<TKey> IDictionary<TKey, TValue>.Keys => throw new System.NotImplementedException();

        ICollection<TValue> IDictionary<TKey, TValue>.Values => throw new System.NotImplementedException();

        bool ICollection<KeyValuePair<TKey, TValue>>.IsReadOnly => throw new System.NotImplementedException();

        TValue IDictionary<TKey, TValue>.this[TKey key] { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }

        public ImmutableDictionary()
        {
        }

        public ImmutableDictionary(int capacity)
        {
            inner = new Dictionary<TKey, TValue>(capacity);
        }

        public ImmutableDictionary(IEnumerable<KeyValuePair<TKey, TValue>> items)
        {
            inner = new Dictionary<TKey, TValue>(items);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool ContainsKey(TKey key) => inner.ContainsKey(key);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TryGetValue(TKey key, out TValue value)
        {
            return inner.TryGetValue(key, out value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() => inner.GetEnumerator();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        IEnumerator IEnumerable.GetEnumerator() => inner.GetEnumerator();

        void IDictionary<TKey, TValue>.Add(TKey key, TValue value)
        {
            throw new System.NotImplementedException();
        }

        bool IDictionary<TKey, TValue>.Remove(TKey key)
        {
            throw new System.NotImplementedException();
        }

        void ICollection<KeyValuePair<TKey, TValue>>.Add(KeyValuePair<TKey, TValue> item)
        {
            throw new System.NotImplementedException();
        }

        void ICollection<KeyValuePair<TKey, TValue>>.Clear()
        {
            throw new System.NotImplementedException();
        }

        bool ICollection<KeyValuePair<TKey, TValue>>.Contains(KeyValuePair<TKey, TValue> item)
        {
            throw new System.NotImplementedException();
        }

        void ICollection<KeyValuePair<TKey, TValue>>.CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            throw new System.NotImplementedException();
        }

        bool ICollection<KeyValuePair<TKey, TValue>>.Remove(KeyValuePair<TKey, TValue> item)
        {
            throw new System.NotImplementedException();
        }
    }
}
