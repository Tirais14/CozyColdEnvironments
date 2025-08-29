using System.Collections;

#nullable enable

namespace CCEnvs.Diagnostics
{
    public class CollectionArgumentException : CCEException
    {
        public CollectionArgumentException() : base()
        {
        }

        public CollectionArgumentException(string paramName)
            :
            base(ConstructMessage(typeof(CollectionArgumentException),
                                  paramName))
        {
        }

        public CollectionArgumentException(string paramName, IEnumerable? collection)
            :
            base(ConstructMessage(typeof(CollectionArgumentException),
                                  paramName,
                                  collection))
        {
        }
    }
}