using System.Collections.Generic;
using UnityEngine;

#nullable enable
namespace CCEnvs.Unity.EditorSerialization
{
    public static class ValueTupleExtensions
    {
        public static SerializedTuple<T1, T2> ToSerializedTuple<T1, T2>(this (T1, T2) source)
        {
            return new SerializedTuple<T1, T2>(source.Item1, source.Item2);
        }
    }
}
