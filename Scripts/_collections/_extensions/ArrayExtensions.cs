#nullable enable

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

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
    }
}