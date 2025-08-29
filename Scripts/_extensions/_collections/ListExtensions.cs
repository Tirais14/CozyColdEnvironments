using System;
using System.Collections.Generic;
using CozyColdEnvironments.Reflection;

#nullable enable
namespace CozyColdEnvironments
{
    public static class ListExtensions
    {
        public static T[] ToArrayOrEmpty<T>(this List<T>? list)
        {
            return list?.ToArray() ?? Array.Empty<T>();
        }
    }
}

namespace CozyColdEnvironments.Reflection
{
    public static class ListExtensions
    {
        public static T[] GetInternalArray<T>(this List<T> list)
        {
            return ListCache<T>.GetInternalArray(list);
        }
    }
}

namespace CozyColdEnvironments.Performance
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

