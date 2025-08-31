using CCEnvs.Diagnostics;

#nullable enable
namespace CCEnvs.Json
{
    public class DeserializeException : CCException
    {
        public DeserializeException()
        {
        }

        public DeserializeException(string message) : base(message)
        {
        }
    }
}
