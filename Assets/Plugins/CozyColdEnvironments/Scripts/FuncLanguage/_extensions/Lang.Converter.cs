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
        public static Maybe<TValue> Maybe<TValue>(this TValue source) => source;

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Maybe<TValue> Maybe<TValue>(this TValue source, TValue @default)
        {
            return (source, @default);
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Maybe<TValue> Maybe<TValue>(this TValue source,
            Predicate<TValue?> isSome)
        {
            Guard.IsNotNull(isSome, nameof(isSome));

            return new Maybe<TValue>(source, isSome);
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IfElse Resolve(this bool source) => source;

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IfElse Resolve(this Func<bool> source) => source;

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IfElse<T> Resolve<T>(this T source, Predicate<T>? predicate = null)
        {
            return (source, predicate);
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Either<L, R> Either<L, R>(this L source, R right)
        {
            return (source, right);
        }
    }
}
