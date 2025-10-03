#nullable enable
using CCEnvs.Language;
using System;
using System.Collections.Generic;

namespace CCEnvs
{
    public static class EitherExtensions
    {
        public static Either<L, R> ToEither<L, R>(this L left, R right)
        {
            return new Either<L, R>(left, right);
        }
        public static Either<L, R> ToEither<L, R>(this L left, R right, Func<bool> leftIsValid)
        {
            return new Either<L, R>(left, right, leftIsValid);
        }
        public static Either<L, R> ToEither<L, R>(this (L left, R right) source)
        {
            return new Either<L, R>(source.left, source.right);
        }
        public static Either<L, R> ToEither<L, R>(this (L left, R right) source, Func<bool> leftIsValid)
        {
            return new Either<L, R>(source.left, source.right, leftIsValid);
        }
        public static Either<L, R> ToEither<L, R>(this KeyValuePair<L, R> source)
        {
            return new Either<L, R>(source.Key, source.Value);
        }
        public static Either<L, R> ToEither<L, R>(this KeyValuePair<L, R> source, Func<bool> predicate)
        {
            return new Either<L, R>(source.Key, source.Value, predicate);
        }
    }
}
