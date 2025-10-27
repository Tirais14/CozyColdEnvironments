#nullable enable
using CommunityToolkit.Diagnostics;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

#pragma warning disable S3236
namespace CCEnvs.FuncLanguage
{
    public static partial class Lang
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T IfSome<T, TValue>(T source, Action<TValue> action)
            where T : struct, IConditional<TValue>
        {
            Guard.IsNotNull(action, nameof(action));

            if (source.IsSome)
                action(source.AccessUnsafe());

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
        public static IConditional IfNone<T, TOut>(T source, Func<TOut> selector)
            where T : struct, IConditional
        {
            Guard.IsNotNull(selector, nameof(selector));
            if (source.IsSome)
                return source;

            return selector().Maybe();
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
        public static bool ItIs<T, TValue>(
            T source,
            TValue? value,
            IEqualityComparer<TValue?>? comparer = null)
            where T : struct, IConditional<TValue>
        {
            comparer ??= EqualityComparer<TValue?>.Default;

            return comparer.Equals(source.AccessUnsafe(), value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool ItIs<T, TValue>(T source, Predicate<TValue> predicate)
            where T : struct, IConditional<TValue>
        {
            if (source.IsNone)
                return false;

            Guard.IsNotNull(predicate, nameof(predicate));

            return predicate(AccessUnsafe<T, TValue>(source));
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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Maybe<TOutValue> Cast<T, TOutValue>(T source)
            where T : struct, IConditional
        {
            if (source.IsNone)
                return default!;

            return source.Access().AsOrDefault<TOutValue>();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Maybe<TOutValue> Cast<T, TValue, TOutValue>(T source)
            where T : struct, IConditional<TValue>
        {
            if (source.IsNone)
                return default!;

            return source.Access().AsOrDefault<TOutValue>();
        }
    }
}
