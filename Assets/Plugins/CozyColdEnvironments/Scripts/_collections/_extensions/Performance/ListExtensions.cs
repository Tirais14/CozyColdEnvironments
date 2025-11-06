using CCEnvs.Collections.Unsafe;
using System;
using System.Collections.Generic;

#nullable enable
namespace CCEnvs.Collections.Performance
{
    public static class ListExtensions
    {
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

        /// <summary>
        /// For less memory allocation.
        /// Use this only if you are sure that the source list will not be used anywhere else.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static ArraySegment<T> GetInternalArraySegment<T>(this List<T> source)
        {
            CC.Guard.SourceArg(source);

            return new ArraySegment<T>(source.GetInternalArrayUnsafe(), 0, source.Count);
        }
    }
}
