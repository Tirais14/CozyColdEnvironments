using CommunityToolkit.Diagnostics;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using static UnityEngine.GraphicsBuffer;


#nullable enable
#pragma warning disable S3236
#pragma warning disable S3400
#pragma warning disable IDE1006
namespace CCEnvs.FuncLanguage
{
    public readonly struct IfElse
    {
        public readonly static IfElse None = default!;
        public readonly static Func<bool> @true = () => true;
        public readonly static Func<bool> @false = () => false;

        private readonly Func<bool> predicate;

        public readonly bool IsTrue => predicate();
        public readonly bool IsFalse => predicate();

        public readonly Func<bool> Predicate => predicate;

        public IfElse(Func<bool> predicate)
        {
            Guard.IsNotNull(predicate, nameof(predicate));

            this.predicate = predicate;
        }
        public IfElse(bool state)
            :
            this(state ? @true : @false)
        {
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator bool(IfElse input)
        {
            return input.IsTrue;
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator IfElse(bool input)
        {
            return new IfElse(input);
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator IfElse(Func<bool> input)
        {
            return new IfElse(input);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IfElse If(Action action)
        {
            Guard.IsNotNull(action, nameof(action));

            if (IsTrue)
                action();

            return this;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IfElse<TOut> If<TOut>(Func<TOut> factory)
        {
            Guard.IsNotNull(factory, nameof(factory));

            if (IsTrue)
                factory();

            return IfElse<TOut>.None;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IfElse Else(Action action)
        {
            Guard.IsNotNull(action, nameof(action));

            if (IsFalse)
                action();

            return this;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IfElse<TOut> Else<TOut>(Func<TOut> factory)
        {
            Guard.IsNotNull(factory, nameof(factory));

            if (IsFalse)
                return factory();

            return IfElse<TOut>.None;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IfElse Match(Action If, Action Else)
        {
            Guard.IsNotNull(If, nameof(If));
            Guard.IsNotNull(Else, nameof(Else));

            if (IsTrue)
                If();
            else
                Else();

            return this;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IfElse<TOut> Match<TOut>(Func<TOut> If, Func<TOut> Else)
        {
            Guard.IsNotNull(predicate, nameof(predicate));
            Guard.IsNotNull(If, nameof(If));
            Guard.IsNotNull(Else, nameof(Else));

            if (IsTrue)
                return If();
            else
                return Else();
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Either<L, R> Match<L, R>(Func<R> If, Func<L> Else)
        {
            Guard.IsNotNull(If, nameof(If));
            Guard.IsNotNull(Else, nameof(Else));

            L? left = default;
            R? right = default;

            if (IsTrue)
                right = If();
            else
                left = Else();

            return (left, right);
        }
    }
    public
#if !UNITY_2017_1_OR_NEWER
        readonly 
#endif
        partial struct IfElse<T>
    {
        public readonly static IfElse<T> None = default!;

        private readonly Predicate<T>? predicate;

#if UNITY_2017_1_OR_NEWER
        [field: UnityEngine.SerializeField]
        private T target;

        [field: UnityEngine.SerializeField]
        public bool IsSome { get; private set; }
#else
        private readonly T target;

        public readonly bool IsSome { get; }
#endif

        public readonly bool IsNone => !IsSome;
        public readonly bool IsTrue => IsSome && (predicate?.Invoke(target) ?? true);
        public readonly bool IsFalse => !IsTrue;
        public readonly T? Target => target;
        public readonly Predicate<T>? Predicate => predicate;

        public IfElse(T? value, Predicate<T>? predicate)
        {
            target = value!;

            IsSome = value.IsNotDefault();
            this.predicate = predicate;
        }

        public IfElse(T? value)
            :
            this(value, predicate: null)
        {
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator IfElse<T>(T? input)
        {
            return new IfElse<T>(input);
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator IfElse<T>((T? value, Predicate<T>? predicate) input) 
        {
            return new IfElse<T>(input.value, input.predicate);
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator T?(IfElse<T> input)
        {
            return input.target;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly IfElse<T> If(Action<T> action, Predicate<T>? predicate = null)
        {
            Guard.IsNotNull(action, nameof(action));

            if (IsTrue && (predicate?.Invoke(target) ?? true))
                action(target);

            return this;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly IfElse<TOut> If<TOut>(Func<T, TOut> selector, Predicate<T>? predicate = null)
        {
            Guard.IsNotNull(predicate, nameof(predicate));
            Guard.IsNotNull(selector, nameof(selector));

            if (IsTrue && (predicate?.Invoke(target) ?? true))
                selector(target);

            return IfElse<TOut>.None;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly IfElse<T> Else(Action action,
            Func<bool>? predicate = null)
        {
            Guard.IsNotNull(action, nameof(action));

            if (IsFalse && (predicate?.Invoke() ?? true))
                action();

            return this;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly IfElse<TOut> Else<TOut>(Func<TOut> factory,
            Func<bool>? predicate = null)
        {
            Guard.IsNotNull(factory, nameof(factory));

            if (IsFalse && (predicate?.Invoke() ?? true))
                return factory();

            return IfElse<TOut>.None;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly IfElse<T> Match(Action<T> If,
            Action Else,
            Predicate<T>? ifPredicate = null,
            Func<bool>? elsePredciate = null)
        {
            Guard.IsNotNull(predicate, nameof(predicate));
            Guard.IsNotNull(If, nameof(If));
            Guard.IsNotNull(Else, nameof(Else));

            if (IsTrue && (ifPredicate?.Invoke(target) ?? true))
                If(target);
            else if (elsePredciate?.Invoke() ?? true)
                Else();

            return this;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly IfElse<TOut> Match<TOut>(Func<T, TOut> If,
            Func<TOut> Else,
            Predicate<T>? ifPredicate = null,
            Func<bool>? elsePredciate = null)
        {
            Guard.IsNotNull(predicate, nameof(predicate));
            Guard.IsNotNull(If, nameof(If));
            Guard.IsNotNull(Else, nameof(Else));

            if (IsTrue && (ifPredicate?.Invoke(target) ?? true))
                return If(target);
            else if (elsePredciate?.Invoke() ?? true)
                return Else();

            return IfElse<TOut>.None;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly Either<L ,R> Match<L, R>(Func<T, R> If,
            Func<L> Else,
            Predicate<T>? ifPredicate = null,
            Func<bool>? elsePredciate = null)
        {
            Guard.IsNotNull(ifPredicate, nameof(ifPredicate));
            Guard.IsNotNull(If, nameof(If));
            Guard.IsNotNull(Else, nameof(Else));

            L? left = default;
            R? right = default;

            if (IsTrue && (ifPredicate?.Invoke(target) ?? true))
                right = If(target);
            else if (elsePredciate?.Invoke() ?? true)
                left = Else();

            return (left, right);
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly Maybe<T> Maybe() => target;

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly Catched<T> Catch() => target;

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly Either<T, R> Either<R>(R? right) => (target, right);

        public readonly IEnumerator<T> GetEnumerator()
        {
            if (IsFalse)
                yield break;

            yield return target;
        }

        readonly IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
