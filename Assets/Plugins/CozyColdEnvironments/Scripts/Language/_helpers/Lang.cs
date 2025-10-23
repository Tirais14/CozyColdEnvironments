#nullable enable
using CommunityToolkit.Diagnostics;
using System;

#pragma warning disable S3236
namespace CCEnvs.Language
{
    public static class Lang
    {
        public static T IfSome<T, TValue>(this T source, Action<TValue> action)
            where T : struct, IConditional<TValue>
        {
            Guard.IsNotNull(action, nameof(action));

            if (source.IsSome)
                action(source.Value()!);

            return source;
        }

        public static T IfNone<T>(this T source, Action action)
            where T : struct, IConditional
        {
            Guard.IsNotNull(action, nameof(action));

            if (source.IsNone)
                action();

            return source;
        }
        public static Conditional<TOutValue> IfNone<T, TValue, TOutValue>(
            this T source,
            TOutValue? defaultValue)
            where T : struct, IConditional<TValue>
        {
            if (source.IsNone)
                return defaultValue!;

            return default;
        }
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

        public static Conditional<TOutValue> Map<T, TValue, TOutValue>(
            this T source,
            Func<TValue, TOutValue> selector)
            where T : struct, IConditional<TValue>
        {
            Guard.IsNotNull(selector, nameof(selector));

            return source.IsSome ? selector(source.Value()!) : default!;
        }

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

        public static TValue? Value<T, TValue>(this T source, TValue? defaultValue)
            where T : struct, IConditional<TValue>
        {
            if (source.IsNone)
                return defaultValue;

            return source.Value()!;
        }
        public static TValue? Value<T, TValue>(this T source, Func<TValue?> defaultValueFactory)
            where T : struct, IConditional<TValue>
        {
            if (source.IsNone)
                return defaultValueFactory();

            return source.Value()!;
        }

        public static TValue ValueUnsafe<T, TValue>(this T source)
            where T : struct, IConditional<TValue>
        {
            if (source.IsNone)
                throw new Diagnostics.CCException("Value is none");

            return source.Value()!;
        }

        public static Ghost<T> ToGhost<T>(this T source) => source;

        public static Ghost<T> AsGhost<T>(this Conditional<T> source)
        {
            return source.Value();
        }

        public static GhostStruct<T> ToGhostStruct<T>(this T source)
            where T : struct
        {
            return source;
        }

        public static GhostStruct<T> AsGhostStruct<T>(this Conditional<T> source)
            where T : struct
        {
            return source.Value();
        }

        public static Conditional<T> ToConditional<T>(this T source) => source;
    }
}
