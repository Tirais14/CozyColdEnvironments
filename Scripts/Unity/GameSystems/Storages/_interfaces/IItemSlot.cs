#nullable enable
namespace CCEnvs.Unity.GameSystems.Storages
{
    public interface IItemSlot : IItemContainer
    {
        int CapacityLimit { get; set; }
        bool HasCapacityLimit { get; }

        bool Contains(IItemStack itemStack);
    }
    public interface IItemSlot<T> : IItemSlot, IItemContainer<T>
        where T : IItemStack
    {
        bool Contains(T itemStack);

        bool IItemSlot.Contains(IItemStack itemStack)
        {
            return itemStack is T typed && Contains(typed);
        }
    }
}
