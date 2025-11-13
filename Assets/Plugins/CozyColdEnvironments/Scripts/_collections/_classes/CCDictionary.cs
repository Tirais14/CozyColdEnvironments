using CCEnvs.Linq;
using System.Collections;
using System.Collections.Generic;

#nullable enable
namespace CCEnvs.Collections
{
    public class CCDictionary<TKey, TValue> : ICCDictionary<TKey, TValue>
    {
        protected readonly IDictionary<TKey, TValue> collection = new Dictionary<TKey, TValue>();

        public Result<TValue> this[TKey key] {
            get
            {
                if (collection.TryGetValue(key, out var result))
                    return (result, null);

                return (default, new KeyNotFoundException($"Key: {key}"));
            }
            set => collection[key] = value.Raw!;
        }

        public IEnumerable<TKey> Keys => collection.Keys;
        public IEnumerable<TValue> Values => collection.Values;
        public int Count => collection.Count;

        bool ICollection<KeyValuePair<TKey, TValue>>.IsReadOnly => false;

        public CCDictionary()
        {
        }

        public CCDictionary(int capacity)
        {
            collection = new Dictionary<TKey, TValue>(capacity);
        }

        public CCDictionary(IEnumerable<KeyValuePair<TKey, TValue>> items)
        {
            collection = new Dictionary<TKey, TValue>(items);
        }

        public CCDictionary(params (TKey, TValue)[] items)
            :
            this(items.AsKeyValuePairs())
        {
        }

        public CCDictionary(IDictionary<TKey, TValue> dictionary)
        {
            collection = dictionary;
        }

        public void Add(TKey key, TValue value)
        {
            collection.Add(key, value);
        }

        public void Add(KeyValuePair<TKey, TValue> item)
        {
            collection.Add(item);
        }

        public void Clear()
        {
            collection.Clear();
        }

        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            return collection.Contains(item);
        }

        public bool ContainsKey(TKey key)
        {
            return collection.ContainsKey(key);
        }

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            collection.CopyTo(array, arrayIndex);
        }

        public bool Remove(KeyValuePair<TKey, TValue> item)
        {
            return collection.Remove(item);
        }

        public bool Remove(TKey key)
        {
            return collection.Remove(key);
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return collection.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
