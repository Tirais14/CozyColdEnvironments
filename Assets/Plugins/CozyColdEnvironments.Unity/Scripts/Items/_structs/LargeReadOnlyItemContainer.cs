#nullable enable
using R3;
using System;
using System.Collections.Generic;

namespace CCEnvs.UnityX.Items
{
    public readonly struct LargeReadOnlyItemContainer
        :
        IItemContainerInfo,
        IEquatable<LargeReadOnlyItemContainer>
    {
        public static LargeReadOnlyItemContainer Empty { get; } = new();
        public IItem? Item { get; }

        public long ItemCount { get; }

        public bool IsEmpty => !ContainsItem();
        public bool IsFull => ItemCount == long.MaxValue;

        int IItemContainerInfoItemless.ItemCount => ItemCount.ToInt();
        int IItemContainerInfoItemless.FreeSpace => 0;

        public LargeReadOnlyItemContainer(IItem? item, long itemCount)
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

        public static explicit operator ReadOnlyItemContainer(LargeReadOnlyItemContainer instance)
        {
            return instance.ToNormal();
        }

        public static bool operator ==(LargeReadOnlyItemContainer left, LargeReadOnlyItemContainer right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(LargeReadOnlyItemContainer left, LargeReadOnlyItemContainer right)
        {
            return !(left == right);
        }

        public readonly ReadOnlyItemContainer ToNormal()
        {
            return new ReadOnlyItemContainer(Item, (int)Math.Min(ItemCount, int.MaxValue));
        }

        public readonly LargeReadOnlyItemContainer<TOutItem> Convert<TOutItem>()
            where TOutItem : IItem
        {
            return new LargeReadOnlyItemContainer<TOutItem>(Item.As<TOutItem>(), ItemCount);
        }

        public readonly LargeReadOnlyItemContainer PutItemTo(IItemContainer? container)
        {
            if (container.IsNull())
                return this;

            return container.PutItem(Item, ItemCount);
        }
        public readonly LargeReadOnlyItemContainer PutItemTo(IItemContainer? container, int count)
        {
            if (container.IsNull())
                return this;

            return container.PutItem(Item, count);
        }

        public readonly bool ContainsItem()
        {
            return ItemCount >= 1 && Item.IsNotNull();
        }
        public readonly bool ContainsItem(IItem? item)
        {
            return ContainsItem() && EqualityComparer<IItem?>.Default.Equals(Item, item);
        }
        public readonly bool ContainsItem(IItem? item, int count)
        {
            return ItemCount >= count && ContainsItem(item);
        }

        public override bool Equals(object? obj)
        {
            return obj is LargeReadOnlyItemContainer container && Equals(container);
        }

        public bool Equals(LargeReadOnlyItemContainer other)
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

        bool IItemContainerInfoItemless.CanPutItem() => false;
        bool IItemContainerInfoItemless.CanPutItem(IItem? item) => false;
        bool IItemContainerInfoItemless.CanPutItem(IItem? item, int count) => false;

        Observable<IItem?> IItemContainerInfo.ObserveItem()
        {
            return Observable.Return(Item);
        }

        Observable<int> IItemContainerInfoItemless.ObserveItemCount()
        {
            return Observable.Return(((IItemContainerInfo)this).ItemCount);
        }
    }

    public readonly struct LargeReadOnlyItemContainer<TItem>
        :
        IItemContainerInfo<TItem>, IEquatable<LargeReadOnlyItemContainer<TItem>> where TItem : IItem
    {
        public static LargeReadOnlyItemContainer<TItem> Empty { get; } = new();

        public TItem? Item { get; }

        public long ItemCount { get; }

        public bool IsEmpty => !ContainsItem();
        public bool IsFull => ItemCount == long.MaxValue;

        int IItemContainerInfoItemless.ItemCount => (int)Math.Min(ItemCount, int.MaxValue);

        int IItemContainerInfoItemless.FreeSpace => 0;

        public LargeReadOnlyItemContainer(TItem? item, long itemCount)
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

        public static implicit operator LargeReadOnlyItemContainer(LargeReadOnlyItemContainer<TItem> instance)
        {
            return instance.AsUntyped();
        }

        public static explicit operator ReadOnlyItemContainer(LargeReadOnlyItemContainer<TItem> instance)
        {
            return instance.ToNormal();
        }

        public static bool operator ==(LargeReadOnlyItemContainer<TItem> left, LargeReadOnlyItemContainer<TItem> right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(LargeReadOnlyItemContainer<TItem> left, LargeReadOnlyItemContainer<TItem> right)
        {
            return !(left == right);
        }

        public ReadOnlyItemContainer<TItem> ToNormal()
        {
            return new ReadOnlyItemContainer<TItem>(Item, ItemCount.ToInt());
        }

        public LargeReadOnlyItemContainer AsUntyped()
        {
            return new LargeReadOnlyItemContainer(Item, ItemCount);
        }

        public LargeReadOnlyItemContainer<TOutItem> Convert<TOutItem>()
            where TOutItem : IItem
        {
            return new LargeReadOnlyItemContainer<TOutItem>(Item.As<TOutItem>(), ItemCount);
        }

        public readonly LargeReadOnlyItemContainer<TItem> PutItemTo(IItemContainer<TItem>? container)
        {
            if (container.IsNull())
                return this;

            return container.PutItem(Item, ItemCount);
        }
        public readonly LargeReadOnlyItemContainer<TItem> PutItemTo(IItemContainer<TItem>? container, int count)
        {
            if (container.IsNull())
                return this;

            return container.PutItem(Item, count);
        }

        public readonly bool ContainsItem()
        {
            return ItemCount >= 1 && Item.IsNotNull();
        }
        public readonly bool ContainsItem(IItem? item)
        {
            return ContainsItem() && EqualityComparer<IItem?>.Default.Equals((IItem?)Item, item);
        }
        public readonly bool ContainsItem(IItem? item, int count)
        {
            return ItemCount >= count && ContainsItem(item);
        }

        public override bool Equals(object? obj)
        {
            return obj is LargeReadOnlyItemContainer<TItem> container && Equals(container);
        }

        public bool Equals(LargeReadOnlyItemContainer<TItem> other)
        {
            return EqualityComparer<TItem?>.Default.Equals(Item, other.Item) &&
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

        bool IItemContainerInfoItemless.CanPutItem() => false;
        bool IItemContainerInfoItemless.CanPutItem(IItem? item) => false;
        bool IItemContainerInfoItemless.CanPutItem(IItem? item, int count) => false;

        Observable<TItem?> IItemContainerInfo<TItem>.ObserveItem()
        {
            return Observable.Return(Item);
        }

        Observable<int> IItemContainerInfoItemless.ObserveItemCount()
        {
            return Observable.Return(((IItemContainerInfo)this).ItemCount);
        }
    }
}
