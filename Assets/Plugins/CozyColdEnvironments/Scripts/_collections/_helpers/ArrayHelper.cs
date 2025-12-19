#nullable enable

using CommunityToolkit.Diagnostics;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace CCEnvs.Collections
{
    public static class ArrayHelper
    {
        /// <exception cref="ArgumentException"></exception>
        public static Array CastToElementType(Array array)
        {
            if (array.Length == 0)
                throw new ArgumentException("Array cannot be empty.");

            foreach (var item in array)
            {
                if (item is not null)
                {
                    Type elementType = item.GetType();
                    Type newArrayType = elementType.MakeArrayType();

                    if (array.GetType() == newArrayType)
                        return array;

                    Array result = Array.CreateInstance(elementType,
                                                        array.Length);

                    object valueToCast;
                    object castedValue;
                    for (int i = 0; i < array.Length; i++)
                    {
                        valueToCast = array.GetValue(i);

                        if (valueToCast is null)
                        {
                            result.SetValue(valueToCast, i);
                            continue;
                        }

                        castedValue = Convert.ChangeType(valueToCast, elementType);

                        result.SetValue(castedValue, i);
                    }

                    return result;
                }
            }

            throw new ArgumentException("Array must be contain any not null element.");
        }

        public static T[] Append<T>(this T[] source, T item)
        {
            Guard.IsNotNull(source);

            var dest = new T[source.Length + 1];
            source.CopyTo(dest, 0);
            dest[^1] = item;
            return dest;
        }

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

        public static ArraySegment<T> GetArraySegment<T>(this T[] source,
                                                         int count,
                                                         int offset = 0)
        {
            Guard.IsNotNull(source, nameof(source));

            return new ArraySegment<T>(source, offset, count);
        }

        public static IEnumerator<T> GetEnumeratorT<T>(this T[] values)
        {
            CC.Guard.IsNotNull(values, nameof(values));

            return ((IEnumerable<T>)values).GetEnumerator();
        }

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