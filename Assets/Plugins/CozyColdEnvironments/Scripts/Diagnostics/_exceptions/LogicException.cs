using System;

#nullable enable
namespace CCEnvs.Diagnostics
{
    public class LogicException : CCException
    {
        public LogicException()
        {
        }

        public LogicException(string message, Exception? innerException = null)
            : 
            base(message, innerException)
        {
        }
    }
}
