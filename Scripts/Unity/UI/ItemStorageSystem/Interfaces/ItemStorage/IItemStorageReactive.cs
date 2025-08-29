using CozyColdEnvironments.GameSystems.ItemStorageSystem;

#nullable enable
namespace CozyColdEnvironments.UI.ItemStorageSystem
{
    public interface IItemStorageReactive : IItemStorage, IOpenableReactive
    {
    }
    public interface IItemStorageReactive<T> : IItemStorageReactive, IItemStorage<T>
        where T : IItemSlot
    {
    }
}
