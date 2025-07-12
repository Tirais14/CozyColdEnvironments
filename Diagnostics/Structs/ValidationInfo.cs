using System;

#nullable enable

namespace UTIRLib.Diagnostics
{
    public readonly struct ValidationInfo
    {
        public readonly Exception exception;

        public ValidationInfo(Exception exception) => this.exception = exception;

        public readonly bool HasException() => exception != null;

        public readonly bool HasException(out ValidationInfo instance)
        {
            instance = this;

            return exception != null;
        }

        public readonly bool HasException(out Exception exception)
        {
            exception = this.exception;

            return this.exception != null;
        }

        public readonly void TryThrow()
        {
            if (exception != null)
            {
                throw exception;
            }
        }

        public static implicit operator bool(ValidationInfo validationInfo) => validationInfo.exception != null;

        public static implicit operator Exception(ValidationInfo validationInfo) => validationInfo.exception;
    }
}