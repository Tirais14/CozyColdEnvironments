#nullable enable
using System;

namespace CCEnvs.Language
{
    public static class EitherConverter
    {
        public static Either<L, R> ToEither<L, R>(this Func<L> leftGetter,
                                                  Func<R> rightGetter,
                                                  Func<bool> leftIsValid)
        {
            return new Either<L, R>(leftGetter, rightGetter, leftIsValid);
        }
        public static Either<L, R> ToEither<L, R>(this Func<L> leftGetter,
                                                  Func<R> rightGetter)
        {
            return new Either<L, R>(leftGetter, rightGetter);
        }
        public static Either<L, R> ToEither<L, R>(this L left, Func<R> rightGetter, Func<bool> leftIsValid)
        {
            return new Either<L, R>(left, rightGetter, leftIsValid);
        }
        public static Either<L, R> ToEither<L, R>(this L left, Func<R> rightGetter)
        {
            return new Either<L, R>(left, rightGetter);
        }
        public static Either<L, R> ToEither<L, R>(this Func<L> leftGetter, R right, Func<bool> leftIsValid)
        {
            return new Either<L, R>(leftGetter, right, leftIsValid);
        }
        public static Either<L, R> ToEither<L, R>(this Func<L> leftGetter, R right)
        {
            return new Either<L, R>(leftGetter, right);
        }
        public static Either<L, R> ToEither<L, R>(this L left, R right)
        {
            return new Either<L, R>(left, right);
        }
        public static Either<L, R> ToEither<L, R>(this L left,
                                                  R right,
                                                  Func<bool> leftIsValid)
        {
            return new Either<L, R>(left, right, leftIsValid);
        }
    }
}
