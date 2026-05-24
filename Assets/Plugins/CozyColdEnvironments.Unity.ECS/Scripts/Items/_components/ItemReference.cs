using CCEnvs.UnityX.Items;
using System;
using System.Runtime.CompilerServices;
using Unity.Entities;

#nullable enable
namespace CCEnvs.UnityX.ECS.Items
{
    [InternalBufferCapacity(16)]
    public struct ItemReference
        :
        IBufferElementData,
        IEquatable<ItemReference>,
        IManagedConvertible<IItem>
    {
        public int ID;

        public static bool operator ==(ItemReference left, ItemReference right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(ItemReference left, ItemReference right)
        {
            return !(left == right);
        }

        public static implicit operator ItemReference(int id)
        {
            return new ItemReference { ID = id };
        }

        public static explicit operator int(ItemReference item)
        {
            return item.ID;
        }

        public ItemReference Create(IItem item)
        {
            CC.Guard.IsNotNull(item, nameof(item));
            return new ItemReference { ID = item.ID };
        }

        public readonly IItem ToManaged() => ItemRegistry.Get(ID);

        public readonly T ToManagedT<T>() where T : IItem => (T)ToManaged();

        public readonly bool Equals(ItemReference other)
        {
            return ID == other.ID;
        }

        public readonly override bool Equals(object obj)
        {
            return obj is ItemReference typed && Equals(typed);
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

    public static class ItemReferenceExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ItemReference GetUnmanagedReference(this IItem source)
        {
            CC.Guard.IsNotNullSource(source);
            return source.ID;
        }
    }
}
