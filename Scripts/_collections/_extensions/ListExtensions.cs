using CCEnvs.Reflection;
using System;
using System.Collections.Generic;

#nullable enable

namespace CCEnvs.Collections
{
    public static class ListExtensions
    {
        public static T[] ToArrayOrEmpty<T>(this List<T>? list)
        {
            return list?.ToArray() ?? Array.Empty<T>();
        }
    }
}

namespace CCEnvs.Collections.Performance
{
    public static class ListExtensions
    {
        public static ReadOnlySpan<T> ToReadOnlySpan<T>(this List<T> list)
        {
            return new ReadOnlySpan<T>(list.GetInternalArray());
        }

        public static Span<T> ToSpan<T>(this List<T> list)
        {
            return new Span<T>(list.GetInternalArray());
        }
    }
}