#nullable enable
using System;
using System.Collections.Generic;

namespace CCEnvs
{
    public static class EitherExtensions
    {
        public static Either<T> ToEitherSingleTyped<T>(this T right, T left)
        {
            return new Either<T>(left, right);
        }
        public static Either<T> ToEitherSingleTyped<T>(this T right, T left, Predicate<T> predicate)
        {
            return new Either<T>(left, right, predicate);
        }
        public static Either<T> ToEitherSingleTyped<T>(this (T left, T right) source)
        {
            return new Either<T>(source.left, source.right);
        }
        public static Either<T> ToEitherSingleTyped<T>(this (T left, T right) source, Predicate<T> predicate)
        {
            return new Either<T>(source.left, source.right, predicate);
        }
        public static Either<T> ToEitherSingleTyped<T>(this KeyValuePair<T, T> source)
        {
            return new Either<T>(source.Key, source.Value);
        }
        public static Either<T> ToEitherSingleTyped<T>(this KeyValuePair<T, T> source, Predicate<T> predicate)
        {
            return new Either<T>(source.Key, source.Value, predicate);
        }

        public static Either<L, R> ToEither<L, R>(this R right, L left)
        {
            return new Either<L, R>(left, right);
        }
        public static Either<L, R> ToEither<L, R>(this R right, L left, Predicate<R> predicate)
        {
            return new Either<L, R>(left, right, predicate);
        }
        public static Either<L, R> ToEither<L, R>(this (L left, R right) source)
        {
            return new Either<L, R>(source.left, source.right);
        }
        public static Either<L, R> ToEither<L, R>(this (L left, R right) source, Predicate<R> predicate)
        {
            return new Either<L, R>(source.left, source.right, predicate);
        }
        public static Either<L, R> ToEither<L, R>(this KeyValuePair<L, R> source)
        {
            return new Either<L, R>(source.Key, source.Value);
        }
        public static Either<L, R> ToEither<L, R>(this KeyValuePair<L, R> source, Predicate<R> predicate)
        {
            return new Either<L, R>(source.Key, source.Value, predicate);
        }
    }
}
