using UTIRLib.GameSystems.ItemStorageSystem;

#nullable enable
namespace UTIRLib.UI.ItemStorageSystem
{
    public interface IItemStorageReactive : IItemStorage, IOpenableReactive
    {
    }
    public interface IItemStorageReactive<T> : IItemStorageReactive, IItemStorage<T>
        where T : IItemSlot
    {
    }
}
