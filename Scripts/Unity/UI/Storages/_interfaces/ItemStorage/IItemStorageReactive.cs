using CCEnvs.Unity.GameSystems.Storages;

#nullable enable
namespace CCEnvs.Unity.UI.Storages
{
    public interface IItemStorageReactive 
        :
        IItemStorage, 
        IOpenableRx
    {
    }
    public interface IItemStorageReactive<T> 
        : 
        IItemStorageReactive, 
        IItemStorage<T>

        where T : IItemSlot
    {
    }
}
