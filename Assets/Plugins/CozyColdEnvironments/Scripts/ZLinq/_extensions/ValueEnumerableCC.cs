#if Z_LINQ
using CCEnvs.Collections;
using CCEnvs.Collections.Unsafe;
using System;
using System.Buffers;
using System.Collections;
using System.Collections.Generic;
using ZLinq;
using ZLinq.Linq;

namespace CCEnvs
{
    public static class ValueEnumerableCC
    {
        public static T[] ForEach<TEnumerator, T>(this ValueEnumerable<TEnumerator, T> source,
            Action<T> action)
            where TEnumerator : struct, IValueEnumerator<T>
        {
            CC.Guard.IsNotNull(action, nameof(action));

            var list = new List<T>();
            foreach (var item in source)
            {
                action(item);
                list.Add(item);
            }

            return list.ToArray();
        }

        /// <summary>
        /// Shortcut to <see cref="ZLinq.ValueEnumerable.AsValueEnumerable(IEnumerable)"/>
        /// </summary>
        public static ValueEnumerable<FromNonGenericEnumerable<object>, object> ZL(this IEnumerable source)
        {
            return source.AsValueEnumerable();
        }

        /// <inheritdoc cref="ZL(IEnumerable)"/>
        public static ValueEnumerable<FromNonGenericEnumerable<T>, T> ZL<T>(this IEnumerable source)
        {
            return source.AsValueEnumerable<T>();
        }

        /// <inheritdoc cref="ZL(IEnumerable)"/>
        public static ValueEnumerable<FromEnumerable<T>, T> ZL<T>(this IEnumerable<T> source)
        {
            return source.AsValueEnumerable();
        }

        /// <inheritdoc cref="ZL(IEnumerable)"/>
        public static ValueEnumerable<FromArray<T>, T> ZL<T>(this T[] source)
        {
            return source.AsValueEnumerable();
        }

        /// <inheritdoc cref="ZL(IEnumerable)"/>
        public static ValueEnumerable<FromList<T>, T> ZL<T>(this List<T> source)
        {
            return source.AsValueEnumerable();
        }

        /// <inheritdoc cref="ZL(IEnumerable)"/>
        public static ValueEnumerable<FromMemory<T>, T> ZL<T>(this ArraySegment<T> source)
        {
            return source.AsValueEnumerable();
        }

        /// <inheritdoc cref="ZL(IEnumerable)"/>
        public static ValueEnumerable<FromMemory<T>, T> ZL<T>(this Memory<T> source)
        {
            return source.AsValueEnumerable();
        }

        /// <inheritdoc cref="ZL(IEnumerable)"/>
        public static ValueEnumerable<FromMemory<T>, T> ZL<T>(this ReadOnlyMemory<T> source)
        {
            return source.AsValueEnumerable();
        }

        /// <inheritdoc cref="ZL(IEnumerable)"/>
        public static ValueEnumerable<FromReadOnlySequence<T>, T> ZL<T>(this ReadOnlySequence<T> source)
        {
            return source.AsValueEnumerable();
        }

        // for System.Collections.Generic

        /// <inheritdoc cref="ZL(IEnumerable)"/>
        public static ValueEnumerable<FromDictionary<TKey, TValue>, KeyValuePair<TKey, TValue>> ZL<TKey, TValue>(this Dictionary<TKey, TValue> source)
            where TKey : notnull
        {
            return source.AsValueEnumerable();
        }

        /// <inheritdoc cref="ZL(IEnumerable)"/>
        public static ValueEnumerable<FromSortedDictionary<TKey, TValue>, KeyValuePair<TKey, TValue>> ZL<TKey, TValue>(this SortedDictionary<TKey, TValue> source)
            where TKey : notnull
        {
            return source.AsValueEnumerable();
        }

        /// <inheritdoc cref="ZL(IEnumerable)"/>
        public static ValueEnumerable<FromQueue<T>, T> ZL<T>(this Queue<T> source)
        {
            return source.AsValueEnumerable();
        }

        /// <inheritdoc cref="ZL(IEnumerable)"/>
        public static ValueEnumerable<FromStack<T>, T> ZL<T>(this Stack<T> source)
        {
            return source.AsValueEnumerable();
        }

        /// <inheritdoc cref="ZL(IEnumerable)"/>
        public static ValueEnumerable<FromLinkedList<T>, T> ZL<T>(this LinkedList<T> source)
        {
            return source.AsValueEnumerable();
        }

        /// <inheritdoc cref="ZL(IEnumerable)"/>
        public static ValueEnumerable<FromHashSet<T>, T> ZLinq<T>(this HashSet<T> source)
        {
            return source.AsValueEnumerable();
        }

        /// <inheritdoc cref="ZL(IEnumerable)"/>
        public static ValueEnumerable<FromSortedSet<T>, T> ZL<T>(this SortedSet<T> source)
        {
            return source.AsValueEnumerable();
        }

#if NET8_0_OR_GREATER

        /// <inheritdoc cref="ZL(IEnumerable)"/>
        public static ValueEnumerable<FromImmutableArray<T>, T> ZL<T>(this ImmutableArray<T> source)
        {
            return source.AsValueEnumerable();
        }

        /// <inheritdoc cref="ZL(IEnumerable)"/>
        public static ValueEnumerable<FromImmutableHashSet<T>, T> ZL<T>(this ImmutableHashSet<T> source)
        {
            return source.AsValueEnumerable();
        }

#endif

#if NET9_0_OR_GREATER
        
        /// <inheritdoc cref="ZL(IEnumerable)"/>
        public static ValueEnumerable<FromSpan<T>, T> ZL<T>(this Span<T> source)
        {
            return source.AsValueEnumerable();
        }

        /// <inheritdoc cref="ZL(IEnumerable)"/>
        public static ValueEnumerable<FromSpan<T>, T> ZL<T>(this ReadOnlySpan<T> source)
        {
            return source.AsValueEnumerable();
        }

#endif

    }
}
#endif //Z_LINQ
