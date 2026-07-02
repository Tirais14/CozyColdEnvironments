using R3;
using System;
using System.Collections.Generic;

#nullable enable
namespace CCEnvs.UnityX.Items
{
    public readonly struct ReadOnlyItemContainer
        :
        IItemContainerInfo,
        IItemContainer,
        IEquatable<ReadOnlyItemContainer>
    {
        public static ReadOnlyItemContainer Empty { get; } = new(null, 0);

        public readonly IItem? Item { get; }

        public readonly int ItemCount { get; }

        public readonly bool IsEmpty => ItemCount == 0 || Item.IsNull();

        readonly IInventory? IItemContainer.ParentInventory => null;

        readonly int IItemContainerInfoItemless.FreeSpace => int.MaxValue;
        readonly int IItemContainer.Capacity { get => ItemCount; set => _ = value; }

        readonly int? IItemContainer.ID {
            get => null;
            set => _ = value;
        }

        readonly bool IItemContainerInfoItemless.IsFull => false;

        bool IItemContainer.IsReadOnlyContainer => true;

        bool IItemContainer.IgnoreMaxItemCount { get => false; set => _ = value; }

        public ReadOnlyItemContainer(IItem? item, int itemCount)
        {
            if (itemCount <= 0)
            {
                Item = default;
                ItemCount = 0;
            }

            Item = item;
            ItemCount = itemCount;
        }

        public static bool operator ==(ReadOnlyItemContainer left, ReadOnlyItemContainer right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(ReadOnlyItemContainer left, ReadOnlyItemContainer right)
        {
            return !(left == right);
        }

        public readonly bool ContainsItem() => !IsEmpty;
        public readonly bool ContainsItem(IItem? item)
        {
            return ContainsItem() && EqualityComparer<IItem?>.Default.Equals(Item, item);
        }
        public readonly bool ContainsItem(IItem? item, int count)
        {
            return ContainsItem(item) && ItemCount >= count;
        }

        public readonly ReadOnlyItemContainer<TItem> Convert<TItem>()
            where TItem : IItem
        {
            return new ReadOnlyItemContainer<TItem>(Item.As<TItem>(), ItemCount);
        }

        public override bool Equals(object? obj)
        {
            return obj is ReadOnlyItemContainer container && Equals(container);
        }

        public bool Equals(ReadOnlyItemContainer other)
        {
            return EqualityComparer<IItem?>.Default.Equals(Item, other.Item) &&
                   ItemCount == other.ItemCount;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Item, ItemCount);
        }

        public override string ToString()
        {
            return ToStringBuilder.CreatePooled()
                .AddProperty(nameof(Item), Item)
                .AddProperty(nameof(ItemCount), ItemCount)
                .AddProperty(nameof(IsEmpty), IsEmpty)
                .ToStringAndDispose();
        }

        readonly bool IItemContainerInfoItemless.CanPut() => false;
        readonly bool IItemContainerInfoItemless.CanPut(IItem? item) => false;
        readonly bool IItemContainerInfoItemless.CanPut(IItem? item, int count) => false;

        readonly ReadOnlyItemContainer IItemAccessor.PutItem(IItem? item, int count)
        {
            return Empty;
        }
        readonly ReadOnlyItemContainer IItemAccessor.PutItem(IItemContainerInfo? containerInfo)
        {
            return Empty;
        }

        readonly ReadOnlyItemContainer IItemAccessor.PutItem<TItemContainerInfo>(TItemContainerInfo containerInfo)
        {
            return Empty;
        }

        readonly ReadOnlyItemContainer IItemAccessor.PutItemFrom(
            IItemContainer? itemContainer,
            int count
            )
        {
            return Empty;
        }
        readonly ReadOnlyItemContainer IItemAccessor.PutItemFrom(
            IItemContainer? itemContainer
            )
        {
            return Empty;
        }

        readonly ReadOnlyItemContainer IItemAccessor.TakeItem()
        {
            return Empty;
        }
        readonly ReadOnlyItemContainer IItemAccessor.TakeItem(int count)
        {
            return Empty;
        }
        readonly ReadOnlyItemContainer IItemAccessor.TakeItem(IItem? item, int count)
        {
            return Empty;
        }

        readonly void IItemAccessor.CopyItemFrom(IItemContainer _) { }

        readonly void IItemAccessor.Clear() { }

        readonly IItemContainer IShallowCloneable<IItemContainer>.ShallowClone()
        {
            return new ReadOnlyItemContainer(Item, ItemCount);
        }

        readonly ReadOnlyItemContainer IItemContainer.ToReadOnly() => this;

        readonly Observable<IItem?> IItemContainerInfo.ObserveItem()
        {
            return Observable.Return(Item);
        }

        readonly Observable<int> IItemContainerInfoItemless.ObserveItemCount()
        {
            return Observable.Return(ItemCount);
        }

        readonly void IItemContainer.SetParentInventory(IInventory? inventory) { }
    }

    public readonly struct ReadOnlyItemContainer<TItem>
        :
        IItemContainerInfo<TItem>,
        IItemContainer<TItem>,
        IEquatable<ReadOnlyItemContainer<TItem>>

        where TItem : IItem
    {
        public static ReadOnlyItemContainer<TItem> Empty { get; } = new(default, 0);

        public readonly TItem? Item { get; }

        public readonly int ItemCount { get; }

        public readonly bool IsEmpty => ItemCount == 0 || Item.IsNull();

        readonly IInventory? IItemContainer.ParentInventory {
            get => null;
        }

        readonly bool IItemContainerInfoItemless.IsFull => false;

        readonly int IItemContainerInfoItemless.FreeSpace => int.MaxValue;
        readonly int IItemContainer.Capacity { get => ItemCount; set => _ = value; }

        readonly int? IItemContainer.ID {
            get => null;
            set => _ = value;
        }

        bool IItemContainer.IsReadOnlyContainer => true;

        bool IItemContainer.IgnoreMaxItemCount { get => false; set => _ = value; }

        public ReadOnlyItemContainer(TItem? item, int itemCount)
        {
            Item = item;
            ItemCount = itemCount;
        }

        public static implicit operator ReadOnlyItemContainer(ReadOnlyItemContainer<TItem> instance)
        {
            return new ReadOnlyItemContainer(instance.Item, instance.ItemCount);
        }

        public static bool operator ==(ReadOnlyItemContainer<TItem> left, ReadOnlyItemContainer<TItem> right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(ReadOnlyItemContainer<TItem> left, ReadOnlyItemContainer<TItem> right)
        {
            return !(left == right);
        }

        public readonly bool ContainsItem() => !IsEmpty;
        public readonly bool ContainsItem(IItem? item)
        {
            return ContainsItem() && EqualityComparer<IItem?>.Default.Equals(Item, item);
        }
        public readonly bool ContainsItem(IItem? item, int count)
        {
            return ContainsItem(item) && ItemCount >= count;
        }

        public readonly ReadOnlyItemContainer ToUntyped() => new(Item, ItemCount);

        public readonly ReadOnlyItemContainer<TItemOut> Convert<TItemOut>()
            where TItemOut : IItem
        {
            return new ReadOnlyItemContainer<TItemOut>(
                Item.As<TItemOut>(),
                ItemCount
                );
        }

        public readonly override bool Equals(object? obj)
        {
            return obj is ReadOnlyItemContainer<TItem> container && Equals(container);
        }
        public readonly bool Equals(ReadOnlyItemContainer<TItem> other)
        {
            return EqualityComparer<IItem?>.Default.Equals(Item, other.Item) &&
                   ItemCount == other.ItemCount;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Item, ItemCount);
        }

        public override string ToString()
        {
            return ToStringBuilder.CreatePooled()
                .AddProperty(nameof(Item), Item)
                .AddProperty(nameof(ItemCount), ItemCount)
                .AddProperty(nameof(IsEmpty), IsEmpty)
                .ToStringAndDispose();
        }

        readonly bool IItemContainerInfoItemless.CanPut() => false;
        readonly bool IItemContainerInfoItemless.CanPut(IItem? item) => false;
        readonly bool IItemContainerInfoItemless.CanPut(IItem? item, int count) => false;

        readonly ReadOnlyItemContainer<TItem> IItemAccessor<TItem>.PutItem(TItem? item, int count)
        {
            return Empty;
        }
        readonly ReadOnlyItemContainer<TItem> IItemAccessor<TItem>.PutItem(IItemContainerInfo<TItem>? containerInfo)
        {
            return Empty;
        }

        readonly ReadOnlyItemContainer<TItem> IItemAccessor<TItem>.PutItem<TItemContainerInfo>(TItemContainerInfo containerInfo)
        {
            return Empty;
        }

        readonly ReadOnlyItemContainer<TItem> IItemAccessor<TItem>.PutItemFrom(
            IItemContainer<TItem>? itemContainer,
            int count
            )
        {
            return Empty;
        }
        readonly ReadOnlyItemContainer<TItem> IItemAccessor<TItem>.PutItemFrom(
            IItemContainer<TItem>? itemContainer
            )
        {
            return Empty;
        }

        readonly ReadOnlyItemContainer<TItem> IItemAccessor<TItem>.TakeItem()
        {
            return Empty;
        }
        readonly ReadOnlyItemContainer<TItem> IItemAccessor<TItem>.TakeItem(int count)
        {
            return Empty;
        }
        readonly ReadOnlyItemContainer<TItem> IItemAccessor<TItem>.TakeItem(TItem? item, int count)
        {
            return Empty;
        }

        readonly void IItemAccessor<TItem>.CopyItemFrom(IItemContainer<TItem> _) { }

        readonly void IItemAccessor.Clear() { }

        readonly IItemContainer IShallowCloneable<IItemContainer>.ShallowClone()
        {
            return new ReadOnlyItemContainer<TItem>(Item, ItemCount);
        }

        readonly ReadOnlyItemContainer<TItem> IItemContainer<TItem>.ToReadOnly() => this;

        readonly Observable<TItem?> IItemContainerInfo<TItem>.ObserveItem()
        {
            return Observable.Return(Item);
        }

        readonly Observable<int> IItemContainerInfoItemless.ObserveItemCount()
        {
            return Observable.Return(ItemCount);
        }

        readonly void IItemContainer.SetParentInventory(IInventory? inventory) { }
    }
}
