#nullable enable
using CCEnvs.Diagnostics;
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
                action(input.GetValueUnsafe());

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

            L? left = input.GetValue();
            R? right = factory();

            return (left, right);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Maybe<TOutValue> Map<T, TValue, TOutValue>(T input,
            Func<TValue, TOutValue?> selector)
            where T : struct, IConditional<TValue>
        {
            Guard.IsNotNull(selector, nameof(selector));

            return input.IsSome ? selector(input.GetValueUnsafe()) : global::CCEnvs.FuncLanguage.Maybe<TOutValue>.None;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Maybe<TOutValue> BiMap<T, TValue, TOutValue>(T input,
            Func<TValue, TOutValue?> some,
            Func<TOutValue?> none)
            where T : struct, IConditional<TValue>
        {
            Guard.IsNotNull(some, nameof(some));
            Guard.IsNotNull(none, nameof(none));

            if (input.IsSome)
                return some(input.GetValueUnsafe());
            else
                return none();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T Do<T, TValue>(T input,
            Action<TValue> some,
            Action none)
            where T : struct, IConditional<TValue>
        {
            Guard.IsNotNull(some, nameof(some));
            Guard.IsNotNull(none, nameof(none));

            if (input.IsSome)
                some(input.GetValueUnsafe());
            else
                none();

            return input;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TOutValue Match<T, TValue, TOutValue>(T input, 
            Func<TValue, TOutValue> some,
            Func<TOutValue> none)
            where T : struct, IConditional<TValue>
        {
            Guard.IsNotNull(some, nameof(some));
            Guard.IsNotNull(none, nameof(none));

            if (input.IsSome)
                return some(input.GetValueUnsafe());
            else
                return none();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TValue Match<T, TValue>(T input,
            Action<TValue> some,
            Func<TValue> none)
            where T : struct, IConditional<TValue>
        {
            Guard.IsNotNull(some, nameof(some));
            Guard.IsNotNull(none, nameof(none));

            if (input.IsSome)
            {
                var value = input.GetValueUnsafe();
                some(value);
                return value;
            }
            else
                return none();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Has<T, TValue>(T input,
            TValue? value,
            IEqualityComparer<TValue?>? comparer = null)
            where T : struct, IConditional<TValue>
        {
            comparer ??= EqualityComparer<TValue?>.Default;

            return comparer.Equals(input.GetValueUnsafe(), value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Has<T, TValue>(T input, Predicate<TValue> predicate)
            where T : struct, IConditional<TValue>
        {
            if (input.IsNone)
                return false;

            Guard.IsNotNull(predicate, nameof(predicate));

            return predicate(GetValueUnsafe<T, TValue>(input));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TValue GetValue<T, TValue>(T input, TValue @default)
            where T : struct, IConditional<TValue>
        {
            if (input.IsNone)
            {
                if (@default.IsNull())
                    throw new ValueIsNoneException();

                return @default;
            }

            return input.GetValueUnsafe();
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TValue GetValue<T, TValue>(T input, Func<TValue> factory)
            where T : struct, IConditional<TValue>
        {
            if (input.IsNone)
            {
                var t = factory();

                if (t.IsNull())
                    throw new ValueIsNoneException();

                return t;
            }

            return input.GetValueUnsafe();
        }

        public static bool TryGetValue<T, TValue>(T input, out TValue? result)
            where T : struct, IConditional<TValue>
        {
            result = input.GetValue();

            return input.IsSome;
        }

        public static Maybe<TOutValue> Unfold<TValue, TOutValue>(IConditional input)
        {
            return input.GetValue() switch
            {
                Maybe<TValue> maybe => Unfold<Maybe<TValue>, TValue, TOutValue>(maybe),
                IMaybe<TValue> untyped => Unfold<TValue, TOutValue>(untyped),
                Maybe<TOutValue> maybe => maybe.GetValue(),
                IMaybe<TOutValue> untyped => Unfold<TValue, TOutValue>(untyped),
                TOutValue value => value,
                _ => default
            };
        }

        public static Maybe<TOutValue> Unfold<T, TValue, TOutValue>(T input)
            where T : struct, IConditional<TValue>
        {
            return input.GetValue() switch
            {
                Maybe<TValue> maybe => Unfold<Maybe<TValue>, TValue, TOutValue>(maybe),
                IMaybe<TValue> untyped => Unfold<TValue, TOutValue>(untyped),
                Maybe<TOutValue> maybe => maybe.GetValue(),
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

            return input.GetValue().As<TOutValue>();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Either<L, R> Cast<T, L, R>(T input)
            where T : struct, IConditional<L>
        {
            L? left = input.GetValue();
            R? right = (R?)left.As<R>();

            return (left, right);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T Where<T, TValue>(T input, Predicate<TValue> predicate)
            where T : struct, IConditional<TValue>
        {
            Guard.IsNotNull(predicate, nameof(predicate));

            if (input.IsSome && predicate(input.GetValueUnsafe()))
                return input;

            return default!;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Either<L, R> Select<T, L, R>(T input,
            Func<L, R?> selector)

            where T : struct, IConditional<L>
        {
            Guard.IsNotNull(selector, nameof(selector));

            L? left = input.GetValue();
            R? right = default;

            if (input.IsSome)
                right = selector(left!);

            return (left, right);
        }
    }
}
