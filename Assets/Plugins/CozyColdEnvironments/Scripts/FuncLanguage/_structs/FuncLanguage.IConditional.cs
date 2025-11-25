using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

#nullable enable
namespace CCEnvs.FuncLanguage
{
    public partial struct Maybe<T> : IConditional<T, Maybe<T>>
    {
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly T? GetValue() => target;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [DebuggerStepThrough]
        public readonly T GetValue(T defaultValue)
        {
            return Lang.GetValue(this, defaultValue);
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly T GetValue(Func<T> defaultValueFactory)
        {
            return Lang.GetValue(this, defaultValueFactory);
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly bool TryGetValue([NotNullWhen(true)] out T? result)
        {
            return Lang.TryGetValue(this, out result);
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly T GetValueUnsafe()
        {
            return Lang.GetValueUnsafe<Maybe<T>, T>(this);
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly T GetValueUnsafe(Exception exception)
        {
            return Lang.GetValueUnsafe<Maybe<T>, T>(this, exception);
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly T GetValueUnsafe(Func<Exception> exceptionFactory)
        {
            return Lang.GetValueUnsafe<Maybe<T>, T>(this, exceptionFactory);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [DebuggerStepThrough]
        public readonly bool Has(T? value)
        {
            return Lang.Has(this, value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [DebuggerStepThrough]
        public readonly bool Has(Predicate<T> predicate)
        {
            return Lang.Has(this, predicate);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [DebuggerStepThrough]
        public readonly bool HasUnsafe(Predicate<T?> predicate)
        {
            return Lang.HasUnsafe(this, predicate);
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly Maybe<T> Apply(T? value) => value;

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly Either<T, R> Cast<R>()
        {
            return Lang.Cast<Maybe<T>, T, R>(this);
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly Maybe<T> Where(Predicate<T> predicate)
        {
            return Lang.Where(this, predicate);
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly Either<T, R> Select<R>(Func<T, R> selector)
        {
            return Lang.Select(this, selector);
        }
    }

    public partial struct IfElse<T> : IConditional<T, IfElse<T>>
    {
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly T GetValue() => target;

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly T GetValue(T defaultValue)
        {
            return Lang.GetValue(this, defaultValue);
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly T GetValue(Func<T> defaultValueFactory)
        {
            return Lang.GetValue(this, defaultValueFactory);
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly bool TryGetValue([NotNullWhen(true)] out T? result)
        {
            return Lang.TryGetValue(this, out result);
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly T GetValueUnsafe()
        {
            return Lang.GetValueUnsafe<IfElse<T>, T>(this);
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly T GetValueUnsafe(Exception exception)
        {
            return Lang.GetValueUnsafe<IfElse<T>, T>(this, exception);
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly T GetValueUnsafe(Func<Exception> exceptionFactory)
        {
            return Lang.GetValueUnsafe<IfElse<T>, T>(this, exceptionFactory);
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly bool Has(T? value)
        {
            return Lang.Has(this, value);
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly bool Has(Predicate<T> predicate)
        {
            return Lang.Has(this, predicate);
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly bool HasUnsafe(Predicate<T?> predicate)
        {
            return Lang.HasUnsafe(this, predicate);
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly IfElse<T> Apply(T? value)
        {
            return value;
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly Either<T, R> Cast<R>()
        {
            return Lang.Cast<IfElse<T>, T, R>(this);
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly IfElse<T> Where(Predicate<T> predicate)
        {
            return Lang.Where(this, predicate);
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly Either<T, R> Select<R>(Func<T, R> selector)
        {
            return Lang.Select(this, selector);
        }
    }
}
