#nullable enable
using System;

namespace CozyColdEnvironments.Diagnostics
{
    public class LogicException : TirLibException
    {
        public LogicException()
        {
        }

        public LogicException(string message, Exception? innerException = null)
            :
            base(message, innerException)
        {
        }

        public LogicException(string notFormattedMessage, params object[] args)
            :
            base(notFormattedMessage, args)
        {
        }

        public LogicException(Exception? innerException,
                              string notFormattedMessage,
                              params object[] args) 
            :
            base(innerException,
                 notFormattedMessage,
                 args)
        {
        }
    }
}
