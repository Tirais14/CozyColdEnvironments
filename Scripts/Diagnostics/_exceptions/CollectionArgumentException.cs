using System.Collections;
using static CCEnvs.Diagnostics.ExceptionMessageConstructor;

#nullable enable

namespace CCEnvs.Diagnostics
{
    public class CollectionArgumentException : CCException
    {
        public CollectionArgumentException() : base()
        {
        }

        public CollectionArgumentException(string paramName)
            :
            base(ConstructMessage<CollectionArgumentException>(paramName))
        {
        }

        public CollectionArgumentException(string paramName, IEnumerable? collection)
            :
            base(ConstructMessage<CollectionArgumentException>(
                new TypeValuePair(paramName),
                TypeValuePair.T(collection)))
        {
        }
    }
}