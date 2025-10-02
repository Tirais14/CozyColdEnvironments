using System;
using System.Collections.Generic;

#nullable enable

namespace CCEnvs.Collections
{
    public static class IDictionaryExtensions
    {
        public static bool TrySetValue<TKey, TValue>(this IDictionary<TKey, TValue> collection,
                                                     TKey key,
                                                     TValue value)
        {
            if (!collection.ContainsKey(key))
                return false;

            collection[key] = value;
            return true;
        }

        public static bool TrySetValue<TKey, TValue>(this IDictionary<TKey, TValue> collection,
                                                     KeyValuePair<TKey, TValue> pair)
        {
            if (!collection.ContainsKey(pair.Key))
                return false;

            collection[pair.Key] = pair.Value;
            return true;
        }

        public static void Add<TKey, TValue>(this IDictionary<Type, Type> value)
        {
            CC.Guard.NullArgument(value, nameof(value));

            value.Add(typeof(TKey), typeof(TValue));
        }

        public static void Add<TKey, TValue>(this IDictionary<TKey, TValue> dictionary,
                                             KeyValuePair<TKey, TValue> item)
        {
            dictionary.Add(item.Key, item.Value);
        }
    }
}