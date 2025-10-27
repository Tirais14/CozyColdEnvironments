#nullable enable
using CCEnvs.Diagnostics;
using System;
using System.Runtime.CompilerServices;
using UnityEngine;

#pragma warning disable S3236
namespace CCEnvs.Language
{
    public static partial class Lang
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T TryIfSome<T, TValue>(
            T source,
            Action<TValue> action,
            LogType logType)
            where T : struct, IConditional<TValue>
        {
            try
            {
                IfSome(source, action);
            }
            catch (Exception ex)
            {
                typeof(Lang).PrintDebug(ex, logType);
            }

            return source;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Maybe<TOutValue> TryMap<T, TValue, TOutValue>(
            T source,
            Func<TValue, TOutValue?> selector,
            LogType logType)
            where T : struct, IConditional<TValue>
        {
            try
            {
                return Map(source, selector);
            }
            catch (Exception ex)
            {
                typeof(Lang).PrintDebug(ex, logType);

                return default!;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T TryMatch<T, TValue>(
            T source,
            Action<TValue> some,
            Action noneOrCatched,
            LogType logType)
            where T : struct, IConditional<TValue>
        {
            try
            {
                Match(source, some, noneOrCatched);
            }
            catch (Exception ex)
            {
                typeof(Lang).PrintDebug(ex, logType);
            }

            return source;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Maybe<TOutValue> TryMatch<T, TValue, TOutValue>(
            T source,
            Func<TValue, TOutValue?> some,
            Func<TOutValue?> noneOrCatched,
            LogType logType)
            where T : struct, IConditional<TValue>
        {
            try
            {
                return Match(source, some, noneOrCatched);
            }
            catch (Exception ex)
            {
                typeof(Lang).PrintDebug(ex, logType);

                return noneOrCatched();
            }
        }
    }
}
