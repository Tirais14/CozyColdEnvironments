using CommunityToolkit.Diagnostics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;

#nullable enable
#pragma warning disable S3236
namespace CCEnvs.FuncLanguage
{
    public
#if !UNITY_2017_1_OR_NEWER
        readonly 
#endif
        partial struct Either<L, R> : IEquatable<Either<L, R>>
    {
        public readonly static Either<L, R> None = default;

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

        public readonly bool IsNotLeft => !IsLeft;

        public readonly bool IsNotRight => !IsRight; 

        public Either(L? left, R? right)
        {
            this.left = left!;
            this.right = right!;

            IsLeft = left.IsNotDefault();
            IsRight = right.IsNotDefault();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator Either<L, R>((L? left, R? right) input)
        {
            return new Either<L, R>(input.left, input.right);
        }

        public static explicit operator L(Either<L, R> input)
        {
            return input.left;
        }

        public static explicit operator R(Either<L, R> input)
        {
            return input.right;
        }

        public static explicit operator (L, R)(Either<L, R> input)
        {
            return (input.left, input.right);
        }

        public static bool operator ==(Either<L, R> left, Either<L, R> right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Either<L, R> left, Either<L, R> right)
        {
            return !(left == right);
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly L? AccessLeft() => left;

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly L AccessLeft(L defaultValue)
        {
            Guard.IsNotNull(defaultValue, nameof(defaultValue));

            if (IsNotLeft)
                return defaultValue;

            return left;
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly R? AccessRight() => right;

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly R AccessRight(R defaultValue)
        {
            Guard.IsNotNull(defaultValue, nameof(defaultValue));

            if (IsNotRight)
                return defaultValue;

            return right;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly Either<L, R> IfRight(Action<R> action)
        {
            Guard.IsNotNull(action, nameof(action));

            if (IsRight)
                action(right);

            return this;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly Either<L, ROut> IfRight<ROut>(Func<R, ROut> selector)
        {
            Guard.IsNotNull(selector, nameof(selector));

            return (left, IsRight ? selector(right) : default);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly Either<L, R> IfLeft(Action<L> action)
        {
            Guard.IsNotNull(action, nameof(action));

            if (IsLeft)
                action(left);

            return this;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly Either<LOut, R> IfLeft<LOut>(Func<L, LOut> selector)
        {
            Guard.IsNotNull(selector, nameof(selector));

            return (IsLeft ? selector(left) : default, right);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly Either<L, R> Match(Action<R> Right, Action<L> Left)
        {
            Guard.IsNotNull(Right, nameof(Right));
            Guard.IsNotNull(Left, nameof(Left));

            if (IsRight)
                Right(right);

            if (IsLeft)
                Left(left);

            return this;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly Either<LOut, ROut> Match<LOut, ROut>(Func<R, ROut> Right, Func<L, LOut> Left)
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

        public readonly Either<LOut, ROut> Cast<LOut, ROut>()
        {
            return new Either<LOut, ROut>(
                (LOut?)left.AsOrDefault<LOut>(),
                (ROut?)right.AsOrDefault<ROut>()
                );
        }

        public readonly bool Equals(Either<L, R> other)
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
            return obj is Either<L, R> typed && Equals(typed);
        }

        public readonly override int GetHashCode()
        {
            return HashCode.Combine(left, right, IsLeft, IsRight);
        }
    }
}
