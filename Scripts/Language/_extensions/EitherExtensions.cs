#nullable enable
using System;
using System.Collections.Generic;

namespace CCEnvs
{
    public static class EitherExtensions
    {
        public static Either<L, R> ToEither<L, R>(this R source, L alt, Predicate<R> predicate)
        {
            return new Either<L, R>(alt, source, predicate);
        }
        public static Either<L, R> ToEither<L, R>(this (L left, R right) source, Predicate<R> predicate)
        {
            return new Either<L, R>(source.left, source.right, predicate);
        }
        public static Either<L, R> ToEither<L, R>(this KeyValuePair<L, R> source, Predicate<R> predicate)
        {
            return new Either<L, R>(source.Key, source.Value, predicate);
        }
    }
}
