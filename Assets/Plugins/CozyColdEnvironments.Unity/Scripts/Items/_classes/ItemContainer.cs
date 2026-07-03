#nullable enable
namespace CCEnvs.UnityX.Items
{
    public class ItemContainer
        :
        ItemContainerBase<
            IItem,
            IItemContainer, 
            IItemContainerInfo, 
            ReadOnlyItemContainer, 
            LargeReadOnlyItemContainer,
            ItemAccessorPutItemEvent,
            ItemAccessorTakeItemEvent
            >,

        IItemContainer
    {
        bool IItemContainer.IsReadOnlyContainer => false;

        public ItemContainer()
            :
            base()
        {
        }

        public ItemContainer(
            IItem? item = null,
            int count = 1,
            int capacity = 0
            )
            :
            base(
                item,
                count,
                capacity
                )
        {
        }

        ~ItemContainer() => Dispose();

        public override IItemContainer ShallowClone()
        {
            return new ItemContainer(Item, ItemCount, Capacity);
        }

        protected override ReadOnlyItemContainer CreateReadOnlyContainer()
        {
            return ReadOnlyItemContainer.Empty;
        }

        protected override ReadOnlyItemContainer CreateReadOnlyContainer(IItem? item, int itemCount)
        {
            return new ReadOnlyItemContainer(item, itemCount);
        }

        protected override LargeReadOnlyItemContainer CreateLargeReadOnlyContainer()
        {
            return LargeReadOnlyItemContainer.Empty;
        }

        protected override LargeReadOnlyItemContainer CreateLargeReadOnlyContainer(IItem? item, long itemCount)
        {
            return new LargeReadOnlyItemContainer(item, itemCount);
        }

        protected override ItemAccessorPutItemEvent CreatePutItemEvent(IItem item, int itemCount)
        {
            return new ItemAccessorPutItemEvent(item, itemCount);
        }

        protected override ItemAccessorTakeItemEvent CreateTakeItemEvent(IItem item, int itemCount)
        {
            return new ItemAccessorTakeItemEvent(item, itemCount);
        }

        protected override LargeReadOnlyItemContainer ConvertReadOnlyContainerToLarge(ReadOnlyItemContainer readOnlyContainer)
        {
            return readOnlyContainer;
        }

        protected override IItem? GetLargeReadOnlyContainerItem(LargeReadOnlyItemContainer largeReadOnlyContainer)
        {
            return largeReadOnlyContainer.Item;
        }

        protected override long GetLargeReadOnlyContainerItemCount(LargeReadOnlyItemContainer largeReadOnlyContainer)
        {
            return largeReadOnlyContainer.ItemCount;
        }
    }

    public class ItemContainer<TItem>
        :
        ItemContainerBase<
            TItem,
            IItemContainer<TItem>, 
            IItemContainerInfo<TItem>,
            ReadOnlyItemContainer<TItem>,
            LargeReadOnlyItemContainer<TItem>,
            ItemAccessorPutItemEvent<TItem>,
            ItemAccessorTakeItemEvent<TItem>
            >,
        IItemContainer<TItem>

        where TItem : class, IItem
    {
        bool IItemContainer.IsReadOnlyContainer => false;

        public ItemContainer(
            TItem? item = default,
            int count = 1,
            int capacity = 0
            )
            :
            base(
                item, 
                count,
                capacity
                )
        {
        }

        public ItemContainer()
            :
            base()
        {
        }

        ~ItemContainer() => Dispose();

        public static explicit operator ReadOnlyItemContainer<TItem>(ItemContainer<TItem> instance)
        {
            return instance.ToReadOnly();
        }

        public override IItemContainer ShallowClone()
        {
            return new ItemContainer<TItem>(Item, ItemCount, Capacity);
        }

        protected override ReadOnlyItemContainer<TItem> CreateReadOnlyContainer()
        {
            return ReadOnlyItemContainer<TItem>.Empty;
        }

        protected override ReadOnlyItemContainer<TItem> CreateReadOnlyContainer(TItem? item, int itemCount)
        {
            return new ReadOnlyItemContainer<TItem>(Item, itemCount);
        }

        protected override LargeReadOnlyItemContainer<TItem> CreateLargeReadOnlyContainer()
        {
            return LargeReadOnlyItemContainer<TItem>.Empty;
        }

        protected override LargeReadOnlyItemContainer<TItem> CreateLargeReadOnlyContainer(TItem? item, long itemCount)
        {
            return new LargeReadOnlyItemContainer<TItem>(item, itemCount);
        }

        protected override LargeReadOnlyItemContainer<TItem> ConvertReadOnlyContainerToLarge(ReadOnlyItemContainer<TItem> readOnlyContainer)
        {
            return readOnlyContainer;
        }

        protected override ItemAccessorPutItemEvent<TItem> CreatePutItemEvent(TItem item, int itemCount)
        {
            return new ItemAccessorPutItemEvent<TItem>(item, itemCount);
        }

        protected override ItemAccessorTakeItemEvent<TItem> CreateTakeItemEvent(TItem item, int itemCount)
        {
            return new ItemAccessorTakeItemEvent<TItem>(item, itemCount);
        }

        protected override TItem? GetLargeReadOnlyContainerItem(LargeReadOnlyItemContainer<TItem> largeReadOnlyContainer)
        {
            return largeReadOnlyContainer.Item;
        }

        protected override long GetLargeReadOnlyContainerItemCount(LargeReadOnlyItemContainer<TItem> largeReadOnlyContainer)
        {
            return largeReadOnlyContainer.ItemCount;
        }
    }
}
