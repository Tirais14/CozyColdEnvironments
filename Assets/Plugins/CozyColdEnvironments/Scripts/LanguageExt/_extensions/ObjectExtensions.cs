#if LANGUAGE_EXT
using CCEnvs.Language;
using LanguageExt;
using System;
using System.Runtime.CompilerServices;

#nullable enable
namespace CCEnvs.LanguageExt
{
    public static class ObjectExtensions
    {
#if UNITY_2017_1_OR_NEWER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Some<T?> ToUnitySome<T>(this T? source)
        {
            return source.IsNull() ? new Some<T?>() : source;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Option<T> ToUnityOption<T>(this T source)
        {
            return source.IsNull() ? Option<T>.None : source;
        }
#endif

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Some<TValue> AsSome<T, TValue>(this T source)
            where T : struct, IConditional<TValue>
        {
            if (source.IsNone)
                return default!;

            return source.Value()!;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Option<TValue> AsOption<T, TValue>(this T source)
            where T : struct, IConditional<TValue>
        {
            if (source.IsNone)
                return Option<TValue>.None;

            return source.Value()!;
        }
    }
}
#endif //LANGUAGE_EXT
