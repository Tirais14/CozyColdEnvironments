using CCEnvs.Conversations;
using System;
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
            return source.Select(x => new KeyValuePair<TKey, TValue>(x.Item1, x.Item2));
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

        public static T[] ForEach<T>(this IEnumerable<T> values, Action<T> action)
        {
            CC.Validate.ArgumentNull(values, nameof(values));
            CC.Validate.ArgumentNull(action, nameof(action));

            T[] materialized = values.ToArray();
            int count = materialized.Length;
            for (int i = 0; i < count; i++)
                action(materialized[i]);

            return materialized;
        }

        ///// <summary>
        ///// Doesn't materialize collection
        ///// </summary>
        ///// <typeparam name="T"></typeparam>
        ///// <param name="values"></param>
        ///// <param name="action"></param>
        ///// <returns></returns>
        //public static IEnumerable<T> Action<T>(this IEnumerable<T> values, Action<T> action)
        //{
        //    return values.Select(x => { action(x); return x; });
        //}

        public static IEnumerable<T> RemoveElement<T>(this IEnumerable<T> values,
                                                      T removeValue)
        {
            CC.Validate.ArgumentNull(values, nameof(values));

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
            CC.Validate.ArgumentNull(values, nameof(values));

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
            CC.Validate.ArgumentNull(values, nameof(values));

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

        public static int SequenceToHashCode<T>(this IEnumerable<T> values)
        {
            CC.Validate.ArgumentNull(values, nameof(values));

            var hash = new HashCode();
            foreach ( var item in values)
                hash.Add(item);

            return hash.ToHashCode();
        }

        public static IEnumerable<KeyValuePair<TKey, TValue>> AssignKey<TKey, TValue>(
            this IEnumerable<TValue> values,
            Func<TValue, TKey> keySelector)
        {
            CC.Validate.ArgumentNull(values, nameof(values));
            CC.Validate.ArgumentNull(keySelector, nameof(keySelector));

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
            CC.Validate.ArgumentNull(value, nameof(value));

            if (value is IList<T> list)
                return new ReadOnlyCollection<T>(list);

            return new ReadOnlyCollection<T>(value.ToArray());
        }

        public static IEnumerable<TResult> DoTransformTypes<TResult>(this IEnumerable value)
        {
            CC.Validate.ArgumentNull(value, nameof(value));

            foreach (var item in value)
                yield return (TResult)TypeTransformer.DoTransform(item, typeof(TResult));
        }

        ///// <summary>
        ///// Only invokes action
        ///// </summary>
        ///// <typeparam name="T"></typeparam>
        ///// <param name="values"></param>
        ///// <param name="action"></param>
        ///// <returns></returns>
        //public static IEnumerable<T> Before<T>(this IEnumerable<T> values, Action action)
        //{
        //    CC.Validate.ArgumentNull(values, nameof(values));
        //    CC.Validate.ArgumentNull(action, nameof(action));

        //    action();

        //    return values;
        //}
        ///// <summary>
        ///// Only invokes action
        ///// </summary>
        ///// <typeparam name="T0"></typeparam>
        ///// <param name="values"></param>
        ///// <param name="action"></param>
        ///// <returns></returns>
        //public static IEnumerable<T0> Before<T0, T1>(this IEnumerable<T0> values,
        //                                             T1 arg,
        //                                             Action<T1> action)
        //{
        //    CC.Validate.ArgumentNull(values, nameof(values));
        //    CC.Validate.ArgumentNull(action, nameof(action));

        //    action(arg);

        //    return values;
        //}

        public static IEnumerable<T> Materialize<T>(this IEnumerable<T> values)
        {
            if (values is T[] array)
                return array;
            if (values is ICollection<T> collection)
                return collection;  

            return values.ToArray();
        }

        public static Queue<T> ToQueue<T>(this IEnumerable<T> values)
        {
            CC.Validate.ArgumentNull(values, nameof(values));

            var results = new Queue<T>();   
            foreach (var value in values)
                results.Enqueue(value);

            return results;
        }

        public static Stack<T> ToStack<T>(this IEnumerable<T> values)
        {
            CC.Validate.ArgumentNull(values, nameof(values));

            var results = new Stack<T>();
            foreach (var value in values)
                results.Push(value);

            return results;
        }
    }
}