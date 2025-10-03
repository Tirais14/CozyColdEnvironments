#nullable enable
using System;

namespace CCEnvs.Language
{
    public static class Either
    {
        public static Either<L, R> Create<L, R>(Func<L> leftGetter, Func<R> rightGetter, Func<bool> leftIsValid)
        {
            return new Either<L, R>(leftGetter, rightGetter, leftIsValid);
        }

        public static Either<L, R> Create<L, R>(Func<L> leftGetter, Func<R> rightGetter)
        {
            return new Either<L, R>(leftGetter, rightGetter);
        }

        public static Either<L, R> Create<L, R>(L left, Func<R> rightGetter, Func<bool> leftIsValid)
        {
            return new Either<L, R>(left, rightGetter, leftIsValid);
        }

        public static Either<L, R> Create<L, R>(L left, Func<R> rightGetter)
        {
            return new Either<L, R>(left, rightGetter);
        }

        public static Either<L, R> Create<L, R>(Func<L> leftGetter, R right, Func<bool> leftIsValid)
        {
            return new Either<L, R>(leftGetter, right, leftIsValid);
        }

        public static Either<L, R> Create<L, R>(Func<L> leftGetter, R right)
        {
            return new Either<L, R>(leftGetter, right);
        }

        public static Either<L, R> Create<L, R>(L left, R right)
        {
            return new Either<L, R>(left, right);
        }

        public static Either<L, R> Create<L, R>(L left, R right, Func<bool> leftIsValid)
        {
            return new Either<L, R>(left, right, leftIsValid);
        }
    }
    public readonly struct Either<L, R> : IEquatable<Either<L, R>>
    {
        private readonly Func<L> leftGetter;
        private readonly Func<R> rightGetter;
        private readonly Func<bool> leftIsValid;

        public L Left => leftGetter();
        public R Right => rightGetter();

        public bool IsLeft => leftIsValid();
        public bool IsRight => !IsLeft;

        public Either(Func<L> leftGetter, Func<R> rightGetter, Func<bool> leftIsValid)
        {
            this.leftGetter = leftGetter;
            this.rightGetter = rightGetter;
            this.leftIsValid = leftIsValid;
        }

        public Either(Func<L> leftGetter, Func<R> rightGetter)
            :
            this(leftGetter, rightGetter, () => IsValidLeftAction(leftGetter))
        {
        }

        public Either(L left, Func<R> rightGetter, Func<bool> leftIsValid)
            :
            this(() => left, rightGetter, leftIsValid)
        {
        }

        public Either(L left, Func<R> rightGetter)
            :
            this(() => left, rightGetter, () => IsValidLeftAction(left))
        {
        }

        public Either(Func<L> leftGetter, R right, Func<bool> leftIsValid)
            :
            this(leftGetter, () => right, leftIsValid)
        {
        }

        public Either(Func<L> leftGetter, R right)
            :
            this(leftGetter, () => right, () => IsValidLeftAction(leftGetter))
        {
        }

        public Either(L left, R right)
            :
            this(() => left, () => right)
        {
        }

        public Either(L left, R right, Func<bool> leftIsValid)
            :
            this(left, right)
        {
            this.leftIsValid = leftIsValid;
        }

        public static bool operator ==(Either<L, R> left, Either<L, R> right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Either<L, R> left, Either<L, R> right)
        {
            return !(left == right);
        }

        private static bool IsValidLeftAction(Func<L> leftGetter)
        {
            L left = leftGetter();

            if (left is IOptional optional)
                return optional.HasValue;

            return left.IsNotDefault();
        }

        private static bool IsValidLeftAction(L left)
        {
            if (left is IOptional optional)
                return optional.HasValue;

            return left.IsNotDefault();
        }

        public object Resolve() => IsLeft ? Left! : Right!;
        public T Resolve<T>() => IsLeft ? Left.As<T>() : Right.As<T>();

        public bool Equals(Either<L, R> other)
        {
            return leftGetter == other.leftGetter
                   &&
                   rightGetter == other.rightGetter
                   &&
                   leftIsValid == other.leftIsValid;
        }
        public override bool Equals(object obj)
        {
            return obj is Either<L, R> typed && Equals(typed);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(leftGetter, rightGetter, leftIsValid);
        }
    }
}
