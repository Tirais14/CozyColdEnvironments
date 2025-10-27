using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

#nullable enable
namespace CCEnvs.FuncLanguage
{
    public partial struct MaybeStruct<T>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly MaybeStruct<T> IfSome(Action<T> action)
        {
            return Lang.IfSome(this, action);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly MaybeStruct<T> IfNone(Action action)
        {
            return Lang.IfNone(this, action);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly IConditional IfNone<TOut>(Func<TOut> selector)
        {
            return Lang.IfNone(this, selector);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly MaybeStruct<T> Match(Action<T> some, Action none)
        {
            return Lang.Match(this, some, none);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly Maybe<TOut> Match<TOut>(Func<T, TOut?> some, Func<TOut?> none)
        {
            return Lang.Match(this, some, none);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly Maybe<TOut> Map<TOut>(Func<T, TOut?> selector)
        {
            return Lang.Map(this, selector);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly Maybe<TOut> MapUnsafe<TOut>(Func<T, TOut?> selector)
        {
            return Lang.MapUnsafe(this, selector);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly bool ItIs(T value)
        {
            return Lang.ItIs(this, value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly bool ItIs(Predicate<T> predicate)
        {
            return Lang.ItIs(this, predicate);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly bool CheckUnsafe(Predicate<T> predicate)
        {
            return Lang.CheckUnsafe(this, predicate);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly T Access() => inner;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly bool Access([NotNullWhen(true)] out T result)
        {
            result = inner;

            return IsSome;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly T Access(T defaultValue)
        {
            return Lang.Access(this, defaultValue);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly T Access(Func<T> defaultValueFactory)
        {
            return Lang.Access(this, defaultValueFactory);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly T AccessUnsafe()
        {
            return Lang.AccessUnsafe<MaybeStruct<T>, T>(this);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly MaybeStruct<T> Apply(T value)
        {
            return value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly Maybe<TOut> Cast<TOut>()
        {
            return Lang.Cast<MaybeStruct<T>, T, TOut>(this);
        }
    }
}
