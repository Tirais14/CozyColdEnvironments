#nullable enable
using System;
using System.Collections.Generic;

namespace CCEnvs
{
    public static class ValueTupleHelper
    {
        public static IEnumerable<object> AsEnumerable<T1, T2>(
            this ValueTuple<T1, T2> source)
        {
            yield return source.Item1!;
            yield return source.Item2!;
        }
        public static IEnumerable<object> AsEnumerable<T1, T2, T3>(
            this ValueTuple<T1, T2, T3> source)
        {
            yield return source.Item1!;
            yield return source.Item2!;
            yield return source.Item3!;
        }
        public static IEnumerable<object> AsEnumerable<T1, T2, T3, T4>(
            this ValueTuple<T1, T2, T3, T4> source)
        {
            yield return source.Item1!;
            yield return source.Item2!;
            yield return source.Item3!;
            yield return source.Item4!;
        }
        public static IEnumerable<object> AsEnumerable<T1, T2, T3, T4, T5>(
            this ValueTuple<T1, T2, T3, T4, T5> source)
        {
            yield return source.Item1!;
            yield return source.Item2!;
            yield return source.Item3!;
            yield return source.Item4!;
            yield return source.Item5!;
        }
        public static IEnumerable<object> AsEnumerable<T1, T2, T3, T4, T5, T6>(
            this ValueTuple<T1, T2, T3, T4, T5, T6> source)
        {
            yield return source.Item1!;
            yield return source.Item2!;
            yield return source.Item3!;
            yield return source.Item4!;
            yield return source.Item5!;
            yield return source.Item6!;
        }

        public static KeyValuePair<T1, T2> ToKeyValuePair<T1, T2>(this (T1, T2) source)
        {
            return new KeyValuePair<T1, T2>(source.Item1, source.Item2);
        }
    }
}
