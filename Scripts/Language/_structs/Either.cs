#nullable enable
using System;

namespace CCEnvs
{
    public readonly struct Either<L, R> : IEquatable<Either<L, R>>
    {
        private readonly Predicate<R> predicate;

        public L Left { get; }
        public R Right { get; }

        public bool IsLeft => !IsRight;
        public bool IsRight => predicate(Right);

        public Either(L left, R right, Predicate<R> predicate)
        {
            Left = left;
            Right = right;
            this.predicate = predicate;
        }

        public static bool operator ==(Either<L, R> left, Either<L, R> right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Either<L, R> left, Either<L, R> right)
        {
            return !(left == right);
        }

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
            return HashCode.Combine(predicate, Left, Right);
        }
    }
}
