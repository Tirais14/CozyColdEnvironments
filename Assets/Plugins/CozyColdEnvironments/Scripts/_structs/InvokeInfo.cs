using CCEnvs.FuncLanguage;
using System;

#nullable enable
namespace CCEnvs
{
    public readonly struct InvokeInfo
    {
        public readonly Maybe<Exception> exception;
        public readonly Maybe<string> message;
        public readonly bool hasError;

        public InvokeInfo(Exception? exception)
            :
            this()
        {
            this.exception = exception;
            this.hasError = exception is not null;
        }

        public InvokeInfo(string? message, bool hasError)
            :
            this()
        {
            this.message = message;
            this.hasError = hasError;
        }

        public static implicit operator InvokeInfo(Exception? exception)
        {
            return new InvokeInfo(exception);
        }

        public static implicit operator InvokeInfo((string? msg, bool hasError) input)
        {
            return new InvokeInfo(input.msg, input.hasError);
        }

        public static explicit operator bool(InvokeInfo input)
        {
            return input.hasError;
        }

        public static explicit operator string?(InvokeInfo input)
        {
            return input.message.Raw;
        }
    }
}
