#nullable enable
namespace CCEnvs.Unity.GameSystems.Storages
{
    public interface IItemAccessor
    {
        IItemContainer Put(IItem? item, int count);
        IItemContainer Put(IItemContainer itemContainer, int count);
        IItemContainer Put(IItemContainer itemContainer);

        IItemContainer Take(int count);
        IItemContainer Take(IItem item, int count);

        void CopyFrom(IItemContainerInfo itemContainer);

        void Clear();
    }
}
