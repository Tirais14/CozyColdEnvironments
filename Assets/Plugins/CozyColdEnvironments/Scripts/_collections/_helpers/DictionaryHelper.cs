using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using CCEnvs.FuncLanguage;
using CommunityToolkit.Diagnostics;

#nullable enable
namespace CCEnvs.Collections
{
    public static class DictionaryHelper
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

        public static TValue GetOrCreate<TKey, TValue>(
            this IDictionary<TKey, TValue> source,
            TKey key,
            Func<TValue> factory)
        {
            CC.Guard.IsNotNullSource(source);
            Guard.IsNotNull(factory, nameof(factory));

            if (!source.TryGetValue(key, out TValue value))
            {
                value = factory();
                source.Add(key, value);
            }

            return value;
        }

        public static TValue GetOrCreate<TKey, TValue, TState>(
            this IDictionary<TKey, TValue> source,
            TKey key,
            TState state,
            Func<TState, TValue> factory)
        {
            CC.Guard.IsNotNullSource(source);
            Guard.IsNotNull(factory, nameof(factory));

            if (!source.TryGetValue(key, out TValue value))
            {
                value = factory(state);
                source.Add(key, value);
            }

            return value;
        }

        public static TValue GetOrCreateNew<TKey, TValue>(this IDictionary<TKey, TValue> source, TKey key)
            where TValue : new()
        {
            if (!source.TryGetValue(key, out TValue value))
            {
                value = new TValue();
                source.Add(key, value);
            }

            return value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Maybe<TValue> GetMaybeValue<TKey, TValue>(this IDictionary<TKey, TValue> source, TKey key)
        {
            CC.Guard.IsNotNullSource(source);

            if (!source.TryGetValue(key, out var result))
                return Maybe<TValue>.None;

            return result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Maybe<TValue> GetMaybeValueReadOnly<TKey, TValue>(this IReadOnlyDictionary<TKey, TValue> source, TKey key)
        {
            CC.Guard.IsNotNullSource(source);

            if (!source.TryGetValue(key, out var result))
                return Maybe<TValue>.None;

            return result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IDictionary<TKey, TValue> Merge<TKey, TValue>(
            this IDictionary<TKey, TValue> left,
            IEnumerable<KeyValuePair<TKey, TValue>> right
            )
        {
            CC.Guard.IsNotNull(left, nameof(left));

            if (left.IsReadOnly)
                throw CC.ThrowHelper.CollectionIsReadOnly();

            CC.Guard.IsNotNull(right, nameof(right));

            if (right.IsEmpty())
                return left;

            if (left.IsEmpty())
            {
                left.AddRange(right);
                return left;
            }

            foreach (var (rightKey, rightValue) in right)
            {
                if (!left.TryAdd(rightKey, rightValue))
                    left[rightKey] = rightValue;
            }

            return left;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IDictionary<TKey, TValue> Override<TKey, TValue>(
            this IDictionary<TKey, TValue> left,
            IEnumerable<KeyValuePair<TKey, TValue>> right
            )
        {
            CC.Guard.IsNotNull(left, nameof(left));

            if (left.IsReadOnly)
                throw CC.ThrowHelper.CollectionIsReadOnly();

            CC.Guard.IsNotNull(right, nameof(right));

            left.Clear();
            left.AddRange(right);

            return left;
        }
    }
}