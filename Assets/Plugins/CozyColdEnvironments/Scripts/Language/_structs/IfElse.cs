using CommunityToolkit.Diagnostics;
using System;
using System.Collections;
using System.Collections.Generic;


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

        public static implicit operator bool(IfElse input)
        {
            return input.IsTrue;
        }

        public static implicit operator IfElse(bool input)
        {
            return new IfElse(input);
        }
        public static implicit operator IfElse(Func<bool> input)
        {
            return new IfElse(input);
        }

        public IfElse If(Action action)
        {
            Guard.IsNotNull(action, nameof(action));

            if (IsTrue)
                action();

            return this;
        }
        public IfElse<TOut> If<TOut>(Func<TOut> factory)
        {
            Guard.IsNotNull(factory, nameof(factory));

            if (IsTrue)
                factory();

            return IfElse<TOut>.None;
        }

        public IfElse Else(Action action)
        {
            Guard.IsNotNull(action, nameof(action));

            if (IsFalse)
                action();

            return this;
        }
        public IfElse<TOut> Else<TOut>(Func<TOut> factory)
        {
            Guard.IsNotNull(factory, nameof(factory));

            if (IsFalse)
                return factory();

            return IfElse<TOut>.None;
        }

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
        public Ways<L, R> Match<L, R>(Func<R> If, Func<L> Else)
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

#if UNITY_2017_1_OR_NEWER
        [field: UnityEngine.SerializeField]
        private T target;

        [field: UnityEngine.SerializeField]
        public bool IsSome { get; private set; }
#else
        private readonly T value;

        public readonly bool IsSome { get; }
#endif

        public readonly bool IsNone => !IsSome;

        public IfElse(T? value)
        {
            this.target = value!;

            IsSome = value.IsNotDefault();
        }

        public static implicit operator IfElse<T>(T? input)
        {
            return new IfElse<T>(input);
        }

        public static implicit operator T?(IfElse<T> input)
        {
            return input.target;
        }

        public readonly IfElse<T> If(Predicate<T> predicate, Action<T> action)
        {
            Guard.IsNotNull(predicate, nameof(predicate));
            Guard.IsNotNull(action, nameof(action));

            if (IsSome && predicate(target))
                action(target);

            return this;
        }
        public readonly IfElse<TOut> If<TOut>(Predicate<T> predicate, Func<T, TOut> selector)
        {
            Guard.IsNotNull(predicate, nameof(predicate));
            Guard.IsNotNull(selector, nameof(selector));

            if (IsSome && predicate(target))
                selector(target);

            return IfElse<TOut>.None;
        }

        public readonly IfElse<T> Else(Action action)
        {
            Guard.IsNotNull(action, nameof(action));

            if (IsSome)
                action();

            return this;
        }
        public readonly IfElse<TOut> Else<TOut>(Func<TOut> factory)
        {
            Guard.IsNotNull(factory, nameof(factory));

            if (IsSome)
                return factory();

            return IfElse<TOut>.None;
        }

        public readonly IfElse<T> Match(Predicate<T> predicate, Action<T> If, Action Else)
        {
            Guard.IsNotNull(predicate, nameof(predicate));
            Guard.IsNotNull(If, nameof(If));
            Guard.IsNotNull(Else, nameof(Else));

            if (IsSome && predicate(target))
                If(target);
            else
                Else();

            return this;
        }
        public readonly IfElse<TOut> Match<TOut>(Predicate<T> predicate, Func<T, TOut> If, Func<TOut> Else)
        {
            Guard.IsNotNull(predicate, nameof(predicate));
            Guard.IsNotNull(If, nameof(If));
            Guard.IsNotNull(Else, nameof(Else));

            if (IsSome && predicate(target))
                return If(target);
            else
                return Else();
        }
        public readonly Ways<L ,R> Match<L, R>(Predicate<T> predicate, Func<T, R> If, Func<L> Else)
        {
            Guard.IsNotNull(predicate, nameof(predicate));
            Guard.IsNotNull(If, nameof(If));
            Guard.IsNotNull(Else, nameof(Else));

            L? left = default;
            R? right = default;

            if (IsSome && predicate(target))
                right = If(target);
            else
                left = Else();

            return (left, right);
        }

        public readonly Maybe<T> Maybe() => target;

        public readonly Catched<T> Catch() => target;

        public readonly IEnumerator<T> GetEnumerator()
        {
            if (IsNone)
                yield break;

            yield return target;
        }

        readonly IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
