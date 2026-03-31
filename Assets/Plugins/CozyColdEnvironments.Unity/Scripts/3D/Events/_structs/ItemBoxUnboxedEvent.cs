using System;
using System.Collections.Generic;

#nullable enable
namespace CCEnvs.Unity.D3.Events
{
    public readonly struct ItemBoxUnboxedEvent : IEquatable<ItemBoxUnboxedEvent>
    {
        public readonly ItemBox ItemBox;

        public readonly IBoxItem Item;

        public ItemBoxUnboxedEvent(ItemBox itemBox, IBoxItem item)
        {
            CC.Guard.IsNotNull(itemBox, nameof(itemBox));
            CC.Guard.IsNotNull(item, nameof(item));

            ItemBox = itemBox;
            Item = item;
        }

        public static bool operator ==(ItemBoxUnboxedEvent left, ItemBoxUnboxedEvent right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(ItemBoxUnboxedEvent left, ItemBoxUnboxedEvent right)
        {
            return !(left == right);
        }

        public readonly override bool Equals(object? obj)
        {
            return obj is ItemBoxUnboxedEvent @event && Equals(@event);
        }

        public readonly bool Equals(ItemBoxUnboxedEvent other)
        {
            return EqualityComparer<ItemBox>.Default.Equals(ItemBox, other.ItemBox)
                   &&
                   EqualityComparer<IBoxItem>.Default.Equals(Item, other.Item);
        }

        public readonly override int GetHashCode()
        {
            return HashCode.Combine(ItemBox, Item);
        }
    }
}
