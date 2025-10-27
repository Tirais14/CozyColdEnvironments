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

            return selector(source.Access()!);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool CheckUnsafe<T, TValue>(T source, Predicate<TValue?> predicate)
            where T : struct, IConditional<TValue>
        {
            Guard.IsNotNull(predicate, nameof(predicate));

            return predicate(source.Access());
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TValue AccessUnsafe<T, TValue>(T source)
            where T : struct, IConditional<TValue>
        {
            if (source.IsNone)
                throw new Diagnostics.CCException("Value is none");

            return source.Access()!;
        }
    }
}
