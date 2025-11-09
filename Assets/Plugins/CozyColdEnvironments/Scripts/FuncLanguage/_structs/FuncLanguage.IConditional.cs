using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

#nullable enable
namespace CCEnvs.FuncLanguage
{
    public partial struct Catched<T> : IConditional<T, Catched<T>>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly T? Access() => target;

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly T Access(T defaultValue)
        {
            return Lang.Access(this, defaultValue);
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly T Access(Func<T> defaultValueFactory)
        {
            return Lang.Access(this, defaultValueFactory);
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly bool TryAccess([NotNullWhen(true)] out T? result)
        {
            return Lang.TryAccess(this, out result);
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly T AccessUnsafe()
        {
            return Lang.AccessUnsafe<Catched<T>, T>(this);
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

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly Catched<T> Apply(T? value) => value!;

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly Either<T, R> Cast<R>()
        {
            return Lang.Cast<Catched<T>, T, R>(this);
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly Catched<T> Where(Predicate<T> predicate)
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

    public partial struct Maybe<T> : IConditional<T, Maybe<T>>
    {
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly T? Access() => target;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [DebuggerStepThrough]
        public readonly T Access(T defaultValue)
        {
            return Lang.Access(this, defaultValue);
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly T Access(Func<T> defaultValueFactory)
        {
            return Lang.Access(this, defaultValueFactory);
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly bool TryAccess([NotNullWhen(true)] out T? result)
        {
            return Lang.TryAccess(this, out result);
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly T AccessUnsafe()
        {
            return Lang.AccessUnsafe<Maybe<T>, T>(this);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [DebuggerStepThrough]
        public readonly bool ItIs(T? value)
        {
            return Lang.ItIs(this, value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [DebuggerStepThrough]
        public readonly bool ItIs(Predicate<T> predicate)
        {
            return Lang.ItIs(this, predicate);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [DebuggerStepThrough]
        public readonly bool ItIsUnsafe(Predicate<T?> predicate)
        {
            return Lang.CheckUnsafe(this, predicate);
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

    public partial struct MaybeStruct<T> : IConditional<T, MaybeStruct<T>>
    {
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly T Access() => target;

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly T Access(T defaultValue)
        {
            return Lang.Access(this, defaultValue);
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly T Access(Func<T> defaultValueFactory)
        {
            return Lang.Access(this, defaultValueFactory);
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly bool TryAccess([NotNullWhen(true)] out T result)
        {
            return Lang.TryAccess(this, out result);
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly T AccessUnsafe()
        {
            return Lang.AccessUnsafe<MaybeStruct<T>, T>(this);
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly bool ItIs(T value)
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
        public readonly bool ItIsUnsafe(Predicate<T> predicate)
        {
            return Lang.CheckUnsafe(this, predicate);
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly MaybeStruct<T> Apply(T value)
        {
            return value;
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly Either<T, R> Cast<R>()
        {
            return Lang.Cast<MaybeStruct<T>, T, R>(this);
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly MaybeStruct<T> Where(Predicate<T> predicate)
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
        public readonly T Access() => target;

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly T Access(T defaultValue)
        {
            return Lang.Access(this, defaultValue);
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly T Access(Func<T> defaultValueFactory)
        {
            return Lang.Access(this, defaultValueFactory);
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly bool TryAccess([NotNullWhen(true)] out T? result)
        {
            return Lang.TryAccess(this, out result);
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly T AccessUnsafe()
        {
            return Lang.AccessUnsafe<IfElse<T>, T>(this);
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
