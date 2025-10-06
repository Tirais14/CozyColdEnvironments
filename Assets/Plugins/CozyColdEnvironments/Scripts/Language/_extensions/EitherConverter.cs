#nullable enable
using System;
using System.Reflection;

namespace CCEnvs.Language
{
    public static class EitherConverter
    {
        public static Either<L, R> Either<L, R>(this Func<L> leftGetter,
                                                  Func<R> rightGetter,
                                                  Func<bool> leftIsValid)
        {
            return new Either<L, R>(leftGetter, rightGetter, leftIsValid);
        }
        public static Either<L, R> Either<L, R>(this Func<L> leftGetter,
                                                  Func<R> rightGetter)
        {
            return new Either<L, R>(leftGetter, rightGetter);
        }
        public static Either<L, R> Either<L, R>(this L left, Func<R> rightGetter, Func<bool> leftIsValid)
        {
            return new Either<L, R>(left, rightGetter, leftIsValid);
        }
        public static Either<L, R> Either<L, R>(this L left, Func<R> rightGetter)
        {
            return new Either<L, R>(left, rightGetter);
        }
        public static Either<L, R> Either<L, R>(this Func<L> leftGetter, R right, Func<bool> leftIsValid)
        {
            return new Either<L, R>(leftGetter, right, leftIsValid);
        }
        public static Either<L, R> Either<L, R>(this Func<L> leftGetter, R right)
        {
            return new Either<L, R>(leftGetter, right);
        }
        public static Either<L, R> Either<L, R>(this L left, R right)
        {
            return new Either<L, R>(left, right);
        }
        public static Either<L, R> Either<L, R>(this L left,
                                                  R right,
                                                  Func<bool> leftIsValid)
        {
            return new Either<L, R>(left, right, leftIsValid);
        }

        public static Either<L, R> Either<T, L, R>(
            this T source,
            Func<T, L> leftGetter,
            Func<T, R> rightGetter,
            Func<T, bool> leftIsValid)
        {
            CC.Guard.NullArgument(leftGetter, nameof(leftGetter));
            CC.Guard.NullArgument(rightGetter, nameof(rightGetter));
            CC.Guard.NullArgument(leftIsValid, nameof(leftIsValid));

            return new Either<L, R>(
                () => leftGetter(source),
                () => rightGetter(source),
                () => leftIsValid(source));
        }
        public static Either<L, R> Either<T, L, R>(
            this T source,
            Func<T, L> leftGetter,
            Func<T, R> rightGetter)
        {
            return source.Either(leftGetter, rightGetter, (x) => x.IsNotDefault());
        }
    }
}
