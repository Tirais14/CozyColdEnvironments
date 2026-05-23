using CCEnvs.UnityX.Items;
using System.Diagnostics.CodeAnalysis;
using Unity.Entities;

#nullable enable
namespace CCEnvs.UnityX.ECS.Items
{
    [InternalBufferCapacity(8)]
    public struct InventoryReferenceUnmanged : IBufferElementData
    {
        public int InventoryID;

        public static implicit operator int(InventoryReferenceUnmanged instance)
        {
            return instance.InventoryID;
        }

        public static implicit operator InventoryReferenceUnmanged(int inventoryID)
        {
            return new InventoryReferenceUnmanged { InventoryID = inventoryID };
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
    }
}
