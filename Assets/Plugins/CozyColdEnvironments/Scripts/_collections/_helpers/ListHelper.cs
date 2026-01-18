using CCEnvs.Collections.Unsafe;
using CommunityToolkit.Diagnostics;
using System;
using System.Collections.Generic;
using System.Linq;

#nullable enable

namespace CCEnvs.Collections
{
    public static class ListHelper
    {
        public static T[] ToArrayOrEmpty<T>(this IList<T>? list)
        {
            return list?.ToArray() ?? Array.Empty<T>();
        }

        public static ReadOnlySpan<T> ToReadOnlySpan<T>(this List<T> source)
        {
            CC.Guard.IsNotNull(source, nameof(source));

            return new ReadOnlySpan<T>(source.GetInternalArrayUnsafe(), 0, source.Count);
        }

        public static Span<T> ToSpan<T>(this List<T> source)
        {
            CC.Guard.IsNotNull(source, nameof(source));

            return new Span<T>(source.GetInternalArrayUnsafe(), 0, source.Count);
        }

        public static ReadOnlyMemory<T> ToReadOnlyMemory<T>(this List<T> source)
        {
            CC.Guard.IsNotNull(source, nameof(source));

            return new ReadOnlyMemory<T>(source.GetInternalArrayUnsafe(), 0, source.Count);
        }

        public static Memory<T> ToMemory<T>(this List<T> source)
        {
            CC.Guard.IsNotNull(source, nameof(source));

            return new Memory<T>(source.GetInternalArrayUnsafe(), 0, source.Count);
        }
    }
}