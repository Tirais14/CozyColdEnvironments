#nullable enable

using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;
using CCEnvs.Diagnostics;

namespace CCEnvs.Collections
{
    public static class ArrayExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int GetLengthOrZero<T>(this T[]? array) => array?.Length ?? 0;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Fill<T>(this T[] array, T value) => Array.Fill(array, value);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T Find<T>(this T[] array, Predicate<T> matchPredicate) => Array.Find(array, matchPredicate);

        public static T? Find<T>(this T[] array, T value)
        {
            int index = Array.IndexOf(array, value);

            return index > -1 ? array[index] : default;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ForEach<T>(this T[] array, Action<T> action) => Array.ForEach(array, action);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T[] ForEachQ<T>(this T[] array, Action<T> action)
        {
            Array.ForEach(array, action);

            return array;
        }
    }
}