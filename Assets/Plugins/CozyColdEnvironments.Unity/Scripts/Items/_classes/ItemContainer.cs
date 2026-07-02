using R3;
using System;
using System.Linq;

#nullable enable
namespace CCEnvs.UnityX.Items
{
    public class ItemContainer
        :
        ItemContainerBase<IItem, IItemContainer, IItemContainerInfo, ReadOnlyItemContainer>,
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

        protected override ReadOnlyItemContainer CreateReadOnlyItemContainer()
        {
            return ReadOnlyItemContainer.Empty;
        }

        protected override ReadOnlyItemContainer CreateReadOnlyItemContainer(IItem? item, int itemCount)
        {
            return new ReadOnlyItemContainer(item, itemCount);
        }
    }

    public class ItemContainer<TItem>
        :
        ItemContainerBase<TItem, IItemContainer<TItem>, IItemContainerInfo<TItem>, ReadOnlyItemContainer<TItem>>,
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

        protected override ReadOnlyItemContainer<TItem> CreateReadOnlyItemContainer()
        {
            return ReadOnlyItemContainer<TItem>.Empty;
        }

        protected override ReadOnlyItemContainer<TItem> CreateReadOnlyItemContainer(TItem? item, int itemCount)
        {
            return new ReadOnlyItemContainer<TItem>(Item, itemCount);
        }
    }
}
