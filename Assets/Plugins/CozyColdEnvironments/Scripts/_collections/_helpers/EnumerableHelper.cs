using CCEnvs.Diagnostics;
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
        public static bool IsEmpty<T>(this IEnumerable<T> enumerable) => !enumerable.Any();

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

        public static void DisposeEach<T>(this IEnumerable<T> disposables)
            where T : IDisposable
        {
            CC.Guard.IsNotNull(disposables, nameof(disposables));

            foreach (var item in disposables)
                item.Dispose();
        }
    }
}