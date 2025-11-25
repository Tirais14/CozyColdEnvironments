using CommunityToolkit.Diagnostics;
using System;
using System.Runtime.CompilerServices;

#nullable enable
#pragma warning disable S3236
namespace CCEnvs.FuncLanguage
{
    public static partial class Lang
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Maybe<TOutValue> MapUnsafe<T, TValue, TOutValue>(
            T source,
            Func<TValue?, TOutValue?> selector)
            where T : struct, IConditional<TValue>
        {
            Guard.IsNotNull(selector, nameof(selector));

            return selector(source.GetValue()!);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool HasUnsafe<T, TValue>(T source, Predicate<TValue?> predicate)
            where T : struct, IConditional<TValue>
        {
            Guard.IsNotNull(predicate, nameof(predicate));

            return predicate(source.GetValue());
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TValue GetValueUnsafe<T, TValue>(T source)
            where T : struct, IConditional<TValue>
        {
            if (source.IsNone)
                throw new ValueIsNoneException();

            return source.GetValue()!;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TValue GetValueUnsafe<T, TValue>(T source, Exception exception)
            where T : struct, IConditional<TValue>
        {
            Guard.IsNotNull(exception);

            if (source.IsNone)
                throw exception;

            return source.GetValue()!;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TValue GetValueUnsafe<T, TValue>(T source, Func<Exception> exceptionFactory)
            where T : struct, IConditional<TValue>
        {
            Guard.IsNotNull(exceptionFactory);

            if (source.IsNone)
                throw exceptionFactory();

            return source.GetValue()!;
        }
    }
}
