using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

#nullable enable

namespace UTIRLib.Linq
{
    public static class EnumerableQueries
    {
        public static T[] ToArrayOrEmpty<T>(this IEnumerable<T>? collection)
        {
            return collection?.ToArray() ?? Array.Empty<T>();
        }

        public static IEnumerable<T> ConcatToBegin<T>(this IEnumerable<T> source, IEnumerable<T> toAdd)
        {
            if (source is null)
                throw new ArgumentNullException(nameof(source));
            if (toAdd is null)
                throw new ArgumentNullException(nameof(toAdd));

            List<T> result = new();

            result.AddRange(toAdd);
            result.AddRange(source);

            return result;
        }

        public static IEnumerable<T> Except<T>(this IEnumerable<T> enumerable, T item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));

            return enumerable.Except(new T[] { item });
        }
    }
}