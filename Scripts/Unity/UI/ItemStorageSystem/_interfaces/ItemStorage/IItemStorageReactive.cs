using CCEnvs.GameSystems.ItemStorageSystem;

#nullable enable
namespace CCEnvs.Unity.UI.Storages
{
    public interface IItemStorageReactive : IItemStorage, IOpenableReactive
    {
    }
    public interface IItemStorageReactive<T> : IItemStorageReactive, IItemStorage<T>
        where T : IItemSlot
    {
    }
}
