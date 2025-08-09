using UTIRLib.Diagnostics;

#nullable enable
namespace UTIRLib.Json
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
