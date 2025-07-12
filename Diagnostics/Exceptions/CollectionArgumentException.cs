using System.Collections;

#nullable enable

namespace UTIRLib.Diagnostics
{
    public class CollectionArgumentException : TirLibException
    {
        public CollectionArgumentException() : base()
        {
        }

        public CollectionArgumentException(string paramName) : base(GetParamNameMsg(paramName))
        {
        }

        public CollectionArgumentException(string paramName, IEnumerable? collection)
            : base($"Collection: {WrongCollectionException.Resolve(collection)}. {GetParamNameMsg(paramName)}")
        {
        }
    }
}