#nullable enable
using CCEnvs.Unity.GameSystems.Storages;

namespace CCEnvs.Unity.GameSystems.Interactables
{
    public interface ICollectableTrigger : IToggleable
    {
        IItemStack Collect(IStorageItem? item = null);
    }
}
