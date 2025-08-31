#nullable enable
using System;

namespace CCEnvs.Diagnostics
{
    public class CCFrameworkException : CCException
    {
        public CCFrameworkException()
        {
        }

        public CCFrameworkException(string message, Exception? innerException = null)
            :
            base(message, innerException)
        {
        }
    }
}
