using CCEnvs.UnityX.Items;
using System;
using Unity.Entities;

#nullable enable
namespace CCEnvs.UnityX.ECS.Items
{
    [InternalBufferCapacity(16)]
    public struct ItemUnmanged : IBufferElementData, IEquatable<ItemUnmanged>
    {
        public int ID;

        public ItemUnmanged Create(IItem item)
        {
            CC.Guard.IsNotNull(item, nameof(item));
            return new ItemUnmanged { ID = item.ID };
        }

        public readonly bool Equals(ItemUnmanged other)
        {
            return ID == other.ID;
        }

        public readonly override bool Equals(object obj)
        {
            return obj is ItemUnmanged typed && Equals(typed);
        }

        public readonly override int GetHashCode()
        {
            return HashCode.Combine(ID);
        }
    }
}
