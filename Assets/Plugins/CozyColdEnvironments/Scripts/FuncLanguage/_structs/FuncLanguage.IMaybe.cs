using CommunityToolkit.Diagnostics;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

#nullable enable
#pragma warning disable S3236
namespace CCEnvs.FuncLanguage
{
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
}
