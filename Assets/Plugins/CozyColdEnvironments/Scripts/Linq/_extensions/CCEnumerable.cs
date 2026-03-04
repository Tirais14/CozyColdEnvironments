using CCEnvs.Conversations;
using CommunityToolkit.Diagnostics;
using SuperLinq;
using System;
using System.Buffers;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

#nullable enable
namespace CCEnvs.Linq
{
    public static class CCEnumerable
    {
        public static IEnumerable<KeyValuePair<TKey, TValue>> AsKeyValuePairs<TKey, TValue>(
            this IEnumerable<(TKey, TValue)> source)
        {
            CC.Guard.IsNotNull(source, nameof(source));

            return source.Select(x => new KeyValuePair<TKey, TValue>(x.Item1, x.Item2)).Where(x => true);
        }

        //public static IEnumerable<KeyValuePair<TKey, T>> SelectValue<TKey, TOut, T>(
        //    this IEnumerable<KeyValuePair<TKey, TOut>> source,
        //    Func<TOut, T> selector)
        //{
        //    CC.Guard.IsNotNull(source, nameof(source));

        //    foreach (var kvp in source)
        //        yield return new KeyValuePair<TKey, T>(kvp.Key, selector(kvp.Value));
        //}

        public static IEnumerable<TOut> SelectValue<TValue, TKey, TOut>(
            this IEnumerable<KeyValuePair<TKey, TValue>> source,
            Func<TValue, TOut> selector
            )
        {
            Guard.IsNotNull(source, nameof(source));
            Guard.IsNotNull(selector, nameof(selector));


            foreach (var kvp in source)
                yield return selector(kvp.Value);
        }

        public static IEnumerable<TValue> SelectValue<TValue, TKey>(
            this IEnumerable<KeyValuePair<TKey, TValue>> source
            )
        {
            Guard.IsNotNull(source, nameof(source));

            foreach (var kvp in source)
                yield return kvp.Value;
        }

        public static IEnumerable<TOut> SelectKey<TValue, TKey, TOut>(
            this IEnumerable<KeyValuePair<TKey, TValue>> source,
            Func<TKey, TOut> selector
            )
        {
            Guard.IsNotNull(source, nameof(source));
            Guard.IsNotNull(selector, nameof(selector));


            foreach (var kvp in source)
                yield return selector(kvp.Key);
        }

        public static IEnumerable<TKey> SelectKey<TValue, TKey>(
            this IEnumerable<KeyValuePair<TKey, TValue>> source
            )
        {
            Guard.IsNotNull(source, nameof(source));

            foreach (var kvp in source)
                yield return kvp.Key;
        }

        //public static IEnumerable<KeyValuePair<T, TOut>> SelectKey<TKey, TOut, T>(
        //    this IEnumerable<KeyValuePair<TKey, TOut>> source,
        //    Func<TKey, T> selector)
        //{
        //    CC.Guard.IsNotNull(source, nameof(source));

        //    foreach (var kvp in source)
        //        yield return KeyValuePair.Create(selector(kvp.Key), kvp.Value);
        //}

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

            foreach (var item in values)
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
            this IEnumerable<KeyValuePair<TKey, TValue>> source)
        {
            CC.Guard.IsNotNull(source, nameof(source));

            return source.Select(x => x.Value);
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
            if (source.IsNull())
            {
                count = -1;
                return false;
            }

            count = source switch
            {
                T[] arr => arr.Length,
                IReadOnlyCollection<T> col => col.Count,
                ICollection<T> col => col.Count,
                ICollection col => col.Count,
                _ => -1
            };

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

        public static T Single<T>(
            this IEnumerable<T> source,
            Func<Exception> notFoundExceptionFactory,
            Func<Exception> notSingleExceptionFactory
            )
        {
            CC.Guard.IsNotNullSource(source);
            Guard.IsNotNull(notFoundExceptionFactory, nameof(notFoundExceptionFactory));
            Guard.IsNotNull(notSingleExceptionFactory, nameof(notSingleExceptionFactory));

            int i = 0;

            T? first = default;

            foreach (var item in source)
            {
                if (i++ > 0)
                    throw notSingleExceptionFactory();

                first = item;
            }

            if (i == 0)
                throw notFoundExceptionFactory();

            return first!;
        }

        public static T Single<T, TState>(
            this IEnumerable<T> source,
            TState state,
            Func<TState, Exception> notFoundExceptionFactory,
            Func<TState, Exception> notSingleExceptionFactory
            )
        {
            CC.Guard.IsNotNullSource(source);
            Guard.IsNotNull(notFoundExceptionFactory, nameof(notFoundExceptionFactory));
            Guard.IsNotNull(notSingleExceptionFactory, nameof(notSingleExceptionFactory));

            int i = 0;

            T? first = default;

            foreach (var item in source)
            {
                if (i++ > 0)
                    throw notSingleExceptionFactory(state);

                first = item;
            }

            if (i == 0)
                throw notFoundExceptionFactory(state);

            return first!;
        }

        public static T Single<T>(
            this IEnumerable<T> source,
            Func<T, bool> predicate,
            Func<Exception> notFoundExceptionFactory,
            Func<Exception> notSingleExceptionFactory
            )
        {
            CC.Guard.IsNotNullSource(source);
            Guard.IsNotNull(predicate, nameof(predicate));
            Guard.IsNotNull(notFoundExceptionFactory, nameof(notFoundExceptionFactory));
            Guard.IsNotNull(notSingleExceptionFactory, nameof(notSingleExceptionFactory));

            int i = 0;

            T? first = default;

            foreach (var item in source)
            {
                if (!predicate(item))
                    continue;

                if (i++ > 0)
                    throw notSingleExceptionFactory();

                first = item;
            }

            if (i == 0)
                throw notFoundExceptionFactory();

            return first!;
        }

        public static T Single<T, TState>(
            this IEnumerable<T> source,
            TState state,
            Func<T, TState, bool> predicate,
            Func<TState, Exception> notFoundExceptionFactory,
            Func<TState, Exception> notSingleExceptionFactory
            )
        {
            CC.Guard.IsNotNullSource(source);
            Guard.IsNotNull(predicate, nameof(predicate));
            Guard.IsNotNull(notFoundExceptionFactory, nameof(notFoundExceptionFactory));
            Guard.IsNotNull(notSingleExceptionFactory, nameof(notSingleExceptionFactory));

            int i = 0;

            T? first = default;

            foreach (var item in source)
            {
                if (!predicate(item, state))
                    continue;

                if (i++ > 0)
                    throw notSingleExceptionFactory(state);

                first = item;
            }

            if (i == 0)
                throw notFoundExceptionFactory(state);

            return first!;
        }

        public static T? SingleOrDefault<T>(
            this IEnumerable<T> source,
            Func<Exception> notSingleExceptionFactory
            )
        {
            CC.Guard.IsNotNullSource(source);
            Guard.IsNotNull(notSingleExceptionFactory, nameof(notSingleExceptionFactory));

            int i = 0;

            T? first = default;

            foreach (var item in source)
            {
                if (i++ > 0)
                    throw notSingleExceptionFactory();

                first = item;
            }

            return first;
        }

        public static T? SingleOrDefault<T, TState>(
            this IEnumerable<T> source,
            TState state,
            Func<TState, Exception> notSingleExceptionFactory
            )
        {
            CC.Guard.IsNotNullSource(source);
            Guard.IsNotNull(notSingleExceptionFactory, nameof(notSingleExceptionFactory));

            int i = 0;

            T? first = default;

            foreach (var item in source)
            {
                if (i++ > 0)
                    throw notSingleExceptionFactory(state);

                first = item;
            }

            return first;
        }

        public static T? SingleOrDefault<T>(
            this IEnumerable<T> source,
            Func<T, bool> predicate,
            Func<Exception> notSingleExceptionFactory
            )
        {
            CC.Guard.IsNotNullSource(source);
            Guard.IsNotNull(predicate, nameof(predicate));
            Guard.IsNotNull(notSingleExceptionFactory, nameof(notSingleExceptionFactory));

            int i = 0;

            T? first = default;

            foreach (var item in source)
            {
                if (!predicate(item))
                    continue;

                if (i++ > 0)
                    throw notSingleExceptionFactory();

                first = item;
            }

            return first;
        }

        public static T? SingleOrDefault<T, TState>(
            this IEnumerable<T> source,
            TState state,
            Func<T, TState, bool> predicate,
            Func<TState, Exception> notSingleExceptionFactory
            )
        {
            CC.Guard.IsNotNullSource(source);
            Guard.IsNotNull(predicate, nameof(predicate));
            Guard.IsNotNull(notSingleExceptionFactory, nameof(notSingleExceptionFactory));

            int i = 0;

            T? first = default;

            foreach (var item in source)
            {
                if (!predicate(item, state))
                    continue;

                if (i++ > 0)
                    throw notSingleExceptionFactory(state);

                first = item;
            }

            return first;
        }

        public static IEnumerable<TOut> Select<T, TState, TOut>(
            this IEnumerable<T> source,
            TState state,
            Func<T, TState, TOut> selector
            )
        {
            return new SelectStatedEnumerator<T, TState, TOut>(source, state, selector);
        }

        public static IEnumerable<T> Where<T, TState>(
            this IEnumerable<T> source,
            TState state,
            Func<T, TState, bool> predicate
            )
        {
            return new WhereStatedEnumerator<T, TState>(source, state, predicate);
        }

        public static T First<T, TState>(
            this IEnumerable<T> source,
            TState state,
            Func<T, TState, bool> predicate
            )
        {
            CC.Guard.IsNotNullSource(source);
            Guard.IsNotNull(predicate, nameof(predicate));

            foreach (var item in source)
            {
                if (predicate(item, state))
                    return item;
            }

            throw new InvalidOperationException($"Enumerable {source} doesn't containt any items");
        }

        public static T? FirstOrDefault<T, TState>(
            this IEnumerable<T> source,
            TState state,
            Func<T, TState, bool> predicate
            )
        {
            CC.Guard.IsNotNullSource(source);
            Guard.IsNotNull(predicate, nameof(predicate));

            foreach (var item in source)
            {
                if (predicate(item, state))
                    return item;
            }

            return default;
        }

        //public static IEnumerable<T> MergeBy<T, TKey>(
        //    this IEnumerable<T> left,
        //    IEnumerable<T> right,
        //    Func<T, TKey> keySelector
        //    )
        //{
        //    CC.Guard.IsNotNull(left, nameof(left));
        //    CC.Guard.IsNotNull(right, nameof(right));
        //    Guard.IsNotNull(keySelector, nameof(keySelector));

        //    using var leftSet = HashSetPool<TKey>.Shared.Get();

        //    TKey leftKey;

        //    foreach (var leftItem in left)
        //    {
        //        leftKey = keySelector(leftItem);

        //        leftSet.Value.Add(leftKey);
        //    }

        //    TKey rightKey;

        //    foreach (var rightItem in right)
        //    {
        //        rightKey = keySelector(rightItem);

        //        if (leftSet.Value.Contains(rightKey))
        //            yield return rightItem;
        //    }
        //}
    }
}