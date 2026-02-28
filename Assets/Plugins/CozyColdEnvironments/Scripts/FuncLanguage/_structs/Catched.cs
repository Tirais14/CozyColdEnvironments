using CCEnvs.Diagnostics;
using CCEnvs.Reflection;
using CommunityToolkit.Diagnostics;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

#nullable enable
#pragma warning disable S3236
#pragma warning disable IDE1006
namespace CCEnvs.FuncLanguage
{
    public readonly partial struct Catched
    {
        public readonly static Catched None = new();

        private readonly Maybe<Type> exceptionType;
        private readonly Action? action;
        private readonly Action<Exception, LogType>? onError;

        public LogType logType { get; }

        public Catched(Type? exceptionType = null,
            LogType logType = LogType.Log,
            Action? action = null,
            Action<Exception, LogType>? onError = null)
        {
            this.exceptionType = exceptionType;
            this.logType = logType;
            this.action = action;
            this.onError = onError;
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly Catched WithExceptionType(Type? type)
        {
            return (type, logType, action, onError);
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly Catched WithLogType(LogType logType)
        {
            return (exceptionType.Raw, logType, action, onError);
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly Catched WithAction(Action? action)
        {
            return (exceptionType.Raw, logType, action, onError);
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly Catched WithActionOnError(Action<Exception, LogType>? onErro)
        {
            return (exceptionType.Raw, logType, action, onError);
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator Catched((Type? exceptionType, LogType logType, Action? action, Action<Exception, LogType>? onError) input)
        {
            return new Catched(exceptionType: input.exceptionType,
                logType: input.logType,
                action: input.action,
                onError: input.onError
                );
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator Catched((Type? exceptionType, LogType logType, Action? action) input)
        {
            return new Catched(exceptionType: input.exceptionType,
                logType: input.logType,
                action: input.action
                );
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator Catched((Type? exceptionType, LogType logType) input)
        {
            return new Catched(exceptionType: input.exceptionType,
                logType: input.logType
                );
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator Catched(Type? exceptionType)
        {
            return new Catched(exceptionType: exceptionType);
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator Catched(LogType logType)
        {
            return new Catched(logType: logType);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly Catched Do()
        {
            Guard.IsNotNull(action, nameof(action));

            if (action is null)
            {
                this.PrintWarning("Action is null");
                return this;
            }

            try
            {
                action();
            }
            catch (Exception ex)
            {
                if (IsThrowAllowed(ex))
                    throw;

                OnError(ex);
            }

            return this;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly Catched Do(Action action)
        {
            Guard.IsNotNull(action, nameof(action));

            try
            {
                action();
            }
            catch (Exception ex)
            {
                if (IsThrowAllowed(ex))
                    throw;

                OnError(ex);
            }

            return this;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly Catched Do<TArg>(TArg arg, Action<TArg> action)
        {
            Guard.IsNotNull(action, nameof(action));

            try
            {
                action(arg);
            }
            catch (Exception ex)
            {
                if (IsThrowAllowed(ex))
                    throw;

                OnError(ex);
            }

            return this;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly Catched Do<TArg, TArg1>(TArg arg,
            TArg1 arg1,
            Action<TArg, TArg1> action)
        {
            Guard.IsNotNull(action, nameof(action));

            try
            {
                action(arg, arg1);
            }
            catch (Exception ex)
            {
                if (IsThrowAllowed(ex))
                    throw;

                OnError(ex);
            }

            return this;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly Catched Do<TArg, TArg1, TArg2>(TArg arg,
            TArg1 arg1,
            TArg2 arg2,
            Action<TArg, TArg1, TArg2> action)
        {
            Guard.IsNotNull(action, nameof(action));

            try
            {
                action(arg, arg1, arg2);
            }
            catch (Exception ex)
            {
                if (IsThrowAllowed(ex))
                    throw;

                OnError(ex);
            }

            return this;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly Maybe<TOut> Do<TOut>(Func<TOut> selector)
        {
            Guard.IsNotNull(selector, nameof(selector));

            try
            {
                return selector();
            }
            catch (Exception ex)
            {
                if (IsThrowAllowed(ex))
                    throw;

                OnError(ex);
            }

            return Maybe<TOut>.None;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly Maybe<TOut> Do<TArg, TOut>(TArg arg,
            Func<TArg, TOut> selector)
        {
            Guard.IsNotNull(selector, nameof(selector));

            try
            {
                return selector(arg);
            }
            catch (Exception ex)
            {
                if (IsThrowAllowed(ex))
                    throw;

                OnError(ex);
            }

            return Maybe<TOut>.None;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly Maybe<TOut> Do<TArg, TArg1, TOut>(TArg arg,
            TArg1 arg1,
            Func<TArg, TArg1, TOut> selector)
        {
            Guard.IsNotNull(selector, nameof(selector));

            try
            {
                return selector(arg, arg1);
            }
            catch (Exception ex)
            {
                if (IsThrowAllowed(ex))
                    throw;

                OnError(ex);
            }

            return Maybe<TOut>.None;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly Maybe<TOut> Do<TArg, TArg1, TArg2, TOut>(TArg arg,
            TArg1 arg1,
            TArg2 arg2,
            Func<TArg, TArg1, TArg2, TOut> selector)
        {
            Guard.IsNotNull(selector, nameof(selector));

            try
            {
                return selector(arg, arg1, arg2);
            }
            catch (Exception ex)
            {
                if (IsThrowAllowed(ex))
                    throw;

                OnError(ex);
            }

            return Maybe<TOut>.None;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool IsThrowAllowed(Exception ex)
        {
            return exceptionType.Map(type => ex.IsInstanceOfType(type)).GetValue(false);
        }

        private void OnError(Exception ex)
        {
            if (onError is not null)
                onError(ex, logType);
            else
                this.PrintDebug(ex, logType);
        }
    }
}
