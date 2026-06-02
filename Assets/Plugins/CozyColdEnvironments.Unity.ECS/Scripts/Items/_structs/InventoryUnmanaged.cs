using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;

#nullable enable
namespace CCEnvs.UnityX.ECS.Items
{
    [InternalBufferCapacity(8)]
    public struct InventoryUnmanaged
        : 
        IBufferElementData,
        INativeDisposable,
        IDisposable, 
        IEnumerable<ItemContainerUnmanaged>
    {
        public MaybeUnmanaged<int> ID;

        public NativeArray<ItemContainerUnmanaged> ItemContainers;

        public bool IsEmpty {
            [BurstCompile]
            get => ContainsItem();
        }

        [BurstCompile]
        public bool ContainsItem()
        {
            for (int i = 0; i < ItemContainers.Length; i++)
                if (ItemContainers[i].ItemCount > 0)
                    return true;

            return false;
        }
        [BurstCompile]
        public bool ContainsItem(ItemReference item)
        {
            for (int i = 0; i < ItemContainers.Length; i++)
                if (ItemContainers[i].ContainsItem(item))
                    return true;

            return false;
        }
        [BurstCompile]
        public bool ContainsItem(ItemReference item, int itemCount)
        {
            if (itemCount <= 0)
                return false;

            int foundItemCount = 0;

            for (int i = 0; i < ItemContainers.Length; i++)
            {
                ItemContainerUnmanaged itemContainer = ItemContainers[i];

                if (!itemContainer.ContainsItem(item))
                    continue;

                foundItemCount += itemContainer.ItemCount;

                if (foundItemCount >= itemCount)
                    return true;
            }

            return false;
        }
        [BurstCompile]
        public bool ContainsItem(in ItemContainerUnmanaged itemContainer)
        {
            return ContainsItem(itemContainer.Item, itemContainer.ItemCount);
        }

        [BurstCompile]
        public bool ContainsItems(NativeArray<ItemContainerUnmanaged> itemInfos)
        {
            using var foundItemInfoFlags = new NativeBitArray(itemInfos.Length, Allocator.Temp);
            foundItemInfoFlags.SetBits(0, false, foundItemInfoFlags.Length);

            for (int i = 0; i < itemInfos.Length; i++)
            {
                if (!ContainsItem(itemInfos[i]))
                    continue;

                foundItemInfoFlags.Set(i, true);
            }

            return foundItemInfoFlags.TestAll(0, itemInfos.Length);
        }

        public JobHandle Dispose(JobHandle inputDeps) => ItemContainers.Dispose(inputDeps);
        public void Dispose() => ItemContainers.Dispose();

        public IEnumerator<ItemContainerUnmanaged> GetEnumerator() => ItemContainers.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

    public static class InventoryUnmanagedExtensions
    {
        [BurstCompile]
        public static InventoryUnmanaged AsInventory<TItemContainerPool>(
            this TItemContainerPool itemContainerPool,
            MaybeUnmanaged<int> inventoryID = default
            )
            where TItemContainerPool : unmanaged, IIndexable<ItemContainerUnmanaged>
        {
            var inventoryContainers = new NativeList<ItemContainerUnmanaged>(itemContainerPool.Length, Allocator.Persistent);

            for (int i = 0; i < itemContainerPool.Length; i++)
            {
                ref readonly ItemContainerUnmanaged itemContainer = ref itemContainerPool.ElementAt(i);

                if (inventoryID.HasValue
                    &&
                    inventoryID.NotEqualsUnmanaged(itemContainer.InventoryRef.Reinterpret<int>()))
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
        public static bool TryGetInventory(
            this in NativeArray<InventoryUnmanaged> inventories,
            InventoryReference inventoryRef,
            out InventoryUnmanaged result
            )
        {
            for (int i = 0; i < inventories.Length; i++)
            {
                InventoryUnmanaged inventory = inventories[i];

                if (!inventory.ID.HasValue && inventory.ID.Value == inventoryRef.InventoryID)
                    continue;

                result = inventory;
                return true;
            }

            result = default;
            return false;
        }
    }
}
