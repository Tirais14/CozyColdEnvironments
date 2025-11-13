using CCEnvs.Unity;
using CommunityToolkit.Diagnostics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using static UnityEditor.Experimental.GraphView.GraphView;

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
        public readonly L? LeftTarget => left;
        public readonly R? RightTarget => right;

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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly L GetLeftValue(L defaultValue)
        {
            Guard.IsNotNull(defaultValue, nameof(defaultValue));

            if (IsNotLeft)
                return defaultValue;

            return left;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly bool TryGetLeftValue([NotNullWhen(true)] out L? left)
        {
            left = this.left;

            return IsLeft;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly R GetValueRight(R defaultValue)
        {
            Guard.IsNotNull(defaultValue, nameof(defaultValue));

            if (IsNotRight)
                return defaultValue;

            return right;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly bool TryGetRightValue([NotNullWhen(true)] out R? right)
        {
            right = this.right;

            return IsRight;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly object GetValueUnsafe()
        {
            if (IsRight)
                return right!;

            if (IsLeft)
                return left!;

            throw new ValueIsNoneException();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly T GetValueUnsafe<T>()
        {
            return GetValueUnsafe().As<T>();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly object? GetValue()
        {
            if (IsRight)
                return right;

            if (IsLeft)
                return left;

            return null;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly T? GetValue<T>() => (T?)GetValue();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly object? GetValue(L leftDefault, R rightDefault)
        {
            if (IsRight)
                return GetValueRight(rightDefault);

            if (IsLeft)
                return GetLeftValue(leftDefault);

            return null;
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

        public readonly Either<LOut, R> SelectLeft<LOut>(Func<L, LOut> selector)
        {
            Guard.IsNotNull(selector, nameof(selector));

            if (IsNotLeft)
                return (default, right);

            return (selector(left), right);
        }

        public readonly Either<LOut, ROut> SelectPair<LOut, ROut>(Func<(Maybe<L>, Maybe<R>), (LOut left, ROut right)> selector)
        {
            Guard.IsNotNull(selector, nameof(selector));

            var result = selector((left, right));

            return (result.left, result.right);
        }

        public readonly Either<L, ROut> Select<ROut>(Func<R, ROut> selector)
        {
            Guard.IsNotNull(selector, nameof(selector));

            if (IsNotRight)
                return (left, default);

            return (left, selector(right));
        }

        public readonly Either<L, R> Where(Predicate<R> predicate)
        {
            Guard.IsNotNull(predicate, nameof(predicate));

            if (IsRight && predicate(right))
                return this;

            return None;
        }

        public readonly Either<L, R> WhereLeft(Predicate<L> predicate)
        {
            Guard.IsNotNull(predicate, nameof(predicate));

            if (IsLeft && predicate(left))
                return this;

            return None;
        }

        public readonly bool Equals(Either<L, R> other)
        {
            return IsLeft
                   && 
                   IsRight
                   &&
                   EqualityComparer<L?>.Default.Equals(left, other.left)
                   &&
                   EqualityComparer<R?>.Default.Equals(right, other.right);
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
