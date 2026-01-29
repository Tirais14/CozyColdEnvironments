using CCEnvs.Diagnostics;
using CCEnvs.Reflection;
using System;

#nullable enable
namespace CCEnvs.Unity
{
    public class CannotRegisterTickerException : CCException
    {
        public CannotRegisterTickerException()
        {
        }

        public CannotRegisterTickerException(string reason,
            Type? tickerType = null,
            Exception? innerException = null)
            :
            base($"Cannot register ticker {(tickerType?.GetName() ?? string.Empty)}. {reason}", innerException)
        {
        }
    }
}
