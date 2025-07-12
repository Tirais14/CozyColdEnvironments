#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

namespace UTIRLib.Linq
{
    public static class StringQueries
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IEnumerable<string> Where(
            this IEnumerable<string> strings,
            string toContains,
            StringComparison stringComparison = StringComparison.InvariantCulture)
        {
            return strings.Where(x => x.Contains(toContains, stringComparison));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IEnumerable<string> WhereByRegex(this IEnumerable<string> strings, string pattern)
        {
            return strings.Where(x => Regex.IsMatch(x, pattern));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IEnumerable<string> Except(
            this IEnumerable<string> strings,
            string toContains,
            StringComparison stringComparison = StringComparison.InvariantCulture)
        {
            return strings.Where(x => !x.Contains(toContains, stringComparison));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IEnumerable<string> ExceptByRegex(this IEnumerable<string> strings, string pattern)
        {
            return strings.Where(x => !Regex.IsMatch(x, pattern));
        }
    }
}
