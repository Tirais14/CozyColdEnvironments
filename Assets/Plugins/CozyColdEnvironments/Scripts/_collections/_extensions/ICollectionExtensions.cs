using CCEnvs.Diagnostics;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

#nullable enable

namespace CCEnvs.Collections
{
    public static class ICollectionExtensions
    {
        public static bool TryAdd<T>(this ICollection<T> source, T value)
        {
            if (source.Contains(value))
                return false;

            source.Add(value);
            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int GetCountOrZero(this ICollection? collection)
        {
            return collection?.Count ?? 0;
        }

        public static void AddRange<T>(this ICollection<T> collection, IEnumerable<T> range)
        {
            foreach (var item in range)
                collection.Add(item);
        }

        public static void AddRange<T>(this ICollection<T> collection, params T[] range)
        {
            int rangeLength = range.Length;
            for (int i = 0; i < rangeLength; i++)
                collection.Add(range[i]);
        }

        public static void RemoveRange<T>(this ICollection<T> collection, IEnumerable<T> range)
        {
            foreach (var item in range)
                collection.Remove(item);
        }

        public static void RemoveRange<T>(this ICollection<T> collection, T[] range)
        {
            int rangeLength = range.Length;
            for (int i = 0; i < rangeLength; i++)
                collection.Remove(range[i]);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsNullOrEmpty([NotNullWhen(false)] this ICollection? collection)
        {
            return collection.IsNull() || collection.Count == 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsNotNullOrEmpty([NotNullWhen(true)] this ICollection? collection)
        {
            return collection.IsNotNull() && collection.Count >= 0;
        }
    }
}