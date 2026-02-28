using CCEnvs.Linq;
using CCEnvs.Pools;
using SuperLinq;
using System;
using System.Buffers;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;

#nullable enable

namespace CCEnvs.Collections
{
    public static class EnumerableHelper
    {
        public static int CountNotNull<T>(this IEnumerable<T> values)
            where T : class
        {
            return values.Count(x => x.IsNotNull());
        }

        public static int CountNotDefault<T>(this IEnumerable<T> values)
        {
            return values.Count(x => x.IsNotDefault());
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Contains<T>(this IEnumerable<T> values, IEnumerable<T> toCheckValues)
        {
            return values.All(a => toCheckValues.Any(b => b!.Equals(a)));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Contains<T>(this IEnumerable<T> values, params T[] toCheckValues)
        {
            return values.Contains((IEnumerable<T>)toCheckValues);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsNullOrEmpty<T>([NotNullWhen(false)] this IEnumerable<T>? enumerable)
        {
            return enumerable.IsNull() || enumerable.IsEmpty();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsNotNullOrEmpty<T>([NotNullWhen(true)] this IEnumerable<T>? enumerable)
        {
            return !enumerable.IsNullOrEmpty();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsEmpty<T>(this IEnumerable<T> enumerable)
        {
            if (enumerable.TryGetNonEnumeratedCount(out var count))
                return count < 1;

            foreach (var _ in enumerable)
                return false;

            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsNotEmpty<T>(this IEnumerable<T> enumerable) => !enumerable.IsEmpty();

        public static IEnumerable<int> RandomizedRange(int count, int start = 0)
        {
            var range = Enumerable.Range(start, count).ToArray();

            var r = new System.Random();

            for (int i = range.Length - 1; i > 0; i--)
            {
                int j = r.Next(0, i);
                (range[i], range[j]) = (range[j], range[i]);
            }

            return range;
        }

        public static PooledArray<T> EnumerableToArrayPooled<T>(this IEnumerable<T>? source, int sourceCount)
        {
            if (source.IsNullOrEmpty() || sourceCount < 1)
                return default;

            var arrHandle = ArrayPool<T>.Shared.RentHandled(sourceCount);
            source.CopyTo(arrHandle.Value, 0);

            return new PooledArray<T>(arrHandle, sourceCount, offset: 0);
        }

        public static PooledArray<T> EnumerableToArrayPooled<T>(this IEnumerable<T>? source)
        {
            if (source.IsNullOrEmpty())
                return default;

            if (source.TryGetNonEnumeratedCount(out var count))
                return source.EnumerableToArrayPooled(count);

            PooledObject<T[]> items = ArrayPool<T>.Shared.RentHandled(16);
            int itemCount = 0;

            foreach (var item in source)
            {
                itemCount++;

                if (items.Value.Length == itemCount)
                {
                    var tItems = items;

                    items = ArrayPool<T>.Shared.RentHandled((int)(tItems.Value.Length * 1.5));

                    tItems.Value.CopyTo(items.Value, 0);
                    tItems.Dispose();
                }

                items.Value[itemCount - 1] = item;
            }

            return new PooledArray<T>(items, itemCount);
        }

        public static bool EqualsByElements<T>(this IEnumerable<T>? source, IEnumerable<T>? other)
        {
            return EqualityComparer<IEnumerable<T>?>.Default.Equals(source, other)
                   &&
                   (source == null
                   ||
                   source.SequenceEqual(other));
        }

        public static bool IsMutableCollection<T>(this IEnumerable<T> source)
        {
            CC.Guard.IsNotNullSource(source);

            if (source is ICollection<T> collection && !collection.IsReadOnly)
                return true;

            if (source is IDictionary dic && !dic.IsReadOnly)
                return true;

            return false;
        }

        public static string ElementsToString<T>(this IEnumerable<T> source)
        {
            CC.Guard.IsNotNullSource(source);

            if (source.IsEmpty())
                return StringHelper.EMPTY_ARRAY;

            using var sb = StringBuilderPool.Shared.Get();

            sb.Value.Append($"[{Environment.NewLine}");
            sb.Value.AppendJoin($",{Environment.NewLine}", source);
            sb.Value.Append($"{Environment.NewLine}]");

            return sb.Value.ToString();
        }
    }
}