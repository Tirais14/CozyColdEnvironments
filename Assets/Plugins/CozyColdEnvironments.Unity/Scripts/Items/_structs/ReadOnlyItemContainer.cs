using CCEnvs.FuncLanguage;
using R3;
using System.Collections.Generic;

#nullable enable
namespace CCEnvs.UnityX.Items
{
    public readonly struct ReadOnlyItemContainer 
        :
        IItemContainerInfo,
        IItemContainer
    {
        public readonly Maybe<IItem> Item { get; }

        public readonly int ItemCount { get; }

        public readonly bool IsEmpty => Item.IsNone || ItemCount == 0;

        readonly Maybe<IInventory> IItemContainerInfoItemless.ParentInventory {
            get => Maybe<IInventory>.None;
            set => _ = value;
        }

        readonly int IItemContainerInfoItemless.Capacity { get => ItemCount; set => _ = value; }

        readonly bool IItemContainerInfoItemless.IsFull => false;

        readonly int IItemContainerInfoItemless.FreeSpace => int.MaxValue;

        bool IItemContainer.IsReadOnlyContainer => true;

        bool IItemContainer.UnlockCapacity { get => false; set => _ = value; }

        public ReadOnlyItemContainer(IItem? item, int itemCount)
        {
            Item = item.Maybe();
            ItemCount = itemCount;
        }

        public readonly bool ContainsItem() => !IsEmpty;
        public readonly bool ContainsItem(IItem? item)
        {
            return ContainsItem() && EqualityComparer<IItem?>.Default.Equals(Item.GetValue(), item);
        }
        public readonly bool ContainsItem(IItem? item, int count)
        {
            return ContainsItem(item) && ItemCount >= count;
        }

        readonly bool IItemContainerInfoItemless.CanPut() => false;
        readonly bool IItemContainerInfoItemless.CanPut(IItem? item) => false;
        readonly bool IItemContainerInfoItemless.CanPut(IItem? item, int count) => false;

        readonly Maybe<int> IItemContainerInfoItemless.GetContainerID() => Maybe<int>.None;

        readonly Maybe<ReadOnlyItemContainer> IItemAccessor.PutItem(IItem? item, int count)
        {
            return Maybe<ReadOnlyItemContainer>.None;
        }

        readonly Maybe<ReadOnlyItemContainer> IItemAccessor.PutItemFrom(
            IItemContainer itemContainer,
            int count
            )
        {
            return Maybe<ReadOnlyItemContainer>.None;
        }
        readonly Maybe<ReadOnlyItemContainer> IItemAccessor.PutItemFrom(
            IItemContainer itemContainer
            )
        {
            return Maybe<ReadOnlyItemContainer>.None;
        }

        readonly Maybe<ReadOnlyItemContainer> IItemAccessor.TakeItem()
        {
            return Maybe<ReadOnlyItemContainer>.None;
        }
        readonly Maybe<ReadOnlyItemContainer> IItemAccessor.TakeItem(int count)
        {
            return Maybe<ReadOnlyItemContainer>.None;
        }
        readonly Maybe<ReadOnlyItemContainer> IItemAccessor.TakeItem(IItem item, int count)
        {
            return Maybe<ReadOnlyItemContainer>.None;
        }

        readonly void IItemAccessor.CopyItemFrom(IItemContainerInfo itemContainer) {  }

        readonly void IItemAccessor.Clear() { }

        readonly IItemContainer IShallowCloneable<IItemContainer>.ShallowClone()
        {
            return new ReadOnlyItemContainer(Item.GetValue(), ItemCount);
        }

        Observable<Maybe<IItem>> IItemContainerInfo.ObserveItem()
        {
            return Observable.Return(Item);
        }

        Observable<int> IItemContainerInfoItemless.ObserveItemCount()
        {
            return Observable.Return(ItemCount);    
        }
    }

    public readonly struct ReadOnlyItemContainer<TItem>
        :
        IItemContainerInfo<TItem>,
        IItemContainer<TItem>

        where TItem : IItem
    {
        public readonly Maybe<TItem> Item { get; }

        public readonly int ItemCount { get; }

        public readonly bool IsEmpty => Item.IsNone || ItemCount == 0;

        readonly Maybe<IInventory> IItemContainerInfoItemless.ParentInventory {
            get => Maybe<IInventory>.None;
            set => _ = value;
        }

        readonly int IItemContainerInfoItemless.Capacity { get => ItemCount; set => _ = value; }

        readonly bool IItemContainerInfoItemless.IsFull => false;

        readonly int IItemContainerInfoItemless.FreeSpace => int.MaxValue;

        bool IItemContainer.IsReadOnlyContainer => true;

        bool IItemContainer.UnlockCapacity { get => false; set => _ = value; }

        public ReadOnlyItemContainer(TItem? item, int itemCount)
        {
            Item = item.Maybe();
            ItemCount = itemCount;
        }

        public readonly bool ContainsItem() => !IsEmpty;
        public readonly bool ContainsItem(IItem? item)
        {
            return ContainsItem() && EqualityComparer<IItem?>.Default.Equals(Item.GetValue(), item);
        }
        public readonly bool ContainsItem(IItem? item, int count)
        {
            return ContainsItem(item) && ItemCount >= count;
        }

        readonly bool IItemContainerInfoItemless.CanPut() => false;
        readonly bool IItemContainerInfoItemless.CanPut(IItem? item) => false;
        readonly bool IItemContainerInfoItemless.CanPut(IItem? item, int count) => false;

        readonly Maybe<int> IItemContainerInfoItemless.GetContainerID() => Maybe<int>.None;

        readonly Maybe<ReadOnlyItemContainer> IItemAccessor.PutItem(IItem? item, int count)
        {
            return Maybe<ReadOnlyItemContainer>.None;
        }

        readonly Maybe<ReadOnlyItemContainer> IItemAccessor.PutItemFrom(
            IItemContainer itemContainer,
            int count
            )
        {
            return Maybe<ReadOnlyItemContainer>.None;
        }
        readonly Maybe<ReadOnlyItemContainer> IItemAccessor.PutItemFrom(
            IItemContainer itemContainer
            )
        {
            return Maybe<ReadOnlyItemContainer>.None;
        }

        readonly Maybe<ReadOnlyItemContainer> IItemAccessor.TakeItem()
        {
            return Maybe<ReadOnlyItemContainer>.None;
        }
        readonly Maybe<ReadOnlyItemContainer> IItemAccessor.TakeItem(int count)
        {
            return Maybe<ReadOnlyItemContainer>.None;
        }
        readonly Maybe<ReadOnlyItemContainer> IItemAccessor.TakeItem(IItem item, int count)
        {
            return Maybe<ReadOnlyItemContainer>.None;
        }

        readonly void IItemAccessor<TItem>.CopyItemFrom(IItemContainerInfo<TItem> itemContainer) { }

        readonly void IItemAccessor.Clear() { }

        readonly IItemContainer IShallowCloneable<IItemContainer>.ShallowClone()
        {
            return new ReadOnlyItemContainer(Item.GetValue(), ItemCount);
        }

        Observable<Maybe<TItem>> IItemContainerInfo<TItem>.ObserveItem()
        {
            return Observable.Return(Item);
        }

        Observable<int> IItemContainerInfoItemless.ObserveItemCount()
        {
            return Observable.Return(ItemCount);
        }
    }
}
