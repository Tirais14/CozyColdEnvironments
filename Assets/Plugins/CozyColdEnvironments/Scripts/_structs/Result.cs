using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using CCEnvs.FuncLanguage;
using CommunityToolkit.Diagnostics;

#nullable enable
namespace CCEnvs
{
    public static class Result
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Result<T> Create<T>(T? value, Exception exception)
        {
            return new Result<T>(value, exception);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Result<T> Create<T>(T value)
        {
            return new Result<T>(value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Result<T> Lazy<T>(Func<T> factory, Func<Exception?> exceptionFactory)
        {
            return Result<T>.Lazy(factory, exceptionFactory);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Result<T> Lazy<T>(Func<T> factory)
        {
            return Result<T>.Lazy(factory);
        }
    }

    public readonly struct Result<T>
    {
        public readonly static Result<T> Empty = new();

        public readonly Lazy<Exception?> exception;
        private readonly Lazy<T?> raw;

        public T? Raw => raw.Value;

        public Result(T? value, Exception? exception)
            :
            this()
        {
            this.raw = new Lazy<T?>(value);
            this.exception = new Lazy<Exception?>(exception);
        }

        public Result(T? value, Func<Exception?> exception)
        {
            this.raw = new Lazy<T?>(value);
            this.exception = new Lazy<Exception?>(exception);
        }

        public Result(T value)
            :
            this()
        {
            CC.Guard.IsNotNull(value, nameof(value));

            raw = new Lazy<T?>(value);
            exception = new Lazy<Exception?>(value: null);
        }

        private Result(Func<T?> valueFactory, Func<Exception?> exceptionFactory)
        {
            Guard.IsNotNull(valueFactory, nameof(valueFactory));
            Guard.IsNotNull(exceptionFactory, nameof(exceptionFactory));

            raw = new Lazy<T?>(valueFactory);
            this.exception = new Lazy<Exception?>(exceptionFactory);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Result<T> Lazy(Func<T> factory, Func<Exception?> exceptionFactory)
        {
            return new Result<T>(factory, exceptionFactory);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Result<T> Lazy(Func<T> factory)
        {
            return new Result<T>(factory, exceptionFactory: null!);
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator Result<T>((T? value, Exception? exception) input)
        {
            if (input.exception is null)
                return new Result<T>(input.value!);

            return new Result<T>(input.value, input.exception);
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

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Maybe<T> Lax() => raw.Value;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T Strict()
        {
            if (raw.Value.IsNull())
            {
                if (exception.Value is null)
                    throw new InvalidOperationException($"{nameof(Strict)} value and exception is null. This exception means violation of contract.");

                throw exception.Value;
            }

            return raw.Value;
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Result<TOut> Cast<TOut>()
        {
            return (raw.Value.CastTo<TOut>(), exception.Value);
        }

        public override string ToString()
        {
            return $"{nameof(Raw)}: {raw.Value}; {nameof(exception)}: {exception}";
        }
    }
}
