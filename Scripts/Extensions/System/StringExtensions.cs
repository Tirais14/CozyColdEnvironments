using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;

#nullable enable
namespace UTIRLib
{
    public static class StringExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string JoinStrings(this IEnumerable<string> strings,
                                          char joinSeparator,
                                          StringJoinOptions options = StringJoinOptions.None)
        {
            if (options.HasFlag(StringJoinOptions.SkipEmpty))
                return JoinToStringSkipEmpty(strings, joinSeparator);

            return string.Join(joinSeparator, strings);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string JoinStrings(this IEnumerable<string> strings,
                                          string joinSeparator,
                                          StringJoinOptions options = StringJoinOptions.None)
        {
            if (options.HasFlag(StringJoinOptions.SkipEmpty))
                return JoinToStringSkipEmpty(strings, joinSeparator);

            return string.Join(joinSeparator, strings);
        }

        public static string JoinStringsByLine(this IEnumerable<string> strings,
                                         StringJoinOptions options = StringJoinOptions.None)
        {
            return strings.JoinStrings(Environment.NewLine, options);
        }

        public static bool ContainsAny(this string str, char[] chars)
        {
            for (int i = 0; i < chars.Length; i++)
            {
                if (str.Contains(chars[i]))
                    return true;
            }

            return false;
        }

        public static string Replace(this string str, IEnumerable<char> oldChars, char newChar)
        {
            StringBuilder stringBuilder = new(str);

            foreach (var oldChar in oldChars)
                stringBuilder.Replace(oldChar, newChar);

            return stringBuilder.ToString();
        }

        public static string Replace(this string str, IEnumerable<string> oldStrings, string newString)
        {
            StringBuilder stringBuilder = new(str);

            foreach (var oldString in oldStrings)
                stringBuilder.Replace(oldString, newString);

            return stringBuilder.ToString();
        }

        #region Delete
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string DeleteWhitespaces(this string str)
        {
            return str.Replace(" ", string.Empty);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string DeleteByRegex(this string str, string pattern)
        {
            return Regex.Replace(str, pattern, string.Empty);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string Delete(this string str, char toRemove)
        {
            return str.Replace($"{toRemove}", string.Empty);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string Delete(this string str, char toRemove, StringComparison stringComparison)
        {
            return str.Replace($"{toRemove}", string.Empty, stringComparison);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string Delete(this string str, IEnumerable<char> chars)
        {
            return str.Replace(chars, char.MinValue);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string Delete(this string str, params char[] chars)
        {
            return str.Delete((IEnumerable<char>)chars);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string Delete(this string str, string toRemove)
        {
            return str.Replace(toRemove, string.Empty);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string Delete(this string str,
                                    string toRemove,
                                    StringComparison stringComparison)
        {
            return str.Replace(toRemove, string.Empty, stringComparison);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string Delete(this string str, IEnumerable<string> strings)
        {
            return str.Replace(strings, string.Empty);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string Delete(this string str, params string[] strings)
        {
            return str.Delete((IEnumerable<string>)strings);
        }
        #endregion Delete

        #region Format
        public static string Format(this string str, bool missingArgsIsNull, params object[] args)
        {
            MatchCollection matches = Regex.Matches(str, @"\{\d+\}");
            if (matches == null || matches.Count == 0)
            {
                Debug.LogError("Incorrect string to format.");
                return str;
            }

            object[] convertedArgs = new object[matches.Count];
            Array.Copy(args, convertedArgs, args.Length);
            return string.Format(str, convertedArgs);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string Format(this string str, params object[] args)
        {
            return string.Format(str, args);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string Format(this string str, IFormatProvider provider, params object[] args)
        {
            return string.Format(provider, str, args);
        }
        #endregion Format

        #region Diagnsotics
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsEmpty(this string str) => str.Length == 0;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsNotEmpty(this string str) => !str.IsEmpty();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsNullOrEmpty([NotNullWhen(false)] this string? str)
        {
            return string.IsNullOrEmpty(str);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsNotNullOrEmpty([NotNullWhen(true)] this string? str)
        {
            return !string.IsNullOrEmpty(str);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsNullOrWhiteSpace([NotNullWhen(false)] this string? str)
        {
            return string.IsNullOrWhiteSpace(str);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsNotNullOrWhiteSpace([NotNullWhen(true)] this string? str)
        {
            return !str.IsNullOrWhiteSpace();
        }
        #endregion Diagnsotics

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool HasNullOrEmptyString(this string[] strings)
        {
            return strings.Any(x => x.IsNullOrEmpty());
        }

        private static string JoinToStringSkipEmpty(IEnumerable<string> strings, object separator)
        {
            StringBuilder sb = new();

            bool isFirst = true;
            foreach (var str in strings)
            {
                if (!isFirst)
                    sb.Append(separator);

                if (isFirst)
                    isFirst = false;

                if (str.IsNotEmpty())
                    sb.Append(str);
            }

            return sb.ToString();
        }
    }
}
