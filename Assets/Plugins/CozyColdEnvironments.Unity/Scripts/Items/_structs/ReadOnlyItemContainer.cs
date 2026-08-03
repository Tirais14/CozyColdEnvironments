using R3;
using System;
using System.Collections.Generic;

#nullable enable
namespace CCEnvs.UnityX.Items
{
    public readonly struct ReadOnlyItemContainer
        :
        IItemContainerInfo,
        IEquatable<ReadOnlyItemContainer>
    {
        public static ReadOnlyItemContainer Empty { get; } = new(null, 0);

        public readonly IItem? Item { get; }

        public readonly int ItemCount { get; }

        public readonly bool IsEmpty => ItemCount == 0 || Item.IsNull();

        readonly int IItemContainerInfoItemless.FreeSpace => int.MaxValue;

        readonly bool IItemContainerInfoItemless.IsFull => false;

        public ReadOnlyItemContainer(IItem? item, int itemCount)
        {
            if (itemCount <= 0 || item.IsNull())
            {
                Item = default;
                ItemCount = 0;
                return;
            }

            Item = item;
            ItemCount = itemCount;
        }

        public static implicit operator LargeReadOnlyItemContainer(ReadOnlyItemContainer instance)
        {
            return instance.AsLarge();
        }

        public static bool operator ==(ReadOnlyItemContainer left, ReadOnlyItemContainer right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(ReadOnlyItemContainer left, ReadOnlyItemContainer right)
        {
            return !(left == right);
        }

        public readonly ReadOnlyItemContainer PutItemTo(IItemContainer? container)
        {
            if (container.IsNull())
                return this;

            return container.PutItem(Item, ItemCount);
        }

        public readonly ReadOnlyItemContainer PutItemTo(IItemContainer? container, int count)
        {
            if (container.IsNull())
                return this;

            return container.PutItem(Item, count);
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

        public LargeReadOnlyItemContainer AsLarge()
        {
            return new LargeReadOnlyItemContainer(Item, ItemCount);
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

        readonly bool IItemContainerInfoItemless.CanPutItem() => false;
        readonly bool IItemContainerInfoItemless.CanPutItem(IItem? item) => false;
        readonly bool IItemContainerInfoItemless.CanPutItem(IItem? item, int count) => false;

        readonly Observable<IItem?> IItemContainerInfo.ObserveItem()
        {
            return Observable.Return(Item);
        }

        readonly Observable<int> IItemContainerInfoItemless.ObserveItemCount()
        {
            return Observable.Return(ItemCount);
        }
    }

    public readonly struct ReadOnlyItemContainer<TItem>
        :
        IItemContainerInfo<TItem>,
        IEquatable<ReadOnlyItemContainer<TItem>>

        where TItem : IItem
    {
        public static ReadOnlyItemContainer<TItem> Empty { get; } = new(default, 0);

        public readonly TItem? Item { get; }

        public readonly int ItemCount { get; }

        public readonly bool IsEmpty => ItemCount == 0 || Item.IsNull();

        readonly bool IItemContainerInfoItemless.IsFull => false;

        readonly int IItemContainerInfoItemless.FreeSpace => int.MaxValue;

        public ReadOnlyItemContainer(TItem? item, int itemCount)
        {
            if (itemCount <= 0 || item.IsNull())
            {
                Item = default;
                ItemCount = 0;
                return;
            }

            Item = item;
            ItemCount = itemCount;
        }

        public static implicit operator ReadOnlyItemContainer(ReadOnlyItemContainer<TItem> instance)
        {
            return new ReadOnlyItemContainer(instance.Item, instance.ItemCount);
        }

        public static implicit operator LargeReadOnlyItemContainer<TItem>(ReadOnlyItemContainer<TItem> instance)
        {
            return instance.AsLarge();
        }

        public static bool operator ==(ReadOnlyItemContainer<TItem> left, ReadOnlyItemContainer<TItem> right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(ReadOnlyItemContainer<TItem> left, ReadOnlyItemContainer<TItem> right)
        {
            return !(left == right);
        }

        public readonly ReadOnlyItemContainer<TItem> PutItemTo(IItemContainer<TItem>? container)
        {
            if (container.IsNull())
                return this;

            return container.PutItem(container.Item, container.ItemCount);
        }

        public readonly ReadOnlyItemContainer<TItem> PutItemTo(IItemContainer<TItem>? container, int count)
        {
            if (container.IsNull())
                return this;

            return container.PutItem(container.Item, count);
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

        public readonly LargeReadOnlyItemContainer<TItem> AsLarge()
        {
            return new LargeReadOnlyItemContainer<TItem>(Item, ItemCount);
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

        readonly bool IItemContainerInfoItemless.CanPutItem() => false;
        readonly bool IItemContainerInfoItemless.CanPutItem(IItem? item) => false;
        readonly bool IItemContainerInfoItemless.CanPutItem(IItem? item, int count) => false;

        readonly Observable<TItem?> IItemContainerInfo<TItem>.ObserveItem()
        {
            return Observable.Return(Item);
        }

        readonly Observable<int> IItemContainerInfoItemless.ObserveItemCount()
        {
            return Observable.Return(ItemCount);
        }
    }
}
