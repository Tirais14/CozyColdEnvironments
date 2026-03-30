using CCEnvs.FuncLanguage;
using CommunityToolkit.Diagnostics;
using R3;
using UnityEngine;

#nullable enable
namespace CCEnvs.Rx
{
    public static class ObservableExtensions
    {
        public static Observable<bool> WhereTrue(this Observable<bool> source)
        {
            Guard.IsNotNull(source);

            return source.Where(x => x);
        }

        public static Observable<bool> WhereFalse(this Observable<bool> source)
        {
            Guard.IsNotNull(source);

            return source.Where(x => !x);
        }

        public static Observable<T> WhereNotNull<T>(this Observable<T> source)
        {
            Guard.IsNotNull(source);

            return source.Where(x => x.IsNotNull());
        }

        public static Observable<T> WhereNotDefault<T>(this Observable<T> source)
        {
            Guard.IsNotNull(source);

            return source.Where(x => x.IsNotDefault());
        }

        public static Observable<sbyte> SelectDelta(this Observable<(sbyte Previous, sbyte Current)> source)
        {
            Guard.IsNotNull(source);
            return source.Select(x => (sbyte)(x.Current - x.Previous));
        }
        public static Observable<byte> SelectDelta(this Observable<(byte Previous, byte Current)> source)
        {
            Guard.IsNotNull(source);
            return source.Select(x => (byte)(x.Current - x.Previous));
        }

        public static Observable<short> SelectDelta(this Observable<(short Previous, short Current)> source)
        {
            Guard.IsNotNull(source);
            return source.Select(x => (short)(x.Current - x.Previous));
        }
        public static Observable<ushort> SelectDelta(this Observable<(ushort Previous, ushort Current)> source)
        {
            Guard.IsNotNull(source);
            return source.Select(x => (ushort)(x.Current - x.Previous));
        }
        public static Observable<int> SelectDelta(this Observable<(int Previous, int Current)> source)
        {
            Guard.IsNotNull(source);
            return source.Select(x => x.Current - x.Previous);
        }
        public static Observable<uint> SelectDelta(this Observable<(uint Previous, uint Current)> source)
        {
            Guard.IsNotNull(source);
            return source.Select(x => x.Current - x.Previous);
        }
        public static Observable<long> SelectDelta(this Observable<(long Previous, long Current)> source)
        {
            Guard.IsNotNull(source);
            return source.Select(x => x.Current - x.Previous);
        }
        public static Observable<ulong> SelectDelta(this Observable<(ulong Previous, ulong Current)> source)
        {
            Guard.IsNotNull(source);
            return source.Select(x => x.Current - x.Previous);
        }
        public static Observable<float> SelectDelta(this Observable<(float Previous, float Current)> source)
        {
            Guard.IsNotNull(source);
            return source.Select(x => x.Current - x.Previous);
        }
        public static Observable<double> SelectDelta(this Observable<(double Previous, double Current)> source)
        {
            Guard.IsNotNull(source);
            return source.Select(x => x.Current - x.Previous);
        }
        public static Observable<decimal> SelectDelta(this Observable<(decimal Previous, decimal Current)> source)
        {
            Guard.IsNotNull(source);
            return source.Select(x => x.Current - x.Previous);
        }

        public static Observable<T> Unmaybe<T>(this Observable<Maybe<T>> source)
        {
            Guard.IsNotNull(source);
            return source.Select(x => x.GetValue()!);
        }

        public static Observable<T> WhereEnabled<T, TMono>(this Observable<T> source, TMono mono)
            where TMono : MonoBehaviour
        {
            Guard.IsNotNull(source);
            return source.Where(mono, static (_, mono) => mono.isActiveAndEnabled);
        }
    }
}
