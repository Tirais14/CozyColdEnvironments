using CCEnvs.Collections;
using CCEnvs.Conversations;
using CCEnvs.FuncLanguage;
using System;
using System.Buffers;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

#nullable enable
namespace CCEnvs.Linq
{
    public static class IEnumerableQuries
    {
        public static IEnumerable<KeyValuePair<TKey, TValue>> AsKeyValuePairs<TKey, TValue>(
            this IEnumerable<(TKey, TValue)> source)
        {
            return source.Select(x => new KeyValuePair<TKey, TValue>(x.Item1, x.Item2)).Where(x => true);
        }

        public static IEnumerable<KeyValuePair<TKey, T>> SelectValue<TKey, TValue, T>(
            this IEnumerable<KeyValuePair<TKey, TValue>> source,
            Func<TValue, T> selector)
        {
            return source.Select(x => new KeyValuePair<TKey, T>(x.Key, selector(x.Value)));
        }

        public static IEnumerable<KeyValuePair<T, TValue>> SelectKey<TKey, TValue, T>(
            this IEnumerable<KeyValuePair<TKey, TValue>> source,
            Func<TKey, T> selector)
        {
            return source.Select(x => new KeyValuePair<T, TValue>(selector(x.Key), x.Value));
        }

        public static T[] ForEachAndMaterialize<T>(this IEnumerable<T> values, Action<T> action)
        {
            CC.Guard.IsNotNull(values, nameof(values));
            CC.Guard.IsNotNull(action, nameof(action));

            var materialized = new List<T>();

            foreach (var value in values)
            {
                materialized.Add(value);
                action(value);
            }

            return materialized.ToArray();
        }

        public static void ForEach<T>(this IEnumerable<T> source)
        {
            CC.Guard.IsNotNullSource(source);

            foreach (var _ in source) { }
        }

        public static IEnumerable<T> RemoveElement<T>(this IEnumerable<T> values,
                                                      T removeValue)
        {
            CC.Guard.IsNotNull(values, nameof(values));

            foreach (var value in values)
            {
                if (Equals(value, removeValue))
                    continue;

                yield return value;
            }
        }

        public static IEnumerable<T> InsertElementAt<T>(
            this IEnumerable<T> values,
            int position,
            T newValue)
        {
            CC.Guard.IsNotNull(values, nameof(values));

            bool inserted = false;
            int i = 0;
            foreach (var value in values)
            {
                if (i == position)
                {
                    inserted = true;
                    yield return newValue;

                    i++;
                    yield return value;
                }
                else
                    yield return value;

                i++;
            }

#pragma warning disable S112
            if (!inserted)
                throw new IndexOutOfRangeException($"Index = {i}");
#pragma warning restore S112
        }

        public static IEnumerable<T> RemoveAt<T>(
            this IEnumerable<T> values,
            int position)
        {
            CC.Guard.IsNotNull(values, nameof(values));

            bool removed = false;
            int i = 0;
            foreach (var value in values)
            {
                if (i == position)
                    removed = true;
                else
                    yield return value;

                i++;
            }

#pragma warning disable S112
            if (!removed)
                throw new IndexOutOfRangeException($"Index = {i}");
#pragma warning restore S112
        }

        public static int HashCodeByElements<T>(this IEnumerable<T>? values)
        {
            if (values.IsNull())
                return 0;

            var hash = new HashCode();

            foreach ( var item in values)
                hash.Add(item);

            return hash.ToHashCode();
        }

        public static IEnumerable<KeyValuePair<TKey, TValue>> AssignKey<TKey, TValue>(
            this IEnumerable<TValue> values,
            Func<TValue, TKey> keySelector)
        {
            CC.Guard.IsNotNull(values, nameof(values));
            CC.Guard.IsNotNull(keySelector, nameof(keySelector));

            foreach (var value in values)
                yield return new KeyValuePair<TKey, TValue>(keySelector(value), value);
        }

        public static IEnumerable<TValue> UnassignKey<TKey, TValue>(
            this IEnumerable<KeyValuePair<TKey, TValue>> values)
        {
            return values.Select(x => x.Value);
        }

        public static T[] ToArrayOrEmpty<T>(this IEnumerable<T>? collection)
        {
            return collection?.ToArray() ?? Array.Empty<T>();
        }

        public static ReadOnlyCollection<T> AsReadOnly<T>(this IEnumerable<T> value)
        {
            CC.Guard.IsNotNull(value, nameof(value));

            if (value is IList<T> list)
                return new ReadOnlyCollection<T>(list);

            return new ReadOnlyCollection<T>(value.ToArray());
        }

        public static IEnumerable<TResult> DoTransformTypes<TResult>(this IEnumerable value)
        {
            CC.Guard.IsNotNull(value, nameof(value));

            foreach (var item in value)
                yield return (TResult)TypeMutator.MutateType(item, typeof(TResult));
        }

        public static bool TryGetNonEnumeratedCount<T>(this IEnumerable<T>? source, out int count)
        {
            count = -1;

            if (source.IsNull())
                return false;

            if (source is T[] array)
                count = array.Length;

            if (source is IReadOnlyCollection<T> readOnlyCollection)
                count = readOnlyCollection.Count;

            if (source is ICollection<T> collection)
                count = collection.Count;

            if (source is ICollection collectionUntyped)
                count = collectionUntyped.Count;

            return count != -1;
        }

        public static IEnumerable<T> Materialize<T>(this IEnumerable<T> source)
        {
            CC.Guard.IsNotNullSource(source);

            if (source is T[] array)
                return array;

            if (source is ICollection<T> collection)
                return collection;

            return source.ToArray();
        }

        public static Queue<T> ToQueue<T>(this IEnumerable<T> values)
        {
            CC.Guard.IsNotNull(values, nameof(values));

            var results = new Queue<T>();   
            foreach (var value in values)
                results.Enqueue(value);

            return results;
        }

        public static Stack<T> ToStack<T>(this IEnumerable<T> values)
        {
            CC.Guard.IsNotNull(values, nameof(values));

            var results = new Stack<T>();
            foreach (var value in values)
                results.Push(value);

            return results;
        }

        public static IEnumerable<TResult> CastCustom<TResult>(this IEnumerable source)
        {
            foreach (var item in source)
                yield return item.To<TResult>();
        }
    }
}