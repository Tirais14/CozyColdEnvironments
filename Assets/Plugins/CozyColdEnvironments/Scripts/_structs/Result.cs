using CCEnvs.Diagnostics;
using CCEnvs.FuncLanguage;
using CommunityToolkit.Diagnostics;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

#nullable enable
namespace CCEnvs.Unity
{
    public readonly struct Result<T>
    {
        public readonly Exception? exception;
        private readonly Lazy<T?> raw;

        public T? Raw => raw.Value;

        public Result(T? value, Exception? exception)
        {
            this.raw = new Lazy<T?>(value);
            this.exception = exception;
        }
        public Result(T value)
            :
            this()
        {
            CC.Guard.IsNotNull(value, nameof(value));

            raw = new Lazy<T?>(value);
        }

        public Result(Func<T> valueFactory, Exception exception)
        {
            Guard.IsNotNull(valueFactory, nameof(valueFactory));
            Guard.IsNotNull(exception, nameof(exception));

            raw = new Lazy<T?>(valueFactory);
            this.exception = exception;
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
                if (exception is null)
                    throw new InvalidOperationException($"{nameof(Strict)} value and exception is null. This exception means violation of contract.");

                throw exception;
            }

            return raw.Value;
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Result<TOut> Cast<TOut>()
        {
            return (raw.Value.As<TOut>(), exception);
        }

        public override string ToString()
        {
            return $"{nameof(Raw)}: {raw.Value}; {nameof(exception)}: {exception}";
        }
    }
}
