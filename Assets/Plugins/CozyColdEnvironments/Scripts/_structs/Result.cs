using CCEnvs.FuncLanguage;
using CCEnvs.Reflection;
using CCEnvs.Reflection.Caching;
using CommunityToolkit.Diagnostics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;

#nullable enable
namespace CCEnvs
{
    public static class Result
    {
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Result<T> Exception<T>(Exception exception)
        {
            return new Result<T>(exception);
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Result<T> Value<T>(T value)
        {
            return new Result<T>(value);
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Result<T> Lazy<T>(
            Func<object?, T?> valueFactory,
            Func<object?, Exception?> exceptionFactory,
            object? valueFactoryState = null,
            object? exceptionFactoryState = null
            )
        {
            return new Result<T>(
                valueFactory,
                exceptionFactory,
                valueFactoryState,
                exceptionFactoryState
                );
        }
    }

    public struct Result<T> : IEquatable<Result<T>>
    {
        public readonly static Result<T> Empty = new();

        private readonly Func<object?, T?>? valueFactory;
        private readonly Func<object?, Exception?>? exceptionFactory;

        private readonly object? valueFactoryState;
        private readonly object? exceptionFactoryState;

        private T? value;
        private Exception? exception;

        private bool valueCreated;
        private bool exceptionCreated;

        public readonly T? Raw => value;

        public Result(T value)
            :
            this()
        {
            CC.Guard.IsNotNull(value);

            this.value = value;

            valueCreated = true;
            exceptionCreated = true;
        }

        public Result(Exception exception)
            :
            this()
        {
            CC.Guard.IsNotNull(exception, nameof(exception));

            this.exception = exception;

            valueCreated = true;
            exceptionCreated = true;
        }

        public Result(
            Func<object?, T?> valueFactory,
            Func<object?, Exception?> exceptionFactory,
            object? valueFactoryState = null,
            object? exceptionFactoryState = null
            )
            :
            this()
        {
            Guard.IsNotNull(valueFactory);
            Guard.IsNotNull(exceptionFactory);

            this.valueFactory = valueFactory;
            this.exceptionFactory = exceptionFactory;
            this.valueFactoryState = valueFactoryState;
            this.exceptionFactoryState = exceptionFactoryState;
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator Result<T>((T? value, Exception? exception) input)
        {
            if (input.value.IsNotNull())
                return new Result<T>(input.value!);

            if (input.exception is not null)
                return new Result<T>(input.exception!);

            throw new ArgumentException("Value and exception cannot be null");
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator Maybe<T>(Result<T> source)
        {
            return source.Lax();
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static explicit operator T(Result<T> source)
        {
            return source.Strict();
        }

        public static bool operator ==(Result<T> left, Result<T> right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Result<T> left, Result<T> right)
        {
            return !(left == right);
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Maybe<T> Lax() => GetValue();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T Strict()
        {
            var value = GetValue();

            if (value.IsNull())
                ThrowException();

            return value!;
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly Result<TOut> Cast<TOut>()
        {
            if (value.IsNotNull())
                return new Result<TOut>(value.CastTo<TOut>());

            if (exception is not null)
                return new Result<TOut>(exception);

            if (exceptionFactory is not null && valueFactory is not null)
            {
                return new Result<TOut>((argsUntyped) =>
                {
                    var args = argsUntyped.CastTo<(object? valueFactoryState, Func<object?, T> valueFactory)>();

                    return args.valueFactory.Invoke(args.valueFactoryState).CastTo<TOut>();
                },
                exceptionFactory,
                (valueFactoryState, valueFactory),
                exceptionFactoryState
                );
            }

            throw new InvalidOperationException();
        }

        public readonly override string ToString()
        {
            if (this == default)
                return TypeCache<Result<T>>.FullName;

            return new ToStringBuilder(null)
                .Add(nameof(valueFactory), value)
                .Add(nameof(exceptionFactory), exceptionFactory)
                .Add(nameof(value), value)
                .Add(nameof(exception), exception)
                .Add(nameof(valueCreated), valueCreated)
                .Add(nameof(exceptionCreated), nameof(exceptionCreated))
                .ToStringAndDispose();
        }

        public readonly override bool Equals(object? obj)
        {
            return obj is Result<T> result && Equals(result);
        }

        public readonly bool Equals(Result<T> other)
        {
            return EqualityComparer<Func<object?, T?>?>.Default.Equals(valueFactory, other.valueFactory)
                   &&
                   EqualityComparer<Func<object?, Exception?>?>.Default.Equals(exceptionFactory, other.exceptionFactory)
                   &&
                   EqualityComparer<object?>.Default.Equals(valueFactoryState, other.valueFactoryState)
                   &&
                   EqualityComparer<object?>.Default.Equals(exceptionFactoryState, other.exceptionFactoryState)
                   &&
                   EqualityComparer<T?>.Default.Equals(value, other.value)
                   &&
                   EqualityComparer<Exception?>.Default.Equals(exception, other.exception)
                   &&
                   valueCreated == other.valueCreated
                   &&
                   exceptionCreated == other.exceptionCreated;
        }

        public readonly override int GetHashCode()
        {
            return HashCode.Combine(
                valueFactory,
                exceptionFactory,
                valueFactoryState,
                exceptionFactoryState,
                value,
                exception,
                valueCreated,
                exceptionCreated
                );
        }

        private T? GetValue()
        {
            if (!valueCreated)
            {
                if (valueFactory is null)
                    throw new InvalidOperationException("Not found value factory");

                value = valueFactory(valueFactoryState);
                valueCreated = true;
            }

            return value;
        }

        private void ThrowException()
        {
            if (!exceptionCreated)
            {
                if (exceptionFactory is null)
                    throw new InvalidOperationException("Not found exception factory");

                exception = exceptionFactory(exceptionFactoryState);
                exceptionCreated = true;
            }

            if (exception is null)
                throw new InvalidOperationException("Not found exception");

            throw exception;
        }
    }
}
