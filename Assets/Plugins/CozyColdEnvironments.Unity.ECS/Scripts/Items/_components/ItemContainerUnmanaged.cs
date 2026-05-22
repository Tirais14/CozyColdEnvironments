using System;
using Unity.Burst;
using Unity.Entities;

#nullable enable
namespace CCEnvs.UnityX.ECS.Items
{
    [InternalBufferCapacity(16)]
    public struct ItemContainerUnmanaged : IBufferElementData, IEquatable<ItemContainerUnmanaged>
    {
        public ItemUnmanged Item;

        public int ItemCount;

        public readonly bool IsEmpty {
            [BurstCompile]
            get => ItemCount > 0;
        }

        public static bool operator ==(ItemContainerUnmanaged left, ItemContainerUnmanaged right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(ItemContainerUnmanaged left, ItemContainerUnmanaged right)
        {
            return !(left == right);
        }

        [BurstCompile]
        public readonly bool ContainsItem() => !IsEmpty;
        [BurstCompile]
        public readonly bool ContainsItem(ItemUnmanged item)
        {
            return Item.Equals(item);
        }
        [BurstCompile]
        public readonly bool ContainsItem(ItemUnmanged item, int itemCount)
        {
            return ContainsItem(item) && ItemCount >= itemCount;
        }

        public readonly override bool Equals(object? obj)
        {
            return obj is ItemContainerUnmanaged unmanaged && Equals(unmanaged);
        }

        public readonly bool Equals(ItemContainerUnmanaged other)
        {
            return Item.Equals(other.Item)
                   &&
                   ItemCount == other.ItemCount;
        }

        public readonly override int GetHashCode()
        {
            return HashCode.Combine(Item, ItemCount);
        }
    }
}
