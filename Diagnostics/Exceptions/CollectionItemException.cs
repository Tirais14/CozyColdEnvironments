using UTIRLib.Diagnostics;

#nullable enable
namespace UTIRLib
{
    public class CollectionItemException : TirLibException
    {
        public CollectionItemException()
        {
        }

        public CollectionItemException(string message) : base(message)
        {
        }

        public CollectionItemException(string message, int index) 
            : base(message + $" Index = {index}.")
        {
        }
    }
}
