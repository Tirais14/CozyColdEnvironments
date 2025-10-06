using CCEnvs.Reflection;
using System;
using System.Linq;
using System.Collections.Generic;

#nullable enable

namespace CCEnvs.Collections
{
    public static class IListExtensions
    {
        public static T[] ToArrayOrEmpty<T>(this IList<T>? list)
        {
            return list?.ToArray() ?? Array.Empty<T>();
        }
    }
}

namespace CCEnvs.Collections.Performance
{
    public static class IListExtensions
    {
        public static ReadOnlySpan<T> ToReadOnlySpan<T>(this IList<T> list)
        {
            return new ReadOnlySpan<T>(list.GetInternalArray());
        }

        public static Span<T> ToSpan<T>(this IList<T> list)
        {
            return new Span<T>(list.GetInternalArray());
        }
    }
}