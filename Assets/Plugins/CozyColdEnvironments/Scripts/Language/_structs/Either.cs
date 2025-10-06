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

        public static bool IsItemValid<T>(Func<T> valueGetter)
        {
            T left = valueGetter();

            if (left is IOptional optional)
                return optional.HasValue;

            return left.IsNotDefault();
        }

        public static bool IsItemValid<T>(T value)
        {
            if (value is IOptional optional)
                return optional.HasValue;

            return value.IsNotDefault();
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
            this(leftGetter, rightGetter, () => Either.IsItemValid(leftGetter))
        {
        }

        public Either(L left, Func<R> rightGetter, Func<bool> leftIsValid)
            :
            this(() => left, rightGetter, leftIsValid)
        {
        }

        public Either(L left, Func<R> rightGetter)
            :
            this(() => left, rightGetter, () => Either.IsItemValid(left))
        {
        }

        public Either(Func<L> leftGetter, R right, Func<bool> leftIsValid)
            :
            this(leftGetter, () => right, leftIsValid)
        {
        }

        public Either(Func<L> leftGetter, R right)
            :
            this(leftGetter, () => right, () => Either.IsItemValid(leftGetter))
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

        public object Resolve() => IsLeft ? Left! : Right!;
        public T Resolve<T>() => IsLeft ? Left.As<T>() : Right.As<T>();

        public void Continue<T>(Action<T> action)
        {
            CC.Guard.NullArgument(action, nameof(action));

            action(Resolve<T>());
        }
        public TOut Continue<T, TOut>(Func<T, TOut> func)
        {
            CC.Guard.NullArgument(func, nameof(func));

            return func(Resolve<T>());
        }

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
