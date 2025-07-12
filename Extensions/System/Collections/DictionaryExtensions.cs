using System.Collections.Generic;

#nullable enable

namespace UTIRLib
{
    public static class DictionaryExtensions
    {
        public static bool TrySetValue<TKey, TValue>(this Dictionary<TKey, TValue> collection,
                                                     TKey key,
                                                     TValue value)
        {
            if (!collection.ContainsKey(key))
                return false;

            collection[key] = value;
            return true;
        }
        public static bool TrySetValue<TKey, TValue>(this Dictionary<TKey, TValue> collection,
                                                     KeyValuePair<TKey, TValue> pair)
        {
            if (!collection.ContainsKey(pair.Key))
                return false;

            collection[pair.Key] = pair.Value;
            return true;
        }

        public static void Add<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, KeyValuePair<TKey, TValue> item) =>
            dictionary.Add(item.Key, item.Value);

        public static void AddRange<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, IEnumerable<KeyValuePair<TKey, TValue>> items)
        {
            foreach (var item in items)
            {
                dictionary.Add(item);
            }
        }

        public static void AddRange<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, KeyValuePair<TKey, TValue>[] items)
        {
            int itemsCount = items.Length;
            for (int i = 0; i < itemsCount; i++)
            {
                dictionary.Add(items[i]);
            }
        }
    }
}