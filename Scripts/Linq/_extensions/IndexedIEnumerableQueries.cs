using CCEnvs.Diagnostics;
using System;
using System.Collections.Generic;
using System.Linq;

#nullable enable
namespace CCEnvs.Linq
{
    public static class IndexedIEnumerableQueries
    {
        public static IEnumerable<T> Unindex<T>(this IEnumerable<IndexValuePair<T>> values)
        {
            CC.Validate.ArgumentNull(values, nameof(values));

            foreach (var pair in values)
                yield return pair.value;
        }

        public static IEnumerable<IndexValuePair<T>> Insert<T>(
            this IEnumerable<IndexValuePair<T>> values,
            int position,
            T newValue)
        {
            CC.Validate.ArgumentNull(values, nameof(values));

            bool inserted = false;
            int i = 0;
            foreach (var indexed in values)
            {
                if (indexed.index == position)
                {
                    inserted = true;
                    yield return new IndexValuePair<T>(i++, newValue);
                }
                else if (inserted)
                    yield return new IndexValuePair<T>(i, indexed.value);
                else
                {
                    i = indexed.index;
                    yield return indexed;
                }
            }

            if (!inserted)
                throw new IndexOutOfRangeException($"Index = {i}");
        }

        public static IEnumerable<IndexValuePair<T>> RemoveAt<T>(
            this IEnumerable<IndexValuePair<T>> values,
            int position)
        {
            CC.Validate.ArgumentNull(values, nameof(values));

            bool removed = false;
            int i = 0;
            foreach (var indexed in values)
            {
                if (indexed.index == position)
                {
                    removed = true;
                    i--;
                }
                else if (removed)
                    yield return new IndexValuePair<T>(i, indexed.value);
                else
                {
                    i = indexed.index;
                    yield return indexed;
                }
            }
        }

        public static T GetValueAt<T>(this IEnumerable<IndexValuePair<T>> values, int index)
        {
            CC.Validate.ArgumentNull(values, nameof(values));

            var found = values.FirstOrDefault(x => x.index == index);

            if (found.IsDefault())
                CC.Throw.IndexOutOfRange(index);

            return found.value;
        }

        public static int IndexOf<T>(this IEnumerable<IndexValuePair<T>> values, T value)
        {
            IndexValuePair<T> found = values.FirstOrDefault(x => x.Equals(value!));

            if (found.IsDefault())
                return -1;

            return found.index;
        }
    }
}
