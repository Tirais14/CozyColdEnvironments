using System.Collections;

#nullable enable

namespace UTIRLib.Diagnostics
{
    public class CollectionException : TirLibException
    {
        public CollectionException()
        {
        }

        public CollectionException(string message) : base(message)
        {
        }

        public CollectionException(IEnumerable? collection)
            : 
            base(ConstructMessage(typeof(CollectionException), collection))
        {
        }

        public CollectionException(IEnumerable? collection, string message)
            :
             base(ConstructMessage(typeof(CollectionException), collection, message))
        {
        }
    }
}