using CCEnvs.FuncLanguage;
using R3;

#nullable enable
namespace CCEnvs.R3
{
    public static class ObservableExtensions
    {
        public static Observable<bool> WhereTrue(this Observable<bool> source)
        {
            CC.Guard.IsNotNullSource(source);

            return source.Where(x => x);
        }

        public static Observable<bool> WhereFalse(this Observable<bool> source)
        {
            CC.Guard.IsNotNullSource(source);

            return source.Where(x => !x);
        }

        public static Observable<T> WhereNotNull<T>(this Observable<T> source)
        {
            CC.Guard.IsNotNullSource(source);

            return source.Where(x => x.IsNotNull());
        }

        public static Observable<T> WhereNotDefault<T>(this Observable<T> source)
        {
            CC.Guard.IsNotNullSource(source);

            return source.Where(x => x.IsNotDefault());
        }

        public static Observable<sbyte> SelectDelta(this Observable<(sbyte Previous, sbyte Current)> source)
        {
            CC.Guard.IsNotNullSource(source);
            return source.Select(x => (sbyte)(x.Current - x.Previous));
        }
        public static Observable<byte> SelectDelta(this Observable<(byte Previous, byte Current)> source)
        {
            CC.Guard.IsNotNullSource(source);
            return source.Select(x => (byte)(x.Current - x.Previous));
        }

        public static Observable<short> SelectDelta(this Observable<(short Previous, short Current)> source)
        {
            CC.Guard.IsNotNullSource(source);
            return source.Select(x => (short)(x.Current - x.Previous));
        }
        public static Observable<ushort> SelectDelta(this Observable<(ushort Previous, ushort Current)> source)
        {
            CC.Guard.IsNotNullSource(source);
            return source.Select(x => (ushort)(x.Current - x.Previous));
        }
        public static Observable<int> SelectDelta(this Observable<(int Previous, int Current)> source)
        {
            CC.Guard.IsNotNullSource(source);
            return source.Select(x => x.Current - x.Previous);
        }
        public static Observable<uint> SelectDelta(this Observable<(uint Previous, uint Current)> source)
        {
            CC.Guard.IsNotNullSource(source);
            return source.Select(x => x.Current - x.Previous);
        }
        public static Observable<long> SelectDelta(this Observable<(long Previous, long Current)> source)
        {
            CC.Guard.IsNotNullSource(source);
            return source.Select(x => x.Current - x.Previous);
        }
        public static Observable<ulong> SelectDelta(this Observable<(ulong Previous, ulong Current)> source)
        {
            CC.Guard.IsNotNullSource(source);
            return source.Select(x => x.Current - x.Previous);
        }
        public static Observable<float> SelectDelta(this Observable<(float Previous, float Current)> source)
        {
            CC.Guard.IsNotNullSource(source);
            return source.Select(x => x.Current - x.Previous);
        }
        public static Observable<double> SelectDelta(this Observable<(double Previous, double Current)> source)
        {
            CC.Guard.IsNotNullSource(source);
            return source.Select(x => x.Current - x.Previous);
        }
        public static Observable<decimal> SelectDelta(this Observable<(decimal Previous, decimal Current)> source)
        {
            CC.Guard.IsNotNullSource(source);
            return source.Select(x => x.Current - x.Previous);
        }

        public static Observable<T> Unmaybe<T>(this Observable<Maybe<T>> source)
        {
            CC.Guard.IsNotNullSource(source);
            return source.Select(x => x.GetValue()!);
        }
    }
}
