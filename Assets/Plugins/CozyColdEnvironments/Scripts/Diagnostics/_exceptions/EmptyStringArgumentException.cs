#nullable enable
using System;

namespace CCEnvs.Diagnostics
{
    public sealed class EmptyStringArgumentException : ArgumentException
    {
        public EmptyStringArgumentException()
        {
        }

        public EmptyStringArgumentException(string paramName, Exception? innerException = null)
            :
            base(string.Empty, paramName, innerException)
        {
        }

        public EmptyStringArgumentException(string paramName, string message, Exception? innerException = null)
            :
            base(message, paramName, innerException)
        {
        }
    }
}