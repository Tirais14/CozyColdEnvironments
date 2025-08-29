#nullable enable
namespace UTIRLib.GameSystems.ItemStorageSystem
{
    public interface IItemStack : IItemContainer
    {
    }

    public interface IItemStack<T> : IItemContainer<IItemStack<T>, T>
        where T : IStorageItem, new()
    {
    }
}
