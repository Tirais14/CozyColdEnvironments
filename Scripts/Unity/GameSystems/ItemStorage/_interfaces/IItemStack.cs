#nullable enable
namespace CCEnvs.GameSystems.Storages
{
    public interface IItemStack : IItemContainer
    {
    }

    public interface IItemStack<T> : IItemContainer<IItemStack<T>, T>
        where T : IStorageItem, new()
    {
    }
}
