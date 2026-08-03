#nullable enable
using CCEnvs.TypeMatching;
using R3;
using UnityEditor;

namespace CCEnvs.UnityX.Items
{
    public interface IItemAccessor
    {
        ReadOnlyItemContainer PutItem(IItem? item, int count = 1);
        ReadOnlyItemContainer PutItem(IItemContainerInfo? containerInfo);
        ReadOnlyItemContainer PutItem(ReadOnlyItemContainer readOnlyContainer);
        LargeReadOnlyItemContainer PutItem(IItem? item, long count);
        LargeReadOnlyItemContainer PutItem(LargeReadOnlyItemContainer largeReadOnlyContainer);

        ReadOnlyItemContainer PutItemFrom(IItemContainer? container, int count);
        ReadOnlyItemContainer PutItemFrom(IItemContainer? container);

        //ReadOnlyItemContainer PutItemTo(IItemContainer? container);
        //ReadOnlyItemContainer PutItemTo(IItemContainer? container, int count);

        ReadOnlyItemContainer TakeItem();
        ReadOnlyItemContainer TakeItem(int count);
        ReadOnlyItemContainer TakeItem(IItem? item, int count);

        void CopyItemFrom(IItemContainer itemContainer);

        void Clear();

        Observable<ItemAccessorPutItemEvent> ObservePutItem();

        Observable<ItemAccessorTakeItemEvent> ObserveTakeItem();
    }

    public interface IItemAccessor<TItem> : IItemAccessor
        where TItem : IItem
    {
        ReadOnlyItemContainer<TItem> PutItem(TItem? item, int count = 1);
        ReadOnlyItemContainer<TItem> PutItem(IItemContainerInfo<TItem>? containerInfo);
        ReadOnlyItemContainer<TItem> PutItem(ReadOnlyItemContainer<TItem> readOnlyContainer);
        LargeReadOnlyItemContainer<TItem> PutItem(TItem? item, long count);
        LargeReadOnlyItemContainer<TItem> PutItem(LargeReadOnlyItemContainer<TItem> largeReadOnlyContainer);

        ReadOnlyItemContainer<TItem> PutItemFrom(IItemContainer<TItem>? container, int count);
        ReadOnlyItemContainer<TItem> PutItemFrom(IItemContainer<TItem>? container);

        //ReadOnlyItemContainer PutItemTo(IItemContainer<TItem>? container);
        //ReadOnlyItemContainer PutItemTo(IItemContainer<TItem>? container, int count);

        new ReadOnlyItemContainer<TItem> TakeItem();
        new ReadOnlyItemContainer<TItem> TakeItem(int count);
        ReadOnlyItemContainer<TItem> TakeItem(TItem? item, int count);

        void CopyItemFrom(IItemContainer<TItem> container);

        new Observable<ItemAccessorPutItemEvent<TItem>> ObservePutItem();

        new Observable<ItemAccessorTakeItemEvent<TItem>> ObserveTakeItem();

        void IItemAccessor.CopyItemFrom(IItemContainer container)
        {
            if (container.IsNot<IItemContainer<TItem>>(out var typed))
                return;

            CopyItemFrom(typed);
        }

        ReadOnlyItemContainer IItemAccessor.PutItem(IItem? item, int count)
        {
            return PutItem(item.As<TItem>(), count);
        }
        ReadOnlyItemContainer IItemAccessor.PutItem(IItemContainerInfo? containerInfo)
        {
            if (containerInfo.IsNot<IItemContainerInfo<TItem>>(out var typedContainerInfo))
                return ReadOnlyItemContainer.Empty;

            return PutItem(typedContainerInfo);
        }
        ReadOnlyItemContainer IItemAccessor.PutItem(ReadOnlyItemContainer readOnlyContainer)
        {
            return PutItem(readOnlyContainer.Convert<TItem>());
        }
        LargeReadOnlyItemContainer IItemAccessor.PutItem(IItem? item, long count)
        {
            if (item.IsNot<TItem>(out var typedItem))
                return LargeReadOnlyItemContainer.Empty;

            return PutItem(typedItem, count);
        }
        LargeReadOnlyItemContainer IItemAccessor.PutItem(LargeReadOnlyItemContainer largeReadOnlyContainer)
        {
            var typedLargeReadOnlyContainer = largeReadOnlyContainer.Convert<TItem>();

            if (typedLargeReadOnlyContainer.IsEmpty)
                return LargeReadOnlyItemContainer.Empty;

            return PutItem(typedLargeReadOnlyContainer);
        }

        ReadOnlyItemContainer IItemAccessor.PutItemFrom(IItemContainer? container)
        {
            if (container.IsNot<IItemContainer<TItem>>(out var typedContainer))
                return default;

            return PutItemFrom(typedContainer);
        }
        ReadOnlyItemContainer IItemAccessor.PutItemFrom(IItemContainer? container, int count)
        {
            if (container.IsNot<IItemContainer<TItem>>(out var typedContainer))
                return default;

            return PutItemFrom(typedContainer, count);
        }

        //ReadOnlyItemContainer IItemAccessor.PutItemTo(IItemContainer? container)
        //{
        //    if (container.IsNot<IItemContainer<TItem>>(out var typedContainer))
        //        return this;

        //    return PutItemTo(typedContainer);
        //}
        //ReadOnlyItemContainer IItemAccessor.PutItemTo(IItemContainer? container, int count)
        //{
        //    if (container.IsNot<IItemContainer<TItem>>(out var typedContainer))
        //        return this;

        //    return PutItemTo(typedContainer, count);
        //}

        ReadOnlyItemContainer IItemAccessor.TakeItem() => TakeItem();
        ReadOnlyItemContainer IItemAccessor.TakeItem(int count) => TakeItem(count);
        ReadOnlyItemContainer IItemAccessor.TakeItem(IItem? item, int count)
        {
            if (item.IsNot<TItem>(out var typedItem))
                return default;

            return TakeItem(typedItem, count);
        }

        Observable<ItemAccessorPutItemEvent> IItemAccessor.ObservePutItem()
        {
            return ObservePutItem().Select(ev => ev.AsUntyped());
        }

        Observable<ItemAccessorTakeItemEvent> IItemAccessor.ObserveTakeItem()
        {
            return ObserveTakeItem().Select(ev => ev.AsUntyped());
        }
    }
}
