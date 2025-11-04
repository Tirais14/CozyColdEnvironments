using CCEnvs.Diagnostics;
using CCEnvs.FuncLanguage;
using CommunityToolkit.Diagnostics;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace CCEnvs.Unity
{
    public readonly struct Result<T>
    {
        public readonly Exception exception;
        private readonly T value;

        public T Raw => value;

        public Result(T value, Exception exception)
        {
            Guard.IsNotNull(exception, nameof(exception));

            this.value = value;
            this.exception = exception;
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator Result<T>((T value, Exception exception) input)
        {
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
        public Maybe<T> Lax() => value;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T Strict()
        {
            if (value.IsNull())
                throw exception;

            return value;
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Result<TOut> Cast<TOut>()
        {
            return (value.As<TOut>(), exception);
        }
    }
}
