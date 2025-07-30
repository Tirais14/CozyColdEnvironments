using UTIRLib.GameSystems.Storage;

#nullable enable
namespace UTIRLib.UI.ItemStorage
{
    public interface IItemStorageReactive : IItemStorage, IOpenableReactive
    {
    }
    public interface IItemStorageReactive<T> : IItemStorageReactive, IItemStorage<T>
        where T : IItemSlot
    {
    }
}
