#nullable enable
using System;

namespace CCEnvs
{
    public readonly struct Either<T> : IEquatable<Either<T>>
    {
        private readonly Predicate<T> chooseRight;

        public T Left { get; }
        public T Right { get; }

        public bool IsLeft => !IsRight;
        public bool IsRight => chooseRight(Right);
        public T Resolved => IsRight ? Right! : Left!;

        public Either(T left, T right)
        {
            Left = left;
            Right = right;
            chooseRight = (x) => x.IsNotDefault();
        }

        public Either(T left, T right, Predicate<T> chooseRight)
            :
            this(left, right)
        {
            this.chooseRight = chooseRight;
        }

        public static bool operator ==(Either<T> left, Either<T> right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Either<T> left, Either<T> right)
        {
            return !(left == right);
        }

        public bool Equals(Either<T> other)
        {
            throw new NotImplementedException();
        }
        public override bool Equals(object obj)
        {
            return obj is Either<T> typed && Equals(typed);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(chooseRight, Left, Right);
        }
    }
    public readonly struct Either<L, R> : IEquatable<Either<L, R>>
    {
        private readonly Predicate<R> chooseRight;

        public L Left { get; }
        public R Right { get; }

        public bool IsLeft => !IsRight;
        public bool IsRight => chooseRight(Right);
        public object Resolved => IsRight ? Right! : Left!;

        public Either(L left, R right)
        {
            Left = left;
            Right = right;
            chooseRight = (x) => x.IsNotDefault();
        }

        public Either(L left, R right, Predicate<R> chooseRight)
            :
            this(left, right)
        {
            this.chooseRight = chooseRight;
        }

        public static bool operator ==(Either<L, R> left, Either<L, R> right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Either<L, R> left, Either<L, R> right)
        {
            return !(left == right);
        }

        public T Resolve<T>() => Resolved.As<T>();

        public bool Equals(Either<L, R> other)
        {
            throw new NotImplementedException();
        }
        public override bool Equals(object obj)
        {
            return obj is Either<L, R> typed && Equals(typed);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(chooseRight, Left, Right);
        }
    }
}
