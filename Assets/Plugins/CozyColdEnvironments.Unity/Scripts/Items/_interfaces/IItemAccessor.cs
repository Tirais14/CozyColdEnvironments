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
        void CopyItemFrom(IItemContainerInfo<TItem> itemContainer);

        void IItemAccessor.CopyItemFrom(IItemContainerInfo itemContainer)
        {
            if (itemContainer.IsNot<IItemContainerInfo<TItem>>(out var typed))
                return;

            CopyItemFrom(typed);
        }
    }
}
