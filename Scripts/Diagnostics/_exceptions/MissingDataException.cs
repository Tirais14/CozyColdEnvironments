#nullable enable
using CCEnvs.Reflection;
using System;

namespace CCEnvs.Diagnostics
{
    public class MissingDataException : CCException
    {
        public MissingDataException()
        {
        }

        public MissingDataException(string message, Exception? innerException = null) 
            :
            base(message, innerException)
        {
        }
    }
}
