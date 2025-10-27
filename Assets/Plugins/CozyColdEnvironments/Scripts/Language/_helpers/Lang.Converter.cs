using System.Runtime.CompilerServices;
using UnityEngine;

#nullable enable
#pragma warning disable S3236
namespace CCEnvs.Language
{
    public static partial class Lang
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Maybe<T> Maybe<T>(this T source) => source;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Catched<T> Catch<T>(this T source,
            LogType logType = LogType.Log)
        {
            return new Catched<T>(source, logType);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static MaybeStruct<T> MaybeStruct<T>(this T source)
            where T : struct
        {
            return source;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static MaybeStruct<T> MaybeStruct<T>(this Maybe<T> source)
            where T : struct
        {
            return source.Access();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static MaybeStruct<T> Struct<T>(this Maybe<T> source)
            where T : struct
        {
            return source.Access();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Resolver Resolve(this bool source) => source;
    }
}
