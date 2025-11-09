using CommunityToolkit.Diagnostics;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

#nullable enable
#pragma warning disable S3236
namespace CCEnvs.FuncLanguage
{
    public partial struct Catched<T> : IMaybe<T, Catched<T>>
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
        public readonly Either<T, R> IfNone<R>(Func<R> factory)
        {
            return Lang.IfNone<Catched<T>, T, R>(this, factory);
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
        public readonly Maybe<TOut> Unfold<TOut>()
        {
            return Lang.Unfold<Catched<T>, T, TOut>(this).Access()!;
        }
    }

    public partial struct Maybe<T> : IMaybe<T, Maybe<T>>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [DebuggerStepThrough]
        public readonly Maybe<T> IfSome(Action<T> action)
        {
            return Lang.IfSome(this, action);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [DebuggerStepThrough]
        public readonly Maybe<T> IfNone(Action action)
        {
            return Lang.IfNone(this, action);
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly Either<T, R> IfNone<R>(Func<R> factory)
        {
            return Lang.IfNone<Maybe<T>, T, R>(this, factory);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [DebuggerStepThrough]
        public readonly Maybe<T> Match(Action<T> some, Action none)
        {
            return Lang.Match(this, some, none);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [DebuggerStepThrough]
        public readonly Maybe<TOut> Match<TOut>(Func<T, TOut?> some, Func<TOut?> none)
        {
            return Lang.Match(this, some, none);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [DebuggerStepThrough]
        public readonly Maybe<TOut> Map<TOut>(Func<T, TOut?> selector)
        {
            return Lang.Map(this, selector);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [DebuggerStepThrough]
        public readonly Maybe<TOut> MapUnsafe<TOut>(Func<T?, TOut?> selector)
        {
            return Lang.MapUnsafe(this, selector);
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly Maybe<TOut> Unfold<TOut>()
        {
            return Lang.Unfold<Maybe<T>, T, TOut>(this);
        }
    }

    public partial struct MaybeStruct<T> : IMaybe<T, MaybeStruct<T>>
    {
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly MaybeStruct<T> IfSome(Action<T> action)
        {
            return Lang.IfSome(this, action);
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly MaybeStruct<T> IfNone(Action action)
        {
            return Lang.IfNone(this, action);
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly Either<T, R> IfNone<R>(Func<R> factory)
        {
            return Lang.IfNone<MaybeStruct<T>, T, R>(this, factory);
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly MaybeStruct<T> Match(Action<T> some, Action none)
        {
            return Lang.Match(this, some, none);
        }
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly Maybe<TOut> Match<TOut>(Func<T, TOut?> some, Func<TOut?> none)
        {
            return Lang.Match(this, some, none);
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly Maybe<TOut> Map<TOut>(Func<T, TOut?> selector)
        {
            return Lang.Map(this, selector);
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly Maybe<TOut> MapUnsafe<TOut>(Func<T, TOut?> selector)
        {
            return Lang.MapUnsafe(this, selector);
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly Maybe<TOut> Unfold<TOut>()
        {
            return Lang.Unfold<MaybeStruct<T>, T, TOut>(this).Access()!;
        }
    }
}
