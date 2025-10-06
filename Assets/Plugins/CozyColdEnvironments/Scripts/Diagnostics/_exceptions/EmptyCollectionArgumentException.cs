#nullable enable

using System;

namespace CCEnvs.Diagnostics
{
    public class EmptyCollectionArgumentException : ArgumentException
    {
        public EmptyCollectionArgumentException() : base()
        {
        }

        public EmptyCollectionArgumentException(string paramName, Exception? innerException = null)
            :
            base(string.Empty, paramName, innerException)
        {
        }

        public EmptyCollectionArgumentException(string paramName, string message, Exception? innerException = null)
            :
            base(message, paramName, innerException)
        {
        }
    }
}