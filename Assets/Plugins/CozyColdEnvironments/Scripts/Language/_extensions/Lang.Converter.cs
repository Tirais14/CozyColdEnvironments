using CCEnvs.Diagnostics;
using CommunityToolkit.Diagnostics;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

#nullable enable
#pragma warning disable S3236
namespace CCEnvs.FuncLanguage
{
    public static partial class Lang
    {
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Maybe<TValue> Maybe<TValue>(this TValue input) => input;

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static MaybeStruct<TValue> Maybe<TValue>(this TValue input, TValue defaultValue)
            where TValue : struct
        {
            return new MaybeStruct<TValue>(input, defaultValue);
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static MaybeStruct<TValue> Maybe<TValue>(this TValue input, bool hasValue)
            where TValue : struct
        {
            return new MaybeStruct<TValue>(input, hasValue);
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static MaybeStruct<TValue> Maybe<TValue>(this TValue input, Predicate<TValue> predicate)
            where TValue : struct
        {
            Guard.IsNotNull(predicate, nameof(predicate));

            return new MaybeStruct<TValue>(input, predicate(input));
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static MaybeStruct<TValue> Maybe<T, TValue>(this T input, TValue defaultValue)
            where T : struct, IMaybe<TValue>
            where TValue : struct
        {
            return new MaybeStruct<TValue>(input.Access(), defaultValue);
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static MaybeStruct<TValue> Maybe<T, TValue>(this T input, bool hasValue)
            where T : struct, IMaybe<TValue>
            where TValue : struct
        {
            return new MaybeStruct<TValue>(input.Access(), hasValue);
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static MaybeStruct<TValue> Maybe<T, TValue>(this T input, Predicate<TValue> predicate)
            where T : struct, IMaybe<TValue>
            where TValue : struct
        {
            Guard.IsNotNull(predicate, nameof(predicate));

            return new MaybeStruct<TValue>(input.Access(), predicate(input.Access()));
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Catched<TValue> Catch<TValue>(this TValue source,
            LogType logType = LogType.Log)
        {
            return new Catched<TValue>(source, logType);
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Catched<TValue> Catch<T, TValue>(this T source,
            LogType logType = LogType.Log)
            where T : struct, IMaybe<TValue>
        {
            return new Catched<TValue>(source.Access(), logType);
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IfElse Resolve(this bool source) => source;

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IfElse Resolve(this Func<bool> source) => source;

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IfElse<T> Resolve<T>(this T source) => source;

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Either<L, R> Either<L, R>(this L source, R right)
        {
            return (source, right);
        }
    }
}
