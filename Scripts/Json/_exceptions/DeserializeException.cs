using CCEnvs.Diagnostics;

#nullable enable
namespace CCEnvs.Json
{
    public class DeserializeException : TirLibException
    {
        public DeserializeException()
        {
        }

        public DeserializeException(string message) : base(message)
        {
        }
    }
}
