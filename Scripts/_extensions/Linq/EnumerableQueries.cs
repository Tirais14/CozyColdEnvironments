using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

#nullable enable

namespace CCEnvs.Linq
{
    public static class EnumerableQueries
    {
        public static T[] ToArrayOrEmpty<T>(this IEnumerable<T>? collection)
        {
            return collection?.ToArray() ?? Array.Empty<T>();
        }

        public static ReadOnlyCollection<T> ToReadOnlyCollection<T>(this IEnumerable<T> value)
        {
            CC.Validate.ArgumentNull(value, nameof(value));

            return new ReadOnlyCollection<T>(value.ToArray());
        }
    }
}