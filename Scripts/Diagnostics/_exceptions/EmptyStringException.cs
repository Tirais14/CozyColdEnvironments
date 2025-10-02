#nullable enable

using System;

namespace CCEnvs.Diagnostics
{
    public sealed class EmptyStringException : CCException
    {
        public EmptyStringException()
        {
        }

        public EmptyStringException(string message, Exception? innerException = null)
            :
            base(message, innerException)
        {
        }
    }
}