using CCEnvs.Diagnostics;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

#nullable enable

namespace CCEnvs.Linq
{
    public static class EnumerableQueries
    {
        public static IEnumerable<IndexValuePair<T>> Index<T>(this IEnumerable<T> values)
        {
            CC.Validate.ArgumentNull(values, nameof(values));

            int i = 0;
            foreach (var value in values)
                yield return new IndexValuePair<T>(i++, value);
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

        public static IEnumerable<TResult> ChangeTypes<TResult>(this IEnumerable value)
        {
            CC.Validate.ArgumentNull(value, nameof(value));

            foreach (var item in value)
                yield return (TResult)CCConvert.ChangeType(item, typeof(TResult));
        }
    }
}