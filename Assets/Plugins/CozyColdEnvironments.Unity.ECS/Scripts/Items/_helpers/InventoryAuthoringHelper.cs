using CCEnvs.UnityX.Items;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;

#nullable enable
namespace CCEnvs.UnityX.ECS.Items
{
    public static class InventoryAuthoringHelper
    {
        public static void AddBufferBasedInventory<TBaker>(
            this TBaker source,
            Entity entity,
            params KeyValuePair<int, IInventory>[] inventoryIDPairs
            )

            where TBaker : IBaker
        {
            var itemContainerPool = source.AddBuffer<ItemContainerUnmanaged>(entity);
            var inventoryRefs = source.AddBuffer<InventoryReference>(entity);

            for (int i = 0; i < inventoryIDPairs.Length; i++)
            {
                KeyValuePair<int, IInventory> inventoryIDPair = inventoryIDPairs[i];

                //TODO: Bind registration handle to entity lifetime by cleanup component
                InventoryRegistry.TryRegister(inventoryIDPair.Key, inventoryIDPair.Value, out _);

                inventoryRefs.Add(inventoryIDPair.Key);

                var inventoryContainers = inventoryIDPair.Value.GetUnmanagedItemContainers(inventoryIDPair.Key, Allocator.Temp);
                itemContainerPool.AddRange(inventoryContainers);
                inventoryContainers.Dispose();
            }
        }
    }
}
