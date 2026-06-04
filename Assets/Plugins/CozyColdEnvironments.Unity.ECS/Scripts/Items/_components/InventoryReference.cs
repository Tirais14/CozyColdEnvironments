using CCEnvs.UnityX.Items;
using System;
using System.Diagnostics.CodeAnalysis;
using Unity.Burst;
using Unity.Entities;

#nullable enable
namespace CCEnvs.UnityX.ECS.Items
{
    [InternalBufferCapacity(8)]
    public struct InventoryReference : IBufferElementData, IEquatable<InventoryReference>
    {
        public long InventoryID;

        [BurstCompile]
        public static implicit operator long(InventoryReference instance)
        {
            return instance.InventoryID;
        }
        [BurstCompile]

        public static implicit operator InventoryReference(long inventoryID)
        {
            return new InventoryReference { InventoryID = inventoryID };
        }

        [BurstCompile]
        public static bool operator ==(InventoryReference left, InventoryReference right)
        {
            return left.Equals(right);
        }

        [BurstCompile]
        public static bool operator !=(InventoryReference left, InventoryReference right)
        {
            return !(left == right);
        }

        public readonly IInventory Materialize() => InventoryRegistry.Get(InventoryID);
        public readonly IInventory Materialize<T>() where T : IInventory => InventoryRegistry.Get<T>(InventoryID);

        public readonly bool TryMaterialize([NotNullWhen(true)] out IInventory? inventory)
        {
            return InventoryRegistry.TryGet(InventoryID, out inventory);
        }
        public readonly bool TryMaterialize<T>([NotNullWhen(true)] out T? inventory)
            where T : IInventory
        {
            return InventoryRegistry.TryGet(InventoryID, out inventory);
        }

        public readonly override bool Equals(object? obj)
        {
            return obj is InventoryReference reference && Equals(reference);
        }

        [BurstCompile]
        public readonly bool Equals(InventoryReference other)
        {
            return InventoryID == other.InventoryID;
        }

        [BurstCompile]
        public readonly override int GetHashCode()
        {
            return HashCode.Combine(InventoryID);
        }
    }
}
