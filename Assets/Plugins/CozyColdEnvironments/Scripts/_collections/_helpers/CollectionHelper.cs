using CCEnvs.Pools;
using CommunityToolkit.Diagnostics;
using System.Buffers;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

#nullable enable

namespace CCEnvs.Collections
{
    public static class CollectionHelper
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryAdd<T>(this ICollection<T> source, T value)
        {
            CC.Guard.IsNotNullSource(source);
            CC.Guard.IsNotNull(value, nameof(value));

            if (source.Contains(value))
                return false;

            source.Add(value);
            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void AddRange<T>(this ICollection<T> collection, IEnumerable<T> range)
        {
            CC.Guard.IsNotNull(collection, nameof(collection));
            Guard.IsNotNull(range, nameof(range));

            foreach (var item in range)
                collection.Add(item);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void AddRange<T>(this ICollection<T> collection, params T[] range)
        {
            CC.Guard.IsNotNull(collection, nameof(collection));
            Guard.IsNotNull(range, nameof(range));

            int rangeLength = range.Length;
            for (int i = 0; i < rangeLength; i++)
                collection.Add(range[i]);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void RemoveRange<T>(this ICollection<T> collection, IEnumerable<T> range)
        {
            CC.Guard.IsNotNull(collection, nameof(collection));
            Guard.IsNotNull(range, nameof(range));

            foreach (var item in range)
                collection.Remove(item);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void RemoveRange<T>(this ICollection<T> collection, params T[] range)
        {
            CC.Guard.IsNotNull(collection, nameof(collection));
            Guard.IsNotNull(range, nameof(range));

            int rangeLength = range.Length;

            for (int i = 0; i < rangeLength; i++)
                collection.Remove(range[i]);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static PooledArray<T> ToArrayPooledReadOnly<T>(this IReadOnlyCollection<T> source)
        {
            CC.Guard.IsNotNullSource(source);

            var items = ArrayPool<T>.Shared.Get(source.Count);
            int i = 0;

            foreach (var item in source)
                items[i++] = item;

            return items;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static PooledArray<T> ToArrayPooled<T>(this ICollection<T> source)
        {
            CC.Guard.IsNotNullSource(source);

            var items = ArrayPool<T>.Shared.Get(source.Count);
            int i = 0;

            foreach (var item in source)
                items[i++] = item;

            return items;
        }
    }
}