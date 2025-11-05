using CCEnvs.FuncLanguage;
using System;

#nullable enable
namespace CCEnvs
{
    public readonly struct InvokeInfo
    {
        public readonly Maybe<Exception> exception;
        public readonly Maybe<string> message;
        public readonly bool isError;

        public InvokeInfo(Exception? exception)
            :
            this()
        {
            this.exception = exception;
            this.isError = exception is not null;
        }

        public InvokeInfo(string? message, bool hasError)
            :
            this()
        {
            this.message = message;
            this.isError = hasError;
        }

        public static implicit operator InvokeInfo(Exception? exception)
        {
            return new InvokeInfo(exception);
        }

        public static implicit operator InvokeInfo((string? msg, bool isError) input)
        {
            return new InvokeInfo(input.msg, input.isError);
        }

        public static explicit operator bool(InvokeInfo input)
        {
            return input.isError;
        }

        public static explicit operator string?(InvokeInfo input)
        {
            return input.message.Raw;
        }
    }
}
