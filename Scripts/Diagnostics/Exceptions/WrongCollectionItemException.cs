#nullable enable

namespace UTIRLib.Diagnostics
{
    public class WrongCollectionItemException : TirLibException
    {
        public WrongCollectionItemException() : base()
        {
        }

        public WrongCollectionItemException(object? item) : base($"Item {GetObjectTypeName(item)}.")
        {
        }

        public WrongCollectionItemException(object item, object position)
            : base($"Item {GetObjectTypeName(item)}. Position: {position}")
        {
        }
    }
}