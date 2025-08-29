using UTIRLib.GameSystems.ItemStorageSystem;

#nullable enable
namespace UTIRLib.GameSystems
{
    public interface IItemContainerInfo
    {
        IStorageItem Item { get; }
        int ItemCount { get; }
        int MaxItemCount { get; }
        bool HasItem { get; }
        bool IsContainerFull { get; }

        bool IsSameItem(IStorageItem item);

        bool CanHold(IStorageItem item);

        bool Contains(IStorageItem item);
    }
    public interface IItemContainerInfo<T> : IItemContainerInfo
        where T : IStorageItem
    {
        new T Item { get; }

        IStorageItem IItemContainerInfo.Item => Item;

        bool IsSameItem(T item);

        bool CanHold(T item);

        bool Contains(T item);

        bool IItemContainerInfo.IsSameItem(IStorageItem item)
        {
            return item is T typed && IsSameItem(typed);
        }

        bool IItemContainerInfo.CanHold(IStorageItem item)
        {
            return item is T typed && CanHold(typed);
        }

        bool IItemContainerInfo.Contains(IStorageItem item)
        {
            return item is T typed && Contains(typed);
        }
    }
}
