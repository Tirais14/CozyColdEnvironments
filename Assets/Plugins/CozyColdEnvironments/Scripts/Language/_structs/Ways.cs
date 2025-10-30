using CommunityToolkit.Diagnostics;
using System;
using System.Collections.Generic;

#nullable enable
namespace CCEnvs.FuncLanguage
{
    public
#if !UNITY_2017_1_OR_NEWER
        readonly 
#endif
        struct Ways<L, R> : IEquatable<Ways<L, R>>
    {
        private readonly L left;
        private readonly R right;

#if UNITY_2017_1_OR_NEWER
        [field: UnityEngine.SerializeField]
        public bool IsLeft { get; private set; }

        [field: UnityEngine.SerializeField]
        public bool IsRight { get; private set; }
#else
        public readonly bool IsLeft { get; }
        public readonly bool IsRight { get; }
#endif

        public Ways(L? left, R? right)
        {
            this.left = left!;
            this.right = right!;

            IsLeft = left.IsNotDefault();
            IsRight = right.IsNotDefault();
        }

        public static implicit operator Ways<L, R>((L? left, R? right) input)
        {
            return new Ways<L, R>(input.left, input.right);
        }

        public static explicit operator L(Ways<L, R> input)
        {
            return input.left;
        }

        public static explicit operator R(Ways<L, R> input)
        {
            return input.right;
        }

        public static explicit operator (L, R)(Ways<L, R> input)
        {
            return (input.left, input.right);
        }

        public static bool operator ==(Ways<L, R> left, Ways<L, R> right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Ways<L, R> left, Ways<L, R> right)
        {
            return !(left == right);
        }

        public readonly Maybe<R> IfRight(Action<R> action)
        {
            Guard.IsNotNull(action, nameof(action));

            if (IsRight)
                action(right);

            return Maybe<R>.None;
        }
        public readonly Maybe<ROut> IfRight<ROut>(Func<R, ROut> selector)
        {
            Guard.IsNotNull(selector, nameof(selector));

            if (IsRight)
                return selector(right);

            return Maybe<ROut>.None;
        }

        public readonly Maybe<L> IfLeft(Action<L> action)
        {
            Guard.IsNotNull(action, nameof(action));

            if (IsLeft)
                action(left);

            return Maybe<L>.None;
        }
        public readonly Maybe<LOut> IfLeft<LOut>(Func<L, LOut> selector)
        {
            Guard.IsNotNull(selector, nameof(selector));

            if (IsLeft)
                selector(left);

            return Maybe<LOut>.None;
        }

        public readonly Ways<L, R> Match(Action<R> Right, Action<L> Left)
        {
            Guard.IsNotNull(Right, nameof(Right));
            Guard.IsNotNull(Left, nameof(Left));

            if (IsRight)
                Right(right);

            if (IsLeft)
                Left(left);

            return this;
        }

        public readonly Ways<LOut, ROut> Match<LOut, ROut>(Func<R, ROut> Right, Func<L, LOut> Left)
        {
            Guard.IsNotNull(Right, nameof(Right));
            Guard.IsNotNull(Left, nameof(Left));

            LOut? lOut = default;
            ROut? rOut = default;

            if (IsRight)
                rOut = Right(right);

            if (IsLeft)
                lOut = Left(left);

            return (lOut, rOut);
        }

        public readonly bool Equals(Ways<L, R> other)
        {
            return IsLeft
                   && 
                   IsRight
                   &&
                   EqualityComparer<L?>.Default.Equals(left, other.left)
                   &&
                   EqualityComparer<R>.Default.Equals(right, other.right);
        }
        public readonly override bool Equals(object obj)
        {
            return obj is Ways<L, R> typed && Equals(typed);
        }

        public readonly override int GetHashCode()
        {
            return HashCode.Combine(left, right, IsLeft, IsRight);
        }
    }
}
