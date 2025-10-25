#nullable enable
using CCEnvs.Diagnostics;
using CommunityToolkit.Diagnostics;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using static Humanizer.On;

#pragma warning disable S3236
namespace CCEnvs.Language
{
    public static class Lang
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T IfSome<T, TValue>(T source, Action<TValue> action)
            where T : struct, IConditional<TValue>
        {
            Guard.IsNotNull(action, nameof(action));

            if (source.IsSome)
                action(source.Access()!);

            return source;
        }

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
        public static T IfNone<T>(T source, Action action)
            where T : struct, IConditional
        {
            Guard.IsNotNull(action, nameof(action));

            if (source.IsNone)
                action();

            return source;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Maybe<TOutValue> Map<T, TValue, TOutValue>(
            T source,
            Func<TValue, TOutValue?> selector)
            where T : struct, IConditional<TValue>
        {
            Guard.IsNotNull(selector, nameof(selector));

            return source.IsSome ? selector(source.AccessUnsafe()) : default!;
        }

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
        public static T Match<T, TValue>(
            T source,
            Action<TValue> some,
            Action none)
            where T : struct, IConditional<TValue>
        {
            Guard.IsNotNull(some, nameof(some));
            Guard.IsNotNull(none, nameof(none));

            if (source.IsSome)
                some(source.AccessUnsafe());
            else
                none();

            return source;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Maybe<TOutValue> Match<T, TValue, TOutValue>(
            T source,
            Func<TValue, TOutValue?> some,
            Func<TOutValue?> none)
            where T : struct, IConditional<TValue>
        {
            Guard.IsNotNull(some, nameof(some));
            Guard.IsNotNull(none, nameof(none));

            if (source.IsSome)
                return some(source.AccessUnsafe());
            else
                return none();
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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Check<T, TValue>(
            T source,
            TValue? value,
            IEqualityComparer<TValue?>? comparer = null)
            where T : struct, IConditional<TValue>
        {
            comparer ??= EqualityComparer<TValue?>.Default;

            return comparer.Equals(source.AccessUnsafe(), value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Check<T, TValue>(T source, Predicate<TValue> predicate)
            where T : struct, IConditional<TValue>
        {
            if (source.IsNone)
                return false;

            Guard.IsNotNull(predicate, nameof(predicate));

            return predicate(AccessUnsafe<T, TValue>(source));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool CheckUnsafe<T, TValue>(T source, Predicate<TValue?> predicate)
            where T : struct, IConditional<TValue> 
        {
            Guard.IsNotNull(predicate, nameof(predicate));

            return predicate(source.Access());
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TValue? Access<T, TValue>(T source, TValue? defaultValue)
            where T : struct, IConditional<TValue>
        {
            if (source.IsNone)
                return defaultValue;

            return source.Access()!;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TValue? Access<T, TValue>(T source, Func<TValue?> defaultValueFactory)
            where T : struct, IConditional<TValue>
        {
            if (source.IsNone)
                return defaultValueFactory();

            return source.Access()!;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TValue AccessUnsafe<T, TValue>(T source)
            where T : struct, IConditional<TValue>
        {
            if (source.IsNone)
                throw new Diagnostics.CCException("Value is none");

            return source.Access()!;
        }

        public static Maybe<TValue> Unfold<TValue>(IConditional source)
        {
            switch (source.Access())
            {
                case Maybe<TValue> maybe:
                    return Unfold<Maybe<TValue>, TValue>(maybe).Access();
                case Catched<TValue> catched:
                    return Unfold<Catched<TValue>, TValue>(catched).Access();
                default:
                    if (source is IConditional<TValue> untyped)
                        return Unfold<TValue>(untyped);

                    return source.As<TValue>();
            }
        }

        public static Maybe<TValue> Unfold<T, TValue>(T source)
            where T : struct, IConditional<TValue>
        {
            switch (source.Access())
            {
                case Maybe<TValue> maybe:
                    return Unfold<Maybe<TValue>, TValue>(maybe).Access();
                case Catched<TValue> catched:
                    return Unfold<Catched<TValue>, TValue>(catched).Access();
                default:
                    if (source is IConditional<TValue> untyped)
                        return Unfold<TValue>(untyped);

                    return source.As<TValue>();
            }
        }

#nullable disable
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
#nullable enable
    }
}
