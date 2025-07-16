using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using UTIRLib.Collections;

#nullable enable

namespace UTIRLib.Extensions
{
    public static class StringExtensions
    {
        public static bool HasNullOrEmptyString(this IReadOnlyList<string> strings)
        {
            int stringsCount = strings.Count;
            for (int i = 0; i < stringsCount; i++)
            {
                if (strings.IsNullOrEmpty())
                    return true;
            }

            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string[] SplitByLines(this string str,
                                            StringSplitOptions stringSplitOptions = StringSplitOptions.None)
        {
            return str.Split(new string[] { "\r\n", "\r", "\n" }, stringSplitOptions);
        }

        public static string InsertWhitespacesByCase(this string str)
        {
            Regex regex = new(@"([a-zà-ÿ])([A-ZÀ-ß])");

            string result = regex.Replace(str, "$1 $2");

            return result;
        }

        public static bool IsWrapped(this string str, char wrapChar)
        {
            return str.StartsWith(wrapChar) && str.EndsWith(wrapChar);
        }

        public static bool IsWrapped(this string str, string wrapStr)
        {
            return str.StartsWith(wrapStr) && str.EndsWith(wrapStr);
        }

        public static bool IsWrappedByDoubleQuotes(this string str)
        {
            return str.IsWrapped('\"');
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string Wrap(this string str, char wrapChar) => wrapChar + str + wrapChar;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string Wrap(this string str, string wrapStr) => wrapStr + str + wrapStr;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string WrapByDoubleQuotes(this string str) => str.Wrap('\"');
    }
}