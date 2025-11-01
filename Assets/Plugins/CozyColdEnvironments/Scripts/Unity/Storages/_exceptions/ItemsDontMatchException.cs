using CCEnvs.Diagnostics;

#nullable enable
namespace CCEnvs.Unity.Storages
{
    public class ItemsDontMatchException : CCException
    {
        public ItemsDontMatchException()
        {
        }

        public ItemsDontMatchException(IItem? left, IItem? right) 
            :
            base($"Item dont match. Left: {left}; right: {right}.")
        {
        }
    }
}
