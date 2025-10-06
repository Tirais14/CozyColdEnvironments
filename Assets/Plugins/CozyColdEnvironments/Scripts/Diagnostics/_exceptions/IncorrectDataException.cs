#nullable enable
using System;

namespace CCEnvs.Diagnostics
{
    public class IncorrectDataException : CCException
    {
        public IncorrectDataException()
        {
        }

        public IncorrectDataException(string message, Exception? innerException = null)
            :
            base(message, innerException)
        {
        }

        public IncorrectDataException(object obj, Exception? innerException = null)
            :
            this(obj.ToString(), innerException)
        {
        }
    }
}
