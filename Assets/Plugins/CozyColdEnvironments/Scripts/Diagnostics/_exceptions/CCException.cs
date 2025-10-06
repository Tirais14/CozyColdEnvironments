using System;

#nullable enable
namespace CCEnvs.Diagnostics
{
    /// <summary>
    /// CozyColdEnvironments framework exception
    /// </summary>
    public class CCException : Exception
    {
        public CCException() : base()
        {
        }

        public CCException(string message,
                           Exception? innerException = null)
            :
            base(message,
                 innerException)
        {
        }
    }
}