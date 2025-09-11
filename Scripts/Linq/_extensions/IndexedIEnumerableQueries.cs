using QuikGraph.Algorithms.ShortestPath;
using System;
using System.Collections.Generic;

#nullable enable
namespace CCEnvs.Linq
{
    public static class IndexedIEnumerableQueries
    {
        public static IEnumerable<T> Unindex<T>(this IEnumerable<(int index, T value)> values)
        {
            CC.Validate.ArgumentNull(values, nameof(values));

            foreach (var (_, value) in values)
                yield return value;
        }

        public static IEnumerable<T> Insert<T>(
            this IEnumerable<T> values,
            int position,
            T newValue)
        {
            CC.Validate.ArgumentNull(values, nameof(values));

            bool inserted = false;
            int i = 0;
            foreach (var value in values)
            {
                i++;
                if (i == position)
                {
                    inserted = true;
                    yield return newValue;
                }
                else
                    yield return value;
            }

#pragma warning disable S112
            if (!inserted)
                throw new IndexOutOfRangeException($"Index = {i}");
#pragma warning restore S112
        }
        public static IEnumerable<(int index, T value)> InsertBy<T>(
            this IEnumerable<(int index, T value)> values,
            int index,
            T newValue)
        {
            CC.Validate.ArgumentNull(values, nameof(values));

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

        public static IEnumerable<T> Remove<T>(this IEnumerable<T> values, T removeValue)
        {
            CC.Validate.ArgumentNull(values, nameof(values));

            foreach (var value in values)
            {
                if (Equals(value, removeValue))
                    continue;

                yield return value;
            }
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
                i++;
                if (i == position)
                    removed = true;
                else
                    yield return value;
            }

#pragma warning disable S112
            if (!removed)
                throw new IndexOutOfRangeException($"Index = {i}");
#pragma warning restore S112
        }
        public static IEnumerable<(int index, T value)> RemoveAtBy<T>(
            this IEnumerable<(int index, T value)> values,
            int index,
            T removedValue)
        {
            CC.Validate.ArgumentNull(values, nameof(values));

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
