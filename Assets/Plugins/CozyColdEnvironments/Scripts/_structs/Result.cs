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
        public static Result<T> ExceptionLazy<T>(
            Func<object?, Exception?> exceptionFactory,
            object? exceptionFactoryState
            )
        {
            return new Result<T>(
                valueFactory: null,
                exceptionFactory,
                exceptionFactoryState: exceptionFactoryState
                );
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Result<T> ValueLazy<T>(
            Func<object?, T> valueFactory,
            object? valueFactoryState
            )
        {
            return new Result<T>(
                valueFactory: valueFactory,
                exceptionFactory: null,
                valueFactoryState: valueFactoryState
                );
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

    public struct Result<TValue> : IEquatable<Result<TValue>>
    {
        public readonly static Result<TValue> Empty = new();

        private TValue? value;
        private Exception? exception;

        public readonly TValue? Raw => value;

        public Result(TValue value)
            :
            this()
        {
            CC.Guard.IsNotNull(value);

            this.value = value;
        }

        public Result(Exception exception)
            :
            this()
        {
            CC.Guard.IsNotNull(exception, nameof(exception));

            this.exception = exception;
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator Result<TValue>((TValue? value, Exception? exception) input)
        {
            if (input.value.IsNotNull())
                return new Result<TValue>(input.value!);

            if (input.exception is not null)
                return new Result<TValue>(input.exception!);

            throw new ArgumentException("Value and exception cannot be null");
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator Maybe<TValue>(Result<TValue> source)
        {
            return source.Lax();
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static explicit operator TValue(Result<TValue> source)
        {
            return source.Strict();
        }

        public static bool operator ==(Result<TValue> left, Result<TValue> right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Result<TValue> left, Result<TValue> right)
        {
            return !(left == right);
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Maybe<TValue> Lax() => GetValue();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public TValue Strict()
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

            throw new InvalidOperationException();
        }

        public readonly override string ToString()
        {
            if (this == default)
                return TypeCache<Result<TValue>>.FullName;

            return new ToStringBuilder(null)
                .Add(nameof(value), value)
                .Add(nameof(exception), exception)
                .ToStringAndDispose();
        }

        public readonly override bool Equals(object? obj)
        {
            return obj is Result<TValue> result && Equals(result);
        }

        public readonly bool Equals(Result<TValue> other)
        {
            return EqualityComparer<TValue?>.Default.Equals(value, other.value)
                   &&
                   EqualityComparer<Exception?>.Default.Equals(exception, other.exception);
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

        private TValue? GetValue()
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

    public struct Result<TValue, TValueFactoryState, TExceptionFactoryState> : IEquatable<Result<TValue, TValueFactoryState, TExceptionFactoryState>>
    {
        public readonly static Result<TValue> Empty = new();

        private readonly Func<TValueFactoryState?, TValue>? valueFactory;
        private readonly Func<TExceptionFactoryState?, Exception>? exceptionFactory;

        private readonly TValueFactoryState? valueFactoryState;
        private readonly TExceptionFactoryState? exceptionFactoryState;

        private TValue? value;
        private Exception? exception;

        private bool valueCreated;
        private bool exceptionCreated;

        public readonly TValue? Raw => value;

        public Result(TValue value)
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
            Func<TValueFactoryState?, TValue> valueFactory,
            TValueFactoryState? valueFactoryState = default
            )
            :
            this()
        {
            Guard.IsNotNull(valueFactory);

            this.valueFactory = valueFactory!;
            this.valueFactoryState = valueFactoryState;
        }

        public Result(
            Func<TExceptionFactoryState?, Exception> exceptionFactory,
            TExceptionFactoryState? exceptionFactoryState = default
            )
            :
            this()
        {
            Guard.IsNotNull(exceptionFactory);

            this.exceptionFactory = exceptionFactory!;
            this.exceptionFactoryState = exceptionFactoryState;
        }

        public Result(
            Func<TValueFactoryState?, TValue> valueFactory,
            Func<TExceptionFactoryState?, Exception> exceptionFactory,
            TValueFactoryState? valueFactoryState = default,
            TExceptionFactoryState? exceptionFactoryState = default
            )
        {
            if (valueFactory is null && exceptionFactory is null)
                throw new ArgumentException("Value factory and exception factory is null");

            this.valueFactory = valueFactory;
            this.exceptionFactory = exceptionFactory;
            this.valueFactoryState = valueFactoryState;
            this.exceptionFactoryState = exceptionFactoryState;
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator Result<TValue>((TValue? value, Exception? exception) input)
        {
            if (input.value.IsNotNull())
                return new Result<TValue>(input.value!);

            if (input.exception is not null)
                return new Result<TValue>(input.exception!);

            throw new ArgumentException("Value and exception cannot be null");
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator Maybe<TValue>(Result<TValue> source)
        {
            return source.Lax();
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static explicit operator TValue(Result<TValue> source)
        {
            return source.Strict();
        }

        public static bool operator ==(Result<TValue> left, Result<TValue> right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Result<TValue> left, Result<TValue> right)
        {
            return !(left == right);
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Maybe<TValue> Lax() => GetValue();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public TValue Strict()
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
                    var args = argsUntyped.CastTo<(object? valueFactoryState, Func<object?, TValue> valueFactory)>();

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
                return TypeCache<Result<TValue, TValueFactoryState, TExceptionFactoryState>>.FullName;

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
            return obj is Result<TValue> result && Equals(result);
        }

        public readonly bool Equals(Result<TValue, TValueFactoryState, TExceptionFactoryState> other)
        {
            return EqualityComparer<Func<TValueFactoryState?, TValue?>?>.Default.Equals(valueFactory, other.valueFactory)
                   &&
                   EqualityComparer<Func<TExceptionFactoryState?, Exception?>?>.Default.Equals(exceptionFactory, other.exceptionFactory)
                   &&
                   EqualityComparer<TValueFactoryState?>.Default.Equals(valueFactoryState, other.valueFactoryState)
                   &&
                   EqualityComparer<TExceptionFactoryState?>.Default.Equals(exceptionFactoryState, other.exceptionFactoryState)
                   &&
                   EqualityComparer<TValue?>.Default.Equals(value, other.value)
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

        private TValue? GetValue()
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
