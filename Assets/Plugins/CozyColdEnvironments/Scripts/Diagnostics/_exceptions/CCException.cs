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

        public CCException(Exception? innerException)
            :
            base(string.Empty, innerException)
        {

        }

        public CCException(string message)
            :
            base(message)
        {

        }

        public CCException(string message,
                           Exception? innerException)
            :
            base(message, innerException)
        {
        }
    }
}