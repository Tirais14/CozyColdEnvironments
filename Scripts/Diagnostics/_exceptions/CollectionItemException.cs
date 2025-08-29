#nullable enable

namespace CCEnvs.Diagnostics
{
    public class CollectionItemException : TirLibException
    {
        public CollectionItemException() : base()
        {
        }

        public CollectionItemException(object? item)
            :
            base(ConstructMessage(typeof(CollectionItemException), item))
        {
        }

        public CollectionItemException(object item, object position)
            :
            base(ConstructMessage(typeof(CollectionItemException), item, position))
        {
        }
    }
}