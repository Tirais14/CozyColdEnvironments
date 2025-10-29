using System;
using System.Collections.Generic;

#nullable enable
namespace CCEnvs.Linq
{
    public static class IndexedIEnumerableQueries
    {
        public static IEnumerable<T> Unindex<T>(this IEnumerable<(int index, T value)> values)
        {
            CC.Guard.IsNotNull(values, nameof(values));

            foreach (var (_, value) in values)
                yield return value;
        }
        public static IEnumerable<(int index, T value)> InsertElementAtBy<T>(
            this IEnumerable<(int index, T value)> values,
            int index,
            T newValue)
        {
            CC.Guard.IsNotNull(values, nameof(values));

            bool inserted = false;
            int i = 0;
            foreach (var indexed in values)
            {
                if (i == index)
                    inserted = true;

                if (inserted)
                {
                    yield return (i++, newValue);
                }
                else
                {
                    i = indexed.index;
                    yield return indexed;
                }
            }

#pragma warning disable S112
            if (!inserted)
                throw new IndexOutOfRangeException($"Index = {i}");
#pragma warning restore S112
        }

        public static IEnumerable<(int index, T value)> RemoveElementAtBy<T>(
            this IEnumerable<(int index, T value)> values,
            int index,
            T removedValue)
        {
            CC.Guard.IsNotNull(values, nameof(values));

            bool removed = false;
            int i = 0;
            foreach (var indexed in values)
            {
                if (i == index)
                {
                    removed = true;
                    continue;
                }

                if (removed)
                {
                    yield return (i++, removedValue);
                }
                else
                {
                    i = indexed.index;
                    yield return indexed;
                }
            }

#pragma warning disable S112
            if (!removed)
                throw new IndexOutOfRangeException($"Index = {i}");
#pragma warning restore S112
        }
    }
}
