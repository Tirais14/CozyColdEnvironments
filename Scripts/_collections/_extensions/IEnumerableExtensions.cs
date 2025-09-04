using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;
using CCEnvs.Diagnostics;

#nullable enable

namespace CCEnvs.Collections
{
    public static class IEnumerableExtensions
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
    }
}