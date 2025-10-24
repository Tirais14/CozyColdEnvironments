#nullable enable
using CCEnvs.Diagnostics;
using CommunityToolkit.Diagnostics;
using System;
using System.Runtime.CompilerServices;

#pragma warning disable S3236
namespace CCEnvs.Language
{
    public static class Lang
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T IfSome<T, TValue>(this T source, Action<TValue> action)
            where T : struct, IConditional<TValue>
        {
            Guard.IsNotNull(action, nameof(action));

            if (source.IsSome)
                action(source.Value()!);

            return source;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T TryIfSome<T, TValue>(this T source, Action<TValue> action)
            where T : struct, IConditional<TValue>
        {
            try
            {
                source.IfSome(action);
            }
            catch (Exception ex)
            {
                typeof(Lang).PrintLog(ex);
            }

            return source;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T IfNone<T>(this T source, Action action)
            where T : struct, IConditional
        {
            Guard.IsNotNull(action, nameof(action));

            if (source.IsNone)
                action();

            return source;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Conditional<TOutValue> IfNone<T, TValue, TOutValue>(
            this T source,
            TOutValue? defaultValue)
            where T : struct, IConditional<TValue>
        {
            if (source.IsNone)
                return defaultValue!;

            return default;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Conditional<TOutValue> IfNone<T, TValue, TOutValue>(
            this T source,
            Func<TOutValue?> defaultValueFactory)
            where T : struct, IConditional<TValue>
        {
            Guard.IsNotNull(defaultValueFactory, nameof(defaultValueFactory));

            if (source.IsNone)
                return defaultValueFactory()!;

            return default;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Conditional<TOutValue> Map<T, TValue, TOutValue>(
            this T source,
            Func<TValue, TOutValue> selector)
            where T : struct, IConditional<TValue>
        {
            Guard.IsNotNull(selector, nameof(selector));

            return source.IsSome ? selector(source.Value()!) : default!;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Conditional<TOutValue> TryMap<T, TValue, TOutValue>(
            this T source,
            Func<TValue, TOutValue> selector)
            where T : struct, IConditional<TValue>
        {
            try
            {
                return source.Map(selector);
            }
            catch (Exception ex)
            {
                typeof(Lang).PrintError(ex);

                return default;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T Match<T, TValue>(
            this T source,
            Action<TValue> some,
            Action none)
            where T : struct, IConditional<TValue>
        {
            Guard.IsNotNull(some, nameof(some));
            Guard.IsNotNull(none, nameof(none));

            if (source.IsSome)
                some(source.Value()!);
            else
                none();

            return source;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Conditional<TOutValue> Match<T, TValue, TOutValue>(
            this T source,
            Func<TValue, TOutValue> some,
            Func<TOutValue> none)
            where T : struct, IConditional<TValue>
        {
            Guard.IsNotNull(some, nameof(some));
            Guard.IsNotNull(none, nameof(none));

            if (source.IsSome)
                return some(source.Value()!);
            else
                return none();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T TryMatch<T, TValue>(
            this T source,
            Action<TValue> some,
            Action noneOrCatched)
            where T : struct, IConditional<TValue>
        {
            try
            {
                source.Match(some, noneOrCatched);
            }
            catch (Exception ex)
            {
                typeof(Lang).PrintLog(ex);
            }

            return source;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Conditional<TOutValue> TryMatch<T, TValue, TOutValue>(
            this T source,
            Func<TValue, TOutValue> some,
            Func<TOutValue> noneOrCatched)
            where T : struct, IConditional<TValue>
        {
            try
            {
                return source.Match(some, noneOrCatched);
            }
            catch (Exception ex)
            {
                typeof(Lang).PrintLog(ex);

                return noneOrCatched();
            }
        }

        public static bool Check<T, TValue>(T source, Predicate<TValue> predicate)
            where T : struct, IConditional<TValue>
        {
            if (source.IsNone)
                return false;

            Guard.IsNotNull(predicate, nameof(predicate));

            return predicate(source.ValueUnsafe<T, TValue>());
        }

        public static bool CheckUnsafe<T, TValue>(T source, Predicate<TValue?> predicate)
            where T : struct, IConditional<TValue> 
        {
            Guard.IsNotNull(predicate, nameof(predicate));

            return predicate(source.Value());
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TValue? Value<T, TValue>(this T source, TValue? defaultValue)
            where T : struct, IConditional<TValue>
        {
            if (source.IsNone)
                return defaultValue;

            return source.Value()!;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TValue? Value<T, TValue>(this T source, Func<TValue?> defaultValueFactory)
            where T : struct, IConditional<TValue>
        {
            if (source.IsNone)
                return defaultValueFactory();

            return source.Value()!;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TValue ValueUnsafe<T, TValue>(this T source)
            where T : struct, IConditional<TValue>
        {
            if (source.IsNone)
                throw new Diagnostics.CCException("Value is none");

            return source.Value()!;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Ghost<T> ToGhost<T>(this T source) => source;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Ghost<T> AsGhost<T>(this Conditional<T> source)
        {
            return source.Value();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Trapped<T> AsTrapped<T>(this Conditional<T> source)
        {
            return source.Value();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static GhostStruct<T> ToGhostStruct<T>(this T source)
            where T : struct
        {
            return source;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static GhostStruct<T> AsGhostStruct<T>(this Conditional<T> source)
            where T : struct
        {
            return source.Value();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Conditional<T> ToConditional<T>(this T source) => source;
    }
}
