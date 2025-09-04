#nullable enable
using CCEnvs.Reflection;
using System;

namespace CCEnvs.Diagnostics
{
    public class DeserializeDataException : CCException
    {
        public DeserializeDataException()
        {
        }

        public DeserializeDataException(Type deserializeType,
                                        string? message = null,
                                        Exception? innerException = null)
            :
            base($"Deserialize type = {deserializeType.GetName()}. {message}", innerException)
        {
        }
    }
}
