using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;
using UTIRLib.Diagnostics;

#nullable enable

namespace UTIRLib
{
    public static class EnumerableExtensions
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
    }
}