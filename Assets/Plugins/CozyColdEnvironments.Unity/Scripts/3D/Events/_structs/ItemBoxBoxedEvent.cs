#nullable enable
using System;
using System.Collections.Generic;

namespace CCEnvs.Unity.D3.Events
{
    public readonly struct ItemBoxBoxedEvent : IEquatable<ItemBoxBoxedEvent>
    {
        public readonly ItemBox ItemBox;

        public readonly IBoxItem Item;

        public ItemBoxBoxedEvent(ItemBox itemBox, IBoxItem item)
        {
            CC.Guard.IsNotNull(itemBox, nameof(itemBox));
            CC.Guard.IsNotNull(item, nameof(item));

            ItemBox = itemBox;
            Item = item;
        }

        public static bool operator ==(ItemBoxBoxedEvent left, ItemBoxBoxedEvent right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(ItemBoxBoxedEvent left, ItemBoxBoxedEvent right)
        {
            return !(left == right);
        }

        public readonly override bool Equals(object? obj)
        {
            return obj is ItemBoxBoxedEvent @event && Equals(@event);
        }

        public readonly bool Equals(ItemBoxBoxedEvent other)
        {
            return EqualityComparer<ItemBox>.Default.Equals(ItemBox, other.ItemBox) &&
                   EqualityComparer<IBoxItem>.Default.Equals(Item, other.Item);
        }

        public readonly override int GetHashCode()
        {
            return HashCode.Combine(ItemBox, Item);
        }
    }
}
