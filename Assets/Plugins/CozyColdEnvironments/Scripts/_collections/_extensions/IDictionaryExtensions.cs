using CCEnvs.FuncLanguage;
using System;
using System.Collections.Generic;
using System.Linq;

#nullable enable

namespace CCEnvs.Collections
{
    public static class IDictionaryExtensions
    {
        public static Maybe<TKey> GetKey<TKey, TValue>(this IEnumerable<KeyValuePair<TKey, TValue>> source, TValue value)
        {
            CC.Guard.IsNotNull(source, nameof(source));

            return source.FirstOrDefault(pair => EqualityComparer<TValue>.Default.Equals(pair.Value, value)).Key;
        }

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
            CC.Guard.IsNotNull(value, nameof(value));

            value.Add(typeof(TKey), typeof(TValue));
        }

        public static void Add<TKey, TValue>(this IDictionary<TKey, TValue> dictionary,
                                             KeyValuePair<TKey, TValue> item)
        {
            dictionary.Add(item.Key, item.Value);
        }
    }
}