#nullable enable
using System.Collections.Generic;

namespace CCEnvs
{
    public static class ValueTupleExtensions
    {
        public static KeyValuePair<T1, T2> ToKeyValuePair<T1, T2>(this (T1, T2) source)
        {
            return new KeyValuePair<T1, T2>(source.Item1, source.Item2);
        }
    }
}
