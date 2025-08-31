#nullable enable
using System;

namespace CCEnvs.Diagnostics
{
    /// <summary>
    /// CozyColdEnvironments framework exception
    /// </summary>
    public class CCArgumentException : ArgumentException
    {
        public CCArgumentException() : base()
        {
        }

        public CCArgumentException(string message,
                                   Exception? innerException = null)
            :
            base(message,
                 innerException)
        {
        }
    }
}
