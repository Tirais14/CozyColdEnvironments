using System;

namespace CCEnvs.Services
{
    public class BindingNotFoundException : Exception
    {
        public BindingNotFoundException(string message)
            :
            base(message)
        {
        }

        public BindingNotFoundException(string message, Exception innerException)
            :
            base(message, innerException)
        {
        }
    }
}
