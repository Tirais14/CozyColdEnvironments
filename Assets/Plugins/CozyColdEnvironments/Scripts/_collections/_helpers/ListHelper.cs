using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using CCEnvs.Collections.Unsafe;
using CCEnvs.FuncLanguage;

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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryRemoveAt<T>(
            this IList<T> source,
            int index,
            [NotNullWhen(true)] out T? removed
            )
        {
            CC.Guard.IsNotNullSource(source);

            if (index >= source.Count
                ||
                source.IsReadOnly)
            {
                removed = default;
                return false;
            }

            removed = source[index]!;
            source.RemoveAt(index);

            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool RemoveLast<T>(this IList<T> source)
        {
            CC.Guard.IsNotNullSource(source);

            if (source.IsReadOnly || source.IsEmpty())
                return false;

            source.RemoveAt(source.Count - 1);
            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool RemoveLast<T>(
            this IList<T> source,
            [NotNullWhen(true)] out T? removed
            )
        {
            CC.Guard.IsNotNullSource(source);

            if (source.IsReadOnly || source.IsEmpty())
            {
                removed = default;
                return false;
            }

            removed = source[^1]!;
            source.RemoveAt(source.Count - 1);

            return true;
        }
    }
}