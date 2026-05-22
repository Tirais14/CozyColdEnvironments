using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;

#nullable enable
namespace CCEnvs.UnityX.ECS.Items
{
    [BurstCompile]
    public struct InventoryUnmanaged : INativeDisposable, IDisposable
    {
        public int ID;

        public NativeArray<ItemContainerUnmanaged> ItemContainers;

        public bool IsEmpty => ContainsItem();

        public bool ContainsItem()
        {
            for (int i = 0; i < ItemContainers.Length; i++)
                if (ItemContainers[i].ItemCount > 0)
                    return true;

            return false;
        }
        public bool ContainsItem(ItemUnmanged item)
        {
            for (int i = 0; i < ItemContainers.Length; i++)
                if (ItemContainers[i].ContainsItem(item))
                    return true;

            return false;
        }
        public bool ContainsItem(ItemUnmanged item, int itemCount)
        {
            for (int i = 0; i < ItemContainers.Length; i++)
                if (ItemContainers[i].ContainsItem(item, itemCount))
                    return true;

            return false;
        }
        public bool ContainsItem(in ItemContainerUnmanaged itemContainer)
        {
            return ContainsItem(itemContainer.Item, itemContainer.ItemCount);
        }

        public JobHandle Dispose(JobHandle inputDeps) => ItemContainers.Dispose(inputDeps);
        public void Dispose() => ItemContainers.Dispose();
    }

    public static class InventoryUnmanagedExtensions
    {
        [BurstCompile]
        public static InventoryUnmanaged ToInventory<TItemContainerPool>(
            this TItemContainerPool itemContainerPool,
            NullableUnmanaged<int> inventoryID
            )
            where TItemContainerPool : unmanaged, IIndexable<ItemContainerUnmanaged>
        {
            var inventoryContainers = new NativeList<ItemContainerUnmanaged>(itemContainerPool.Length, Allocator.Persistent);

            for (int i = 0; i < itemContainerPool.Length; i++)
            {
                ref readonly ItemContainerUnmanaged itemContainer = ref itemContainerPool.ElementAt(i);

                if (inventoryID.HasValue
                    &&
                    inventoryID.NotEqualsUnmanaged(itemContainer.InventoryID))
                {
                    continue;
                }

                inventoryContainers.Add(itemContainer);
            }

            return new InventoryUnmanaged
            {
                ID = inventoryID.Value,
                ItemContainers = inventoryContainers.AsArray()
            };
        }

        [BurstCompile]
        public static InventoryUnmanaged ToInventory<TItemContainerPool>(
            this TItemContainerPool itemContainerPool,
            InventoryReferenceUnmanged inventoryRef
            )
            where TItemContainerPool : unmanaged, IIndexable<ItemContainerUnmanaged>
        {
            return itemContainerPool.ToInventory(inventoryRef.InventoryID);
        }
    }
}
