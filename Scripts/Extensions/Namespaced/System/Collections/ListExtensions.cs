using System;
using System.Collections.Generic;
using UTIRLib.Reflection;

#nullable enable
namespace UTIRLib.Reflection
{
    public static class ListExtensions
    {
        public static T[] GetInternalArray<T>(this List<T> list)
        {
            return ListCache<T>.GetInternalArray(list);
        }
    }
}

namespace UTIRLib.Performance
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
