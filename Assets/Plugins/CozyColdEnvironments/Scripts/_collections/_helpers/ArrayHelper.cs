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

        public static T[] AppendToArray<T>(this T[] source, T item)
        {
            CC.Guard.IsNotNullSource(source);

            var dest = new T[source.Length + 1];
            source.CopyTo(dest, 0);
            dest[^1] = item;

            return dest;
        }

        public static T[] PrependToArray<T>(this T[] source, T item)
        {
            CC.Guard.IsNotNullSource(source);

            var dest = new T[source.Length + 1];
            source.CopyTo(dest, 1);
            dest[0] = item;

            return dest;
        }

        public static T[] ConcatToArray<T>(this T[] source, T[] other)
        {
            CC.Guard.IsNotNullSource(source);
            CC.Guard.IsNotNullSource(source);

            var arr = new T[source.Length + other.Length];

            source.CopyTo(arr, 0);
            other.CopyTo(arr, source.Length - 1);

            return arr;
        }

        public static void Fill<T>(this T[] source, T item)
        {
            Array.Fill(source, item);
        }

        public static T Find<T>(this T[] source, Predicate<T> matchPredicate)
        {
            return Array.Find(source, matchPredicate);
        }

        public static T? Find<T>(this T[] source, T item)
        {
            int index = Array.IndexOf(source, item);

            return index > -1 ? source[index] : default;
        }

        public static bool Contains<T>(this T[] source, T item)
        {
            var idx = Array.IndexOf(source, item);
            return idx > -1;
        }

        public static int IndexOf<T>(this T[] source, T item)
        {
            return Array.IndexOf(source, item); 
        }

        public static int LastIndexOf<T>(this T[] source, T item)
        {
            return Array.LastIndexOf(source, item);
        }

        public static int FindIndex<T>(this T[] source, Predicate<T> match)
        {
            return Array.FindIndex(source, match);
        }

        public static ArraySegment<T> GetArraySegment<T>(this T[] source,
                                                         int count,
                                                         int offset = 0)
        {
            Guard.IsNotNull(source, nameof(source));

            return new ArraySegment<T>(source, offset, count);
        }

        public static bool TryFindHole<T>(this T?[] array, [NotNullWhen(true)] out int? holeIdx)
        {
            Guard.IsNotNull(array, nameof(array));

            int arrLength = array.Length;
            var defaultEqComparer = EqualityComparer<T?>.Default;

            for (int i = 0; i < arrLength; i++)
            {
                if (defaultEqComparer.Equals(array[i], default))
                {
                    holeIdx = i;
                    return true;
                }
            }

            holeIdx = null;
            return false;
        }

        public static bool TryAddToArray<T>(this T?[] array, T item, [NotNullWhen(true)] out int? addedIdx)
        {
            Guard.IsNotNull(array, nameof(array));
            CC.Guard.IsNotNull(item, nameof(item));

            if (!array.TryFindHole(out var holeIdx))
            {
                addedIdx = null;
                return false;
            }

            array[holeIdx.Value] = item;
            addedIdx = holeIdx;
            return true;
        }

        public static int AddToArray<T>(this T?[] array, T item)
        {
            if (!array.TryAddToArray(item, out var addedIdx))
                throw new InvalidOperationException("Array is full");

            return addedIdx.Value;
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