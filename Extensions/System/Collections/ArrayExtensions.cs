#nullable enable

using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;
using UTIRLib.Collections;
using UTIRLib.Diagnostics;

namespace UTIRLib
{
    public static class ArrayExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int GetLengthOrZero<T>(this T[]? array) => array?.Length ?? 0;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static DefaultEnumerator<T> GetEnumeratorT<T>(this T[]? array)
        {
            return new DefaultEnumerator<T>(array ?? Array.Empty<T>());
        }

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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool HasNullOrEmptyString(this string[] strings)
        {
            return strings.Any(x => x.IsNullOrEmpty());
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool HasNullElement<T>(this T[] array) => array.Any(x => x.IsNull());

        /// <summary>
        /// Counts items in collection, but ignore null or default
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Quantity<T>(this T[] array) => array.Count(x => x.IsNotDefault());

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsEmpty<T>(this T[] array) => array.Length == 0;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsNotEmpty<T>(this T[] array) => !array.IsEmpty();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsNullOrEmpty<T>([NotNullWhen(false)] this T[]? array)
        {
            return array is null || array.IsEmpty();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsNotNullOrEmpty<T>([NotNullWhen(true)] this T[]? array)
        {
            return array is not null && array.IsNotEmpty();
        }
    }
}