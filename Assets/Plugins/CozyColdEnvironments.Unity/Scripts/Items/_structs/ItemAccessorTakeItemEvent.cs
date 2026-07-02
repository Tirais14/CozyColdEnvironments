using CommunityToolkit.Diagnostics;
using System;
using System.Collections.Generic;

namespace CCEnvs.UnityX.Items
{
    public readonly struct ItemAccessorTakeItemEvent : IEquatable<ItemAccessorTakeItemEvent>
    {
        public IItem Item { get; }

        public int ItemCount { get; }

        public ItemAccessorTakeItemEvent(IItem item, int itemCount)
        {
            CC.Guard.IsNotNull(item, nameof(item));
            Guard.IsGreaterThan(itemCount, 0);

            Item = item;
            ItemCount = itemCount;
        }

        public static bool operator ==(ItemAccessorTakeItemEvent left, ItemAccessorTakeItemEvent right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(ItemAccessorTakeItemEvent left, ItemAccessorTakeItemEvent right)
        {
            return !(left == right);
        }

        public ItemAccessorTakeItemEvent<TItem> Convert<TItem>()
            where TItem : IItem
        {
            return new ItemAccessorTakeItemEvent<TItem>(Item.CastTo<TItem>(), ItemCount);
        }

        public override bool Equals(object obj)
        {
            return obj is ItemAccessorTakeItemEvent @event && Equals(@event);
        }

        public bool Equals(ItemAccessorTakeItemEvent other)
        {
            return EqualityComparer<IItem>.Default.Equals(Item, other.Item) &&
                   ItemCount == other.ItemCount;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Item, ItemCount);
        }
    }

    public readonly struct ItemAccessorTakeItemEvent<TItem> : IEquatable<ItemAccessorTakeItemEvent<TItem>> where TItem : IItem
    {
        public TItem Item { get; }

        public int ItemCount { get; }

        public ItemAccessorTakeItemEvent(TItem item, int itemCount)
        {
            CC.Guard.IsNotNull(item, nameof(item));
            Guard.IsGreaterThan(itemCount, 0);

            Item = item;
            ItemCount = itemCount;
        }

        public static implicit operator ItemAccessorTakeItemEvent(ItemAccessorTakeItemEvent<TItem> instance)
        {
            return instance.AsUntyped();
        }

        public static bool operator ==(ItemAccessorTakeItemEvent<TItem> left, ItemAccessorTakeItemEvent<TItem> right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(ItemAccessorTakeItemEvent<TItem> left, ItemAccessorTakeItemEvent<TItem> right)
        {
            return !(left == right);
        }

        public ItemAccessorTakeItemEvent AsUntyped()
        {
            return new ItemAccessorTakeItemEvent(Item, ItemCount);
        }

        public ItemAccessorTakeItemEvent<TOutItem> Convert<TOutItem>()
            where TOutItem : TItem
        {
            return new ItemAccessorTakeItemEvent<TOutItem>(Item.CastTo<TOutItem>(), ItemCount);
        }

        public override bool Equals(object obj)
        {
            return obj is ItemAccessorTakeItemEvent<TItem> @event && Equals(@event);
        }

        public bool Equals(ItemAccessorTakeItemEvent<TItem> other)
        {
            return EqualityComparer<TItem>.Default.Equals(Item, other.Item) &&
                   ItemCount == other.ItemCount;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Item, ItemCount);
        }
    }
}
