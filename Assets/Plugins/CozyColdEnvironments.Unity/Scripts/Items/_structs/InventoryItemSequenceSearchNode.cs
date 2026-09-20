using System;
using System.Collections.Generic;

#nullable enable
namespace CCEnvs.UnityX.Items
{
    public readonly struct InventoryItemSequenceSearchNode : IEquatable<InventoryItemSequenceSearchNode>
    {
        public IItem? Item { get; }

        public int ItemCount { get; }

        public ItemCountCheckType ItemCountCheckType { get; }

        public InventoryItemSequenceSearchNode(
            IItem? item,
            int itemCount = 1,
            ItemCountCheckType itemCountCheckType = ItemCountCheckType.Default
            )
        {
            Item = item;
            ItemCount = Math.Max(itemCount, 1);
            ItemCountCheckType = itemCountCheckType;
        }

        public static InventoryItemSequenceSearchNode Create(
            IItem? item = null,
            int itemCount = 1,
            ItemCountCheckType itemCountCheckType = ItemCountCheckType.Default
            )
        {
            return new InventoryItemSequenceSearchNode(item, itemCount, itemCountCheckType);
        }

        public static bool operator ==(InventoryItemSequenceSearchNode left, InventoryItemSequenceSearchNode right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(InventoryItemSequenceSearchNode left, InventoryItemSequenceSearchNode right)
        {
            return !(left == right);
        }

        public override bool Equals(object? obj)
        {
            return obj is InventoryItemSequenceSearchNode node && Equals(node);
        }

        public bool Equals(InventoryItemSequenceSearchNode other)
        {
            return EqualityComparer<IItem?>.Default.Equals(Item, other.Item) &&
                   ItemCount == other.ItemCount &&
                   ItemCountCheckType == other.ItemCountCheckType;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Item, ItemCount, ItemCountCheckType);
        }
    }
}
