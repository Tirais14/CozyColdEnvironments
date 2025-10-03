#nullable enable
using System;

namespace CCEnvs.Language
{
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
            this(leftGetter, rightGetter, () => leftGetter().IsNotDefault())
        {
        }

        public Either(L left, Func<R> rightGetter)
            :
            this(() => left, rightGetter, () => left.IsNotDefault())
        {
        }

        public Either(Func<L> leftGetter, R right)
            :
            this(leftGetter, () => right, () => leftGetter().IsNotDefault())
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
