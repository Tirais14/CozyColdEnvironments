using CCEnvs.Diagnostics;
using CCEnvs.TypeMatching;
using CommunityToolkit.Diagnostics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using static UnityEditorInternal.ReorderableList;

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

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly R? GetRightValue() => right;

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly R GetRightValue(R @default)
        {
            if (IsRight)
                return right!;

            return @default;
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly R GetRightValue(Func<R> factory)
        {
            Guard.IsNotNull(factory);

            if (IsRight)
                return right!;

            return factory();
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly R GetRightValueUnsafe()
        {
            if (!IsRight)
                throw new ValueIsNoneException();

            return right!;
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly R GetRightValueUnsafe(Func<Exception> exceptionFactory)
        {
            Guard.IsNotNull(exceptionFactory);

            if (!IsLeft)
                throw exceptionFactory();

            return right!;
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly bool TryGetRightValue(out R right)
        {
            right = this.right;
            return IsRight;
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly L? GetLeftValue() => left;

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly L GetLeftValue(L @default)
        {
            if (IsLeft)
                return left!;

            return @default;
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly L GetLeftValue(Func<L> factory)
        {
            Guard.IsNotNull(factory);

            if (IsLeft)
                return left!;

            return factory();
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly L GetLeftValueUnsafe()
        {
            if (!IsLeft)
                throw new ValueIsNoneException();

            return left!;
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly L GetLeftValueUnsafe(Func<Exception> exceptionFactory)
        {
            Guard.IsNotNull(exceptionFactory);

            if (!IsLeft)
                throw exceptionFactory();

            return left!;
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly bool TryGetLeftValue(out R right)
        {
            right = this.right;
            return IsRight;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly object? GetValue()
        {
            if (IsRight)
                return right;
            else if (IsLeft)
                return left;

            return null;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly T GetValue<T>(T @default)
        {
            if (IsRight && right.Is<T>(out var r))
                return r;
            else if (IsLeft && left.Is<T>(out var l))
                return l;

            return @default;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly T GetValue<T>(Func<T> factory)
        {
            Guard.IsNotNull(factory);

            if (IsRight && right.Is<T>(out var r))
                return r;
            else if (IsLeft && left.Is<T>(out var l))
                return l;

            return factory();
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly bool TryGetValue<T>([NotNullWhen(true)] out object? result)
        {
            result = GetValue();
            return IsRight || IsLeft;
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly bool TryGetValue<T>([NotNullWhen(true)] out T? result)
        {
            var x = GetValue().As<T>();
            var state = x.IsSome;
            result = x.Raw;

            return state;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly T? GetValueUnsafe<T>()
        {
            var x = GetValue();

            if (x.IsNull())
                return default;

            return x.To<T>();
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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly TOut Match<TOut>(Func<R, TOut> Right, Func<L, TOut> Left, Func<TOut> Other)
        {
            Guard.IsNotNull(Right);
            Guard.IsNotNull(Left);
            Guard.IsNotNull(Other);

            if (IsRight)
                return Right(right);
            else if (IsLeft)
                return Left(left);

            return Other();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly TOut MatchUnsafe<TOut>(Func<R, TOut> Right, Func<L, TOut> Left)
        {
            Guard.IsNotNull(Right);
            Guard.IsNotNull(Left);

            if (IsRight)
                return Right(right);
            else if (IsLeft)
                return Left(left);

            throw new ValueIsNoneException();
        }

        public readonly Either<LOut, ROut> Cast<LOut, ROut>()
        {
            return new Either<LOut, ROut>(
                (LOut?)left.As<LOut>(),
                (ROut?)right.As<ROut>()
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
