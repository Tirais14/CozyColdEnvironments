using System.Collections;
using static CCEnvs.Diagnostics.ExceptionMessageConstructor;

#nullable enable

namespace CCEnvs.Diagnostics
{
    public class CollectionException : CCException
    {
        public CollectionException()
        {
        }

        public CollectionException(string message) : base(message)
        {
        }

        public CollectionException(IEnumerable? collection)
            : 
            base(ConstructMessage<CollectionException>(TypeValuePair.T(collection)))
        {
        }

        public CollectionException(IEnumerable? collection, string message)
            :
             base(ConstructMessage<CollectionException>(
                 TypeValuePair.T(collection),
                 new TypeValuePair(message)))
        {
        }
    }
}