#nullable enable
using CCEnvs.FuncLanguage;
using CCEnvs.TypeMatching;

namespace CCEnvs.UnityX.Items
{
    public interface IItemAccessor
    {
        Maybe<IItemContainerInfo> PutItem(IItem? item, int count = 1);
        Maybe<IItemContainerInfo> PutItemFrom(IItemContainer itemContainer, int count);
        Maybe<IItemContainerInfo> PutItemFrom(IItemContainer itemContainer);

        Maybe<IItemContainerInfo> TakeItem();
        Maybe<IItemContainerInfo> TakeItem(int count);
        Maybe<IItemContainerInfo> TakeItem(IItem item, int count);

        void CopyItemFrom(IItemContainerInfo itemContainer);

        void Clear();
    }

    public interface IItemAccessor<TItem, TItemContainerInfo> : IItemAccessor
        where TItem : IItem
        where TItemContainerInfo : IItemContainerInfo
    {
        Maybe<TItemContainerInfo> PutItem(TItem? item, int count = 1);
        Maybe<TItemContainerInfo> PutItemFrom(IItemContainer<TItem> itemContainer, int count);
        Maybe<TItemContainerInfo> PutItemFrom(IItemContainer<TItem> itemContainer);

        new Maybe<TItemContainerInfo> TakeItem();
        new Maybe<TItemContainerInfo> TakeItem(int count);
        Maybe<TItemContainerInfo> TakeItem(TItem item, int count);

        void CopyItemFrom(IItemContainerInfo<TItem> itemContainer);

        Maybe<IItemContainerInfo> IItemAccessor.TakeItem()
        {
            return TakeItem().Cast<IItemContainerInfo>();
        }

        Maybe<IItemContainerInfo> IItemAccessor.TakeItem(int count)
        {
            return TakeItem(count).Cast<IItemContainerInfo>();
        }

        Maybe<IItemContainerInfo> IItemAccessor.TakeItem(IItem item, int count)
        {
            return TakeItem(item, count).Cast<IItemContainerInfo>();
        }

        void IItemAccessor.CopyItemFrom(IItemContainerInfo itemContainer)
        {
            if (itemContainer.IsNot<TItemContainerInfo>(out var typed))
                return;

            CopyItemFrom(typed);
        }

        Maybe<IItemContainerInfo> IItemAccessor.PutItem(IItem? item, int count)
        {
            if (item.IsNot<TItem>(out var typedItem))
                return new ItemContainer(item, count, capacity: int.MaxValue);

            return PutItem(typedItem, count).Cast<IItemContainerInfo>();
        }

        Maybe<IItemContainerInfo> IItemAccessor.PutItemFrom(IItemContainer itemContainer)
        {
            if (itemContainer.IsNot<IItemContainer<TItem>>(out var typedContainer))
                return ((IItemContainerInfo)itemContainer.ShallowClone()).Maybe();

            return PutItemFrom(typedContainer).Cast<IItemContainerInfo>();
        }

        Maybe<IItemContainerInfo> IItemAccessor.PutItemFrom(IItemContainer itemContainer, int count)
        {
            if (itemContainer.IsNot<IItemContainer<TItem>>(out var typedContainer))
                return ((IItemContainerInfo)itemContainer.ShallowClone()).Maybe();

            return PutItemFrom(typedContainer, count).Cast<IItemContainerInfo>();
        }
    }
}
