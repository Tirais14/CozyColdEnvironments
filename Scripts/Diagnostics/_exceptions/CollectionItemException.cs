#nullable enable
using static CCEnvs.Diagnostics.ExceptionMessageConstructor;

namespace CCEnvs.Diagnostics
{
    public class CollectionItemException : CCException
    {
        public CollectionItemException() : base()
        {
        }

        public CollectionItemException(object? item)
            :
            base(ConstructMessage<CollectionItemException>(TypeValuePair.Create(item)))
        {
        }

        public CollectionItemException(object? item, object position)
            :
            base(ConstructMessage<CollectionItemException>(
                TypeValuePair.Create(item),
                new TypeValuePair(position)))
        {
        }
    }
}