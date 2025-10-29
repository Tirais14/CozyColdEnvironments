using CommunityToolkit.Diagnostics;
using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

#nullable enable
namespace CCEnvs.FuncLanguage
{
    public partial struct Catched<T> : IConditional<Catched<T>, T>
    {
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly Catched<T> IfSome(Action<T> action)
        {
            return Lang.TryIfSome(this, action, logType);
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly Catched<T> IfNone(Action action)
        {
            return Lang.IfNone(this, action);
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly IConditional IfNone<TOut>(Func<TOut> selector)
        {
            return Lang.IfNone(this, selector);
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly Catched<T> Match(Action<T> some, Action none)
        {
            return Lang.TryMatch(this, some, none, logType);
        }
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly Maybe<TOut> Match<TOut>(Func<T, TOut?> some, Func<TOut?> none)
        {
            return Lang.TryMatch(this, some, none, logType);
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly Maybe<TOut> Map<TOut>(Func<T, TOut?> selector)
        {
            return Lang.TryMap(this, selector, logType);
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly Maybe<TOut> MapUnsafe<TOut>(Func<T?, TOut?> selector)
        {
            return Lang.MapUnsafe(this, selector);
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly bool ItIs(T? value)
        {
            return Lang.ItIs(this, value);
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly bool ItIs(Predicate<T> predicate)
        {
            return Lang.ItIs(this, predicate);
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly bool ItIsUnsafe(Predicate<T?> predicate)
        {
            return Lang.CheckUnsafe(this, predicate);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly T? Access() => inner;

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly T? Access(T? defaultValue)
        {
            return Lang.Access(this, defaultValue);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly bool Access([NotNullWhen(true)] out T? result)
        {
            result = inner;

            return IsSome;
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly T? Access(Func<T?> defaultValueFactory)
        {
            return Lang.Access(this, defaultValueFactory);
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly T AccessUnsafe()
        {
            return Lang.AccessUnsafe<Catched<T>, T>(this);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly Catched<T> Apply(T? value)
        {
            return value!;
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly Maybe<TOut> Cast<TOut>()
        {
            return Lang.Cast<Catched<T>, T, TOut>(this);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly Catched<T> Where(Predicate<T> predicate)
        {
            Guard.IsNotNull(predicate, nameof(predicate));

            if (IsSome && predicate(inner!))
                return this;

            return default!;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly Maybe<TOut> Select<TOut>(Func<T, TOut> selector)
        {
            Guard.IsNotNull(selector, nameof(selector));

            if (IsNone)
                return Maybe<TOut>.None;

            return selector(inner!);
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly Catched<T> Unfold()
        {
            return Lang.Unfold<Catched<T>, T>(this).Access()!;
        }
    }
}
