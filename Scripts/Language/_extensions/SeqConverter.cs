#nullable enable
namespace CCEnvs.Language
{
    public static class SeqConverter
    {
        public static Seq<T> ToSeq<T>(this T source) => new(source);
        public static Seq<T> ToSeq<T>(this (T, T) source)
        {
            return new Seq<T>(source.Item1, source.Item2);
        }
        public static Seq<T> ToSeq<T>(this (T, T, T) source)
        {
            return new Seq<T>(source.Item1, source.Item2, source.Item3);
        }
        public static Seq<T> ToSeq<T>(this (T, T, T, T) source)
        {
            return new Seq<T>(source.Item1, source.Item2, source.Item3, source.Item4);
        }
    }
}
