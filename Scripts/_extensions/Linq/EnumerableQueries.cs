using System;
using System.Collections.Generic;
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
    }
}