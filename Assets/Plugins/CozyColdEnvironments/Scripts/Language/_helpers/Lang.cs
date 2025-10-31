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
        public static T IfSome<T, TValue>(T input, Action<TValue> action)
            where T : struct, IConditional<TValue>
        {
            Guard.IsNotNull(action, nameof(action));

            if (input.IsSome)
                action(input.AccessUnsafe());

            return input;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T IfNone<T>(T input, Action action)
            where T : struct, IConditional
        {
            Guard.IsNotNull(action, nameof(action));

            if (input.IsNone)
                action();

            return input;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Either<L, R> IfNone<T, L, R>(T input, Func<R> factory)
            where T : struct, IConditional<L>
        {
            Guard.IsNotNull(factory, nameof(factory));

            L? left = input.Access();
            R? right = factory();

            return (left, right);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Maybe<TOutValue> Map<T, TValue, TOutValue>(T input,
            Func<TValue, TOutValue?> selector)
            where T : struct, IConditional<TValue>
        {
            Guard.IsNotNull(selector, nameof(selector));

            return input.IsSome ? selector(input.AccessUnsafe()) : default!;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T Match<T, TValue>(T input,
            Action<TValue> some,
            Action none)
            where T : struct, IConditional<TValue>
        {
            Guard.IsNotNull(some, nameof(some));
            Guard.IsNotNull(none, nameof(none));

            if (input.IsSome)
                some(input.AccessUnsafe());
            else
                none();

            return input;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Maybe<TOutValue> Match<T, TValue, TOutValue>(T input,
            Func<TValue, TOutValue?> some,
            Func<TOutValue?> none)
            where T : struct, IConditional<TValue>
        {
            Guard.IsNotNull(some, nameof(some));
            Guard.IsNotNull(none, nameof(none));

            if (input.IsSome)
                return some(input.AccessUnsafe());
            else
                return none();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool ItIs<T, TValue>(T input,
            TValue? value,
            IEqualityComparer<TValue?>? comparer = null)
            where T : struct, IConditional<TValue>
        {
            comparer ??= EqualityComparer<TValue?>.Default;

            return comparer.Equals(input.AccessUnsafe(), value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool ItIs<T, TValue>(T input, Predicate<TValue> predicate)
            where T : struct, IConditional<TValue>
        {
            if (input.IsNone)
                return false;

            Guard.IsNotNull(predicate, nameof(predicate));

            return predicate(AccessUnsafe<T, TValue>(input));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TValue Access<T, TValue>(T input, TValue defaultValue)
            where T : struct, IConditional<TValue>
        {
            if (input.IsNone)
                return defaultValue;

            return input.AccessUnsafe();
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TValue Access<T, TValue>(T input, Func<TValue> defaultValueFactory)
            where T : struct, IConditional<TValue>
        {
            if (input.IsNone)
                return defaultValueFactory();

            return input.AccessUnsafe();
        }

        public static bool TryAccess<T, TValue>(T input, out TValue? result)
            where T : struct, IConditional<TValue>
        {
            result = input.Access();

            return input.IsSome;
        }

        public static Maybe<TOutValue> Unfold<TValue, TOutValue>(IConditional input)
        {
            return input.Access() switch
            {
                Maybe<TValue> maybe => Unfold<Maybe<TValue>, TValue, TOutValue>(maybe),
                Catched<TValue> catched => Unfold<Catched<TValue>, TValue, TOutValue>(catched),
                IMaybe<TValue> untyped => Unfold<TValue, TOutValue>(untyped),
                Maybe<TOutValue> maybe => maybe.Access(),
                Catched<TOutValue> catched => catched.Access(),
                IMaybe<TOutValue> untyped => Unfold<TValue, TOutValue>(untyped),
                TOutValue value => value,
                _ => default
            };
        }

        public static Maybe<TOutValue> Unfold<T, TValue, TOutValue>(T input)
            where T : struct, IConditional<TValue>
        {
            return input.Access() switch
            {
                Maybe<TValue> maybe => Unfold<Maybe<TValue>, TValue, TOutValue>(maybe),
                Catched<TValue> catched => Unfold<Catched<TValue>, TValue, TOutValue>(catched),
                IMaybe<TValue> untyped => Unfold<TValue, TOutValue>(untyped),
                Maybe<TOutValue> maybe => maybe.Access(),
                Catched<TOutValue> catched => catched.Access(),
                IMaybe<TOutValue> untyped => Unfold<TValue, TOutValue>(untyped),
                TOutValue value => value,
                _ => default
            };
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Maybe<TOutValue> Cast<T, TOutValue>(T input)
            where T : struct, IConditional
        {
            if (input.IsNone)
                return default!;

            return input.Access().AsOrDefault<TOutValue>();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Either<L, R> Cast<T, L, R>(T input)
            where T : struct, IConditional<L>
        {
            L? left = input.Access();
            R? right = (R?)left.AsOrDefault<R>();

            return (left, right);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T Where<T, TValue>(T input, Predicate<TValue> predicate)
            where T : struct, IConditional<TValue>
        {
            Guard.IsNotNull(predicate, nameof(predicate));

            if (input.IsSome && predicate(input.AccessUnsafe()))
                return input;

            return default!;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Either<L, R> Select<T, L, R>(T input,
            Func<L, R?> selector)

            where T : struct, IConditional<L>
        {
            Guard.IsNotNull(selector, nameof(selector));

            L? left = input.Access();
            R? right = default;

            if (input.IsSome)
                right = selector(left!);

            return (left, right);
        }
    }
}
