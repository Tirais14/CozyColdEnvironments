#nullable enable
using System;

namespace CCEnvs.Diagnostics
{
    public class CannotResolvedException : CCException
    {
        public CannotResolvedException()
        {
        }

        public CannotResolvedException(string message, Exception? innerException = null)
            : 
            base(message, innerException)
        {
        }
    }
}
