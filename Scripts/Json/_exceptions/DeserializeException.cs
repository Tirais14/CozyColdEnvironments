using CozyColdEnvironments.Diagnostics;

#nullable enable
namespace CozyColdEnvironments.Json
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
