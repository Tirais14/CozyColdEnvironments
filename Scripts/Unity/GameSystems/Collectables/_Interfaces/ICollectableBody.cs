using CCEnvs.Unity.GameSystems.Storages;

#nullable enable
namespace CCEnvs.Unity.GameSystems.Interactables
{
    public interface ICollectableBody
    {
        IStorageItem? Item { get; }
        int ItemCount { get; }

        IItemStack GetCollectedItems(IStorageItem? item = null);
    }
}
