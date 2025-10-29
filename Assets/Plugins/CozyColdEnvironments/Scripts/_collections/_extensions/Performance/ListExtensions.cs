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

            return new ReadOnlySpan<T>(source.GetInternalArray(), 0, source.Count);
        }

        public static Span<T> ToSpan<T>(this List<T> source)
        {
            CC.Guard.IsNotNull(source, nameof(source));

            return new Span<T>(source.GetInternalArray(), 0, source.Count);
        }

        public static ReadOnlyMemory<T> ToReadOnlyMemory<T>(this List<T> source)
        {
            CC.Guard.IsNotNull(source, nameof(source));

            return new ReadOnlyMemory<T>(source.GetInternalArray(), 0, source.Count);
        }

        public static Memory<T> ToMemory<T>(this List<T> source)
        {
            CC.Guard.IsNotNull(source, nameof(source));

            return new Memory<T>(source.GetInternalArray(), 0, source.Count);
        }

        public static ArraySegment<T> GetInternalArraySegment<T>(this List<T> source)
        {
            CC.Guard.SourceArg(source);

            return new ArraySegment<T>(source.GetInternalArray(), 0, source.Count);
        }
    }
}
