#nullable enable

namespace CCEnvs.Diagnostics
{
    public class EmptyCollectionException : CCException
    {
        public EmptyCollectionException()
        {
        }

        public EmptyCollectionException(string message) : base(message)
        {
        }
    }
}