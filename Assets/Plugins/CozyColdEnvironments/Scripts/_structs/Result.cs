using CCEnvs.FuncLanguage;
using CCEnvs.Reflection.Caching;
using CommunityToolkit.Diagnostics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;

#nullable enable
namespace CCEnvs
{
    public readonly struct Result<TValue> : IEquatable<Result<TValue>>
    {
        public readonly static Result<TValue> Empty = new();

        private readonly TValue? value;
        private readonly Exception? exception;

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
        public readonly Maybe<TValue> Lax() => value;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly TValue Strict()
        {
            if (value.IsNull())
                ThrowException();

            return value!;
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
                value,
                exception
                );
        }

        private readonly void ThrowException()
        {
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
        public static explicit operator TValue(Result<TValue, TValueFactoryState, TExceptionFactoryState> source)
        {
            return source.Strict();
        }

        public static bool operator ==(Result<TValue, TValueFactoryState, TExceptionFactoryState> left, Result<TValue, TValueFactoryState, TExceptionFactoryState> right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Result<TValue, TValueFactoryState, TExceptionFactoryState> left, Result<TValue, TValueFactoryState, TExceptionFactoryState> right)
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
