using CommunityToolkit.Diagnostics;
using System;
using System.Collections.Generic;

#nullable enable
namespace CCEnvs.UnityX.Items
{
    public readonly struct ItemAccessorPutItemEvent : IEquatable<ItemAccessorPutItemEvent>
    {
        public IItem Item { get; }

        public int ItemCount { get; }

        public ItemAccessorPutItemEvent(IItem item, int itemCount)
        {
            CC.Guard.IsNotNull(item, nameof(item));
            Guard.IsGreaterThan(itemCount, 0);

            Item = item;
            ItemCount = itemCount;
        }

        public static bool operator ==(ItemAccessorPutItemEvent left, ItemAccessorPutItemEvent right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(ItemAccessorPutItemEvent left, ItemAccessorPutItemEvent right)
        {
            return !(left == right);
        }

        public ItemAccessorPutItemEvent<TItem> Convert<TItem>()
            where TItem : IItem
        {
            return new ItemAccessorPutItemEvent<TItem>(Item.CastTo<TItem>(), ItemCount);
        }

        public override bool Equals(object? obj)
        {
            return obj is ItemAccessorPutItemEvent @event && Equals(@event);
        }

        public bool Equals(ItemAccessorPutItemEvent other)
        {
            return EqualityComparer<IItem>.Default.Equals(Item, other.Item) &&
                   ItemCount == other.ItemCount;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Item, ItemCount);
        }
    }

    public readonly struct ItemAccessorPutItemEvent<TItem> : IEquatable<ItemAccessorPutItemEvent<TItem>> where TItem : IItem
    {
        public TItem Item { get; }

        public int ItemCount { get; }

        public ItemAccessorPutItemEvent(TItem item, int itemCount)
        {
            CC.Guard.IsNotNull(item, nameof(item));
            Guard.IsGreaterThan(itemCount, 0);

            Item = item;
            ItemCount = itemCount;
        }

        public static implicit operator ItemAccessorPutItemEvent(ItemAccessorPutItemEvent<TItem> instance)
        {
            return instance.AsUntyped();
        }

        public static bool operator ==(ItemAccessorPutItemEvent<TItem> left, ItemAccessorPutItemEvent<TItem> right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(ItemAccessorPutItemEvent<TItem> left, ItemAccessorPutItemEvent<TItem> right)
        {
            return !(left == right);
        }

        public ItemAccessorPutItemEvent AsUntyped()
        {
            return new ItemAccessorPutItemEvent(Item, ItemCount);
        }

        public ItemAccessorPutItemEvent<TOutItem> Convert<TOutItem>()
            where TOutItem : TItem
        {
            return new ItemAccessorPutItemEvent<TOutItem>(Item.CastTo<TOutItem>(), ItemCount);
        }

        public override bool Equals(object? obj)
        {
            return obj is ItemAccessorPutItemEvent<TItem> @event && Equals(@event);
        }

        public bool Equals(ItemAccessorPutItemEvent<TItem> other)
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
