#nullable enable
using CCEnvs.FuncLanguage;
using CCEnvs.TypeMatching;

namespace CCEnvs.UnityX.Items
{
    public interface IItemAccessor
    {
        Maybe<ReadOnlyItemContainer> PutItem(IItem? item, int count = 1);
        Maybe<ReadOnlyItemContainer> PutItemFrom(IItemContainer itemContainer, int count);
        Maybe<ReadOnlyItemContainer> PutItemFrom(IItemContainer itemContainer);

        Maybe<ReadOnlyItemContainer> TakeItem();
        Maybe<ReadOnlyItemContainer> TakeItem(int count);
        Maybe<ReadOnlyItemContainer> TakeItem(IItem item, int count);

        void CopyItemFrom(IItemContainerInfo itemContainer);

        void Clear();
    }

    public interface IItemAccessor<TItem> : IItemAccessor
        where TItem : IItem
    {
        Maybe<ReadOnlyItemContainer<TItem>> PutItem(TItem? item, int count = 1);
        Maybe<ReadOnlyItemContainer<TItem>> PutItemFrom(IItemContainer<TItem> itemContainer, int count);
        Maybe<ReadOnlyItemContainer<TItem>> PutItemFrom(IItemContainer<TItem> itemContainer);

        new Maybe<ReadOnlyItemContainer<TItem>> TakeItem();
        new Maybe<ReadOnlyItemContainer<TItem>> TakeItem(int count);
        Maybe<ReadOnlyItemContainer<TItem>> TakeItem(TItem item, int count);

        void CopyItemFrom(IItemContainerInfo<TItem> itemContainer);

        void IItemAccessor.CopyItemFrom(IItemContainerInfo itemContainer)
        {
            if (itemContainer.IsNot<IItemContainerInfo<TItem>>(out var typed))
                return;

            CopyItemFrom(typed);
        }

        Maybe<ReadOnlyItemContainer> IItemAccessor.PutItem(IItem? item, int count)
        {
            return PutItem(item.As<TItem>(), count).Select(restItems => restItems.ToUntyped());
        }

        Maybe<ReadOnlyItemContainer> IItemAccessor.PutItemFrom(IItemContainer itemContainer)
        {
            if (itemContainer.IsNot<IItemContainer<TItem>>(out var typedContainer))
                return default;

            return PutItemFrom(typedContainer).Select(restItems => restItems.ToUntyped());
        }
        Maybe<ReadOnlyItemContainer> IItemAccessor.PutItemFrom(IItemContainer itemContainer, int count)
        {
            if (itemContainer.IsNot<IItemContainer<TItem>>(out var typedContainer))
                return default;

            return PutItemFrom(typedContainer, count).Select(restItems => restItems.ToUntyped());
        }

        Maybe<ReadOnlyItemContainer> IItemAccessor.TakeItem()
        {
            return TakeItem().Select(restItems => restItems.ToUntyped());
        }
        Maybe<ReadOnlyItemContainer> IItemAccessor.TakeItem(int count)
        {
            return TakeItem(count).Select(restItems => restItems.ToUntyped());
        }
        Maybe<ReadOnlyItemContainer> IItemAccessor.TakeItem(IItem item, int count)
        {
            if (item.IsNot<TItem>(out var typedItem))
                return default;

            return TakeItem(typedItem, count).Select(restItems => restItems.ToUntyped());
        }
    }
}
