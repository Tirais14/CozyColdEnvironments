using CCEnvs.Diagnostics;

#nullable enable
namespace CCEnvs.Json
{
    public class DeserializeException : CCEException
    {
        public DeserializeException()
        {
        }

        public DeserializeException(string message) : base(message)
        {
        }
    }
}
