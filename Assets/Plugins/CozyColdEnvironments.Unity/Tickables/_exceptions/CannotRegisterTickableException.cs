using CCEnvs.Diagnostics;
using CCEnvs.Reflection;
using System;

#nullable enable
namespace CCEnvs.Unity
{
    public class CannotRegisterTickableException : CCException
    {
        public CannotRegisterTickableException()
        {
        }

        public CannotRegisterTickableException(string reason, Type? tickableType = null, Exception? innerException = null)
            :
            base($"Cannot register tickable {(tickableType?.GetName() ?? ".")}. {reason}", innerException)
        {
        }
    }
}
