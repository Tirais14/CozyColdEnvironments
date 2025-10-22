using CommunityToolkit.Diagnostics;
using System;

#nullable enable
namespace CCEnvs.Language
{
    [Obsolete("In process", true)]
    public readonly struct Ways<L, R> : IEquatable<Ways<L, R>>
    {
        private readonly LeftWay<L> left;
        private readonly RightWay<R> right;

        public Ghost<L> Left => left.value;
        public Ghost<R> Right => right.value;

        public Ways(L? left, R? right)
        {
            this.left = left!;
            this.right = right!; 
        }

        public static implicit operator Ways<L, R>((L? left, R? right) source)
        {
            return new Ways<L, R>(source.left, source.right);
        }

        public static implicit operator Ways<L, R>((LeftWay<L> left, RightWay<R> right) source)
        {
            return new Ways<L, R>(source.left, source.right);
        }

        public static implicit operator Ways<L, R>(LeftWay<L> left)
        {
            return new Ways<L, R>(left.value.Value(), default);
        }

        public static implicit operator Ways<L, R>(RightWay<R> right)
        {
            return new Ways<L, R>(default, right.value.Value());
        }

        public Ways<L, R> IfLeft(Action<L> onLeft)
        {
            Left.IfSome(onLeft);

            return this;
        }
        public Ways<LOut, R> IfLeft<LOut>(Func<L, LOut> onLeft)
        {
            return new Ways<LOut, R>(Left.IfSome(
                x => onLeft(x)).Value(),
                Right.Value()
                );
        }

        public Ways<L, R> IfRight(Action<R> onRight)
        {
            Right.IfSome(onRight);

            return this;
        }
        public Ways<L, ROut> IfRight<ROut>(Func<R, ROut> onRight)
        {
            return new Ways<L, ROut>(
                Left.Value(),
                Right.IfSome(x => onRight(x)).Value()
                );
        }

        public Ways<LOut, ROut> Map<LOut, ROut>(
            Func<L, LOut> onLeft,
            Func<R, ROut> onRight)
        {
            Guard.IsNotNull(onLeft, nameof(onLeft));
            Guard.IsNotNull(onRight, nameof(onRight));

            return new Ways<LOut, ROut>(
                Left.IfSome(x => onLeft(x)).Value(),
                Right.IfSome(x => onRight(x)).Value()
                );
        }

        public Ways<L, R> Match(Action<L> onLeft, Action<R> onRight)
        {
            IfLeft(onLeft);
            IfRight(onRight);

            return this;
        }

        public bool Equals(Ways<L, R> other)
        {
            return left == other.left && right == other.right;
        }

        public override bool Equals(object obj)
        {
            return obj is Ways<L, R> typed && Equals(typed);
        }
    }
}
