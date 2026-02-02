using CCEnvs.Collections.Unsafe;
using CCEnvs.FuncLanguage;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

#nullable enable

namespace CCEnvs.Collections
{
    public static class ListHelper
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ReadOnlySpan<T> ToReadOnlySpan<T>(this List<T> source)
        {
            CC.Guard.IsNotNull(source, nameof(source));

            return new ReadOnlySpan<T>(source.GetInternalArrayUnsafe(), 0, source.Count);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Span<T> ToSpan<T>(this List<T> source)
        {
            CC.Guard.IsNotNull(source, nameof(source));

            return new Span<T>(source.GetInternalArrayUnsafe(), 0, source.Count);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ReadOnlyMemory<T> ToReadOnlyMemory<T>(this List<T> source)
        {
            CC.Guard.IsNotNull(source, nameof(source));

            return new ReadOnlyMemory<T>(source.GetInternalArrayUnsafe(), 0, source.Count);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Memory<T> ToMemory<T>(this List<T> source)
        {
            CC.Guard.IsNotNull(source, nameof(source));

            return new Memory<T>(source.GetInternalArrayUnsafe(), 0, source.Count);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Maybe<T> GetMaybeValue<T>(this IList<T> source, int index)
        {
            CC.Guard.IsNotNullSource(source);

            if (source.Count >= index || index < 0)
                return Maybe<T>.None;

            return source[index];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Maybe<T> GetMaybeValueReadOnly<T>(this IReadOnlyList<T> source, int index)
        {
            CC.Guard.IsNotNullSource(source);

            if (source.Count >= index || index < 0)
                return Maybe<T>.None;

            return source[index];
        }
    }
}