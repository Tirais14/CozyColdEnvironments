using System;

namespace CCEnvs.Services
{
    public class ServiceResolveException : Exception
    {
        public ServiceResolveException(string message)
            :
            base(message)
        {
        }

        public ServiceResolveException(string message, Exception innerException)
            :
            base(message, innerException)
        {
        }
    }
}
