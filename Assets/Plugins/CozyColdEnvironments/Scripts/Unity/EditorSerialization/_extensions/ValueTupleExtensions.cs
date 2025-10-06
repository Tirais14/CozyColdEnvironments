#nullable enable
namespace CCEnvs.Unity.EditorSerialization
{
    public static class ValueTupleExtensions
    {
        public static SerializedTuple<T1, T2> ToSerializedTuple<T1, T2>(
            this (T1, T2) source)
        {
            return SerializedTuple.Create(source.Item1, source.Item2);
        }
        public static SerializedTuple<T1, T2, T3> ToSerializedTuple<T1, T2, T3>(
            this (T1, T2, T3) source)
        {
            return SerializedTuple.Create(source.Item1, source.Item2, source.Item3);
        }
        public static SerializedTuple<T1, T2, T3, T4> ToSerializedTuple<T1, T2, T3, T4>(
            this (T1, T2, T3, T4) source)
        {
            return SerializedTuple.Create(
                source.Item1,
                source.Item2,
                source.Item3,
                source.Item4);
        }
        public static SerializedTuple<T1, T2, T3, T4, T5> ToSerializedTuple<T1, T2, T3, T4, T5>(
            this (T1, T2, T3, T4, T5) source)
        {
            return SerializedTuple.Create(
                source.Item1,
                source.Item2,
                source.Item3,
                source.Item4,
                source.Item5);
        }
        public static SerializedTuple<T1, T2, T3, T4, T5, T6> ToSerializedTuple<T1, T2, T3, T4, T5, T6>(
            this (T1, T2, T3, T4, T5, T6) source)
        {
            return SerializedTuple.Create(
                source.Item1,
                source.Item2,
                source.Item3,
                source.Item4,
                source.Item5,
                source.Item6);
        }
    }
}
