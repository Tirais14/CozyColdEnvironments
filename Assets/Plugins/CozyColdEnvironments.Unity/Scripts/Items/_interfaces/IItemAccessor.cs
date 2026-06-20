#nullable enable
using CCEnvs.TypeMatching;

namespace CCEnvs.UnityX.Items
{
    public interface IItemAccessor
    {
        ReadOnlyItemContainer PutItem(IItem? item, int count = 1);
        ReadOnlyItemContainer PutItem(IItemContainerInfo? containerInfo);
        ReadOnlyItemContainer PutItem<TItemContainerInfo>(TItemContainerInfo containerInfo)
            where TItemContainerInfo : struct, IItemContainerInfo;

        ReadOnlyItemContainer PutItemFrom(IItemContainer? itemContainer, int count);
        ReadOnlyItemContainer PutItemFrom(IItemContainer? itemContainer);

        ReadOnlyItemContainer TakeItem();
        ReadOnlyItemContainer TakeItem(int count);
        ReadOnlyItemContainer TakeItem(IItem? item, int count);

        void CopyItemFrom(IItemContainerInfo itemContainer);

        void Clear();
    }

    public interface IItemAccessor<TItem> : IItemAccessor
        where TItem : IItem
    {
        ReadOnlyItemContainer<TItem> PutItem(TItem? item, int count = 1);
        ReadOnlyItemContainer<TItem> PutItem(IItemContainerInfo<TItem>? containerInfo);
        new ReadOnlyItemContainer<TItem> PutItem<TItemContainerInfo>(TItemContainerInfo containerInfo)
            where TItemContainerInfo : struct, IItemContainerInfo<TItem>;

        ReadOnlyItemContainer<TItem> PutItemFrom(IItemContainer<TItem>? itemContainer, int count);
        ReadOnlyItemContainer<TItem> PutItemFrom(IItemContainer<TItem>? itemContainer);

        new ReadOnlyItemContainer<TItem> TakeItem();
        new ReadOnlyItemContainer<TItem> TakeItem(int count);
        ReadOnlyItemContainer<TItem> TakeItem(TItem? item, int count);

        void CopyItemFrom(IItemContainerInfo<TItem> itemContainer);

        void IItemAccessor.CopyItemFrom(IItemContainerInfo itemContainer)
        {
            if (itemContainer.IsNot<IItemContainerInfo<TItem>>(out var typed))
                return;

            CopyItemFrom(typed);
        }

        ReadOnlyItemContainer IItemAccessor.PutItem(IItem? item, int count)
        {
            return PutItem(item.As<TItem>(), count);
        }
        ReadOnlyItemContainer IItemAccessor.PutItem(IItemContainerInfo containerInfo)
        {
            if (containerInfo.IsNot<IItemContainerInfo<TItem>>(out var typedContainerInfo))
                return ReadOnlyItemContainer.Empty;

            return PutItem(typedContainerInfo);
        }
        ReadOnlyItemContainer IItemAccessor.PutItem<TItemContainerInfo>(TItemContainerInfo containerInfo)
        {
            if (containerInfo.IsNot<IItemContainerInfo<TItem>>(out var typedContainerInfo))
                return ReadOnlyItemContainer.Empty;

            return PutItem(typedContainerInfo);
        }

        ReadOnlyItemContainer IItemAccessor.PutItemFrom(IItemContainer itemContainer)
        {
            if (itemContainer.IsNot<IItemContainer<TItem>>(out var typedContainer))
                return default;

            return PutItemFrom(typedContainer);
        }
        ReadOnlyItemContainer IItemAccessor.PutItemFrom(IItemContainer itemContainer, int count)
        {
            if (itemContainer.IsNot<IItemContainer<TItem>>(out var typedContainer))
                return default;

            return PutItemFrom(typedContainer, count);
        }

        ReadOnlyItemContainer IItemAccessor.TakeItem() => TakeItem();
        ReadOnlyItemContainer IItemAccessor.TakeItem(int count) => TakeItem(count);
        ReadOnlyItemContainer IItemAccessor.TakeItem(IItem item, int count)
        {
            if (item.IsNot<TItem>(out var typedItem))
                return default;

            return TakeItem(typedItem, count);
        }
    }
}
