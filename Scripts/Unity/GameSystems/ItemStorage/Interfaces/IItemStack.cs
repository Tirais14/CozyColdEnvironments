#nullable enable
namespace CozyColdEnvironments.GameSystems.ItemStorageSystem
{
    public interface IItemStack : IItemContainer
    {
    }

    public interface IItemStack<T> : IItemContainer<IItemStack<T>, T>
        where T : IStorageItem, new()
    {
    }
}
