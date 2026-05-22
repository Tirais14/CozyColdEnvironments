using CCEnvs.UnityX.Items;
using System;
using Unity.Entities;

#nullable enable
namespace CCEnvs.UnityX.ECS.Items
{
    [InternalBufferCapacity(16)]
    public struct ItemUnmanged
        :
        IBufferElementData,
        IEquatable<ItemUnmanged>,
        IManagedConvertible<IItem>
    {
        public int ID;

        public static bool operator ==(ItemUnmanged left, ItemUnmanged right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(ItemUnmanged left, ItemUnmanged right)
        {
            return !(left == right);
        }

        public static implicit operator ItemUnmanged(int id)
        {
            return new ItemUnmanged { ID = id };
        }

        public static explicit operator int(ItemUnmanged item)
        {
            return item.ID;
        }

        public ItemUnmanged Create(IItem item)
        {
            CC.Guard.IsNotNull(item, nameof(item));
            return new ItemUnmanged { ID = item.ID };
        }

        public readonly IItem ConvertToManaged() => ItemRegistry.Get(ID);

        public readonly T ConvertToManagedT<T>() where T : IItem => (T)ConvertToManaged();

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

        public readonly override string ToString()
        {
            return ToStringBuilder.CreatePooled()
                .AddProperty(nameof(ID), ID)
                .ToStringAndDispose();
        }
    }
}
