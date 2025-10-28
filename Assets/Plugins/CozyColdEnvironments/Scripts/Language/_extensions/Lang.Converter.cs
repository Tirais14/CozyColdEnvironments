using CommunityToolkit.Diagnostics;
using System;
using System.Runtime.CompilerServices;
using UnityEngine;

#nullable enable
#pragma warning disable S3236
namespace CCEnvs.FuncLanguage
{
    public static partial class Lang
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Maybe<TValue> Maybe<TValue>(this TValue input)
        {
            return input;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static MaybeStruct<TValue> Maybe<TValue>(this TValue input, TValue defaultValue)
            where TValue : struct
        {
            return new MaybeStruct<TValue>(input, defaultValue);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static MaybeStruct<TValue> Maybe<TValue>(this TValue input, bool hasValue)
            where TValue : struct
        {
            return new MaybeStruct<TValue>(input, hasValue);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static MaybeStruct<TValue> Maybe<TValue>(this TValue input, Predicate<TValue> predicate)
            where TValue : struct
        {
            Guard.IsNotNull(predicate, nameof(predicate));

            return new MaybeStruct<TValue>(input, predicate(input));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static MaybeStruct<TValue> Maybe<T, TValue>(this T input, TValue defaultValue)
            where T : struct, IConditional<TValue>
            where TValue : struct
        {
            return new MaybeStruct<TValue>(input.Access(), defaultValue);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static MaybeStruct<TValue> Maybe<T, TValue>(this T input, bool hasValue)
            where T : struct, IConditional<TValue>
            where TValue : struct
        {
            return new MaybeStruct<TValue>(input.Access(), hasValue);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static MaybeStruct<TValue> Maybe<T, TValue>(this T input, Predicate<TValue> predicate)
            where T : struct, IConditional<TValue>
            where TValue : struct
        {
            Guard.IsNotNull(predicate, nameof(predicate));

            return new MaybeStruct<TValue>(input.Access(), predicate(input.Access()));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Catched<TValue> Catch<TValue>(this TValue source,
            LogType logType = LogType.Log)
        {
            return new Catched<TValue>(source, logType);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Catched<TValue> Catch<T, TValue>(this T source,
            LogType logType = LogType.Log)
            where T : struct, IConditional<TValue>
        {
            return new Catched<TValue>(source.Access(), logType);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Resolver Resolve(this bool source) => source;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Resolver Resolve<T>(this T source, Predicate<T> predicate)
        {
            return new Resolver(predicate(source));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Resolver Resolve<T>(this T source)
            where T : struct, IConditional
        {
            return new Resolver(source.IsSome);
        }

    }
}
