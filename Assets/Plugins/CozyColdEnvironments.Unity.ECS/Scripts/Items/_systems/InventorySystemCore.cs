using CCEnvs.Threading;
using CCEnvs.UnityX.Items;
using R3;
using System;
using System.Collections.Generic;
using System.Threading;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

#nullable enable
namespace CCEnvs.UnityX.ECS.Items
{
    public static class InventorySystemCore
    {
        public static bool IsAnyInventoryChanged => (changedInventories?.Count ?? 0) > 0;

        public static IReadOnlyDictionary<int, IInventory> ChangedInventories => changedInventories ?? throw new InvalidOperationException($"{typeof(InventorySystemCore)} is not created");

        private static Dictionary<int, IInventory>? changedInventories;

        private static CancellationTokenSource? destroyCancellationTokenSource;
        private static IDisposable? anySub;

        public static void Create()
        {
            Destroy();

            destroyCancellationTokenSource = new CancellationTokenSource();
            changedInventories = new Dictionary<int, IInventory>(InventoryRegistry.Inventories.Count);

            anySub = InventoryRegistry.ObserveAny(destroyCancellationTokenSource.Token)
                .Subscribe(static (inventory) =>
                {
                    changedInventories.Add(inventory.Key, inventory.Value);
                });
        }

        public static void Destroy()
        {
            if (anySub.IsNotNull())
            {
                anySub.Dispose();
                anySub = null;
            }

            if (destroyCancellationTokenSource is null)
                return;

            destroyCancellationTokenSource.CancelAndDispose();
            destroyCancellationTokenSource = null;

            changedInventories = null;
        }

        public static void ResetChangedInventories()
        {
            changedInventories?.Clear();
        }

        public static void UpdateInventoryContainersContentIncrementally(
            in DynamicBuffer<InventoryReference> inventoryRefs,
            in DynamicBuffer<ItemContainerUnmanaged> itemContainerPool,
            bool compactItemContainers = true
            )
        {
            for (int i = 0; i < inventoryRefs.Length; i++)
            {
                InventoryReference inventoryRef = inventoryRefs[i];

                if (!ChangedInventories.TryGetValue(inventoryRef.InventoryID, out var inventory))
                    continue;

                RemoveInventoryContainers(inventoryRef, itemContainerPool);

                AddInventoryContainers(
                    inventory,
                    inventoryRef,
                    itemContainerPool,
                    compactItemContainers
                    );
            }
        }

        public static void UpdateInventoryContainersContent(
            in DynamicBuffer<InventoryReference> inventoryRefs,
            in DynamicBuffer<ItemContainerUnmanaged> itemContainerPool
            )
        {
            if (inventoryRefs.Length == 0)
            {
                itemContainerPool.Clear();
                return;
            }

            bool hasChangedInventory = ChangedInventories.ContainsKey(inventoryRefs[^1].InventoryID);

            if (!hasChangedInventory)
            {
                if (inventoryRefs.Length == 1)
                    return;

                for (int i = 0; i < inventoryRefs.Length; i++)
                {
                    if (!ChangedInventories.ContainsKey(inventoryRefs[i]))
                        continue;

                    hasChangedInventory = true;
                    break;
                }
            }

            if (!hasChangedInventory)
                return;

            itemContainerPool.Clear();

            for (int i = 0; i < inventoryRefs.Length; i++)
            {
                ref readonly InventoryReference inventoryRef = ref inventoryRefs.ElementAt(i);

                if (!InventoryRegistry.TryGet(inventoryRef, out IInventory? inventory))
                    continue;

                foreach (var itemContainer in inventory)
                    itemContainerPool.Add(itemContainer.ToUnmanaged(inventoryRef));
            }
        }

        public static void UpdateInventoriesContent(
            in DynamicBuffer<InventoryUnmanaged> inventories
            )
        {
            for (int i = 0; i < inventories.Length; i++)
            {
                ref InventoryUnmanaged inventoryUnmanaged = ref inventories.ElementAt(i);

                if (!inventoryUnmanaged.ID.HasValue
                    ||
                    !ChangedInventories.TryGetValue(inventoryUnmanaged.ID.Value, out IInventory? inventory))
                {
                    continue;
                }

                inventoryUnmanaged.ItemContainers.Dispose();
                inventoryUnmanaged.ItemContainers = default;

                if (inventory.ItemCount <= 0)
                    continue;

                using var newItemContainers = new NativeList<ItemContainerUnmanaged>(inventory.ContainerCount, Allocator.Persistent);

                foreach (var itemContainer in inventory)
                {
                    if (itemContainer.IsEmpty)
                        continue;

                    newItemContainers.Add(itemContainer.ToUnmanaged());
                }

                inventoryUnmanaged.ItemContainers = newItemContainers.AsArray();
            }
        }

        [BurstCompile]
        private static bool TryAddItemToItemContainers(
            NativeList<ItemContainerUnmanaged> itemContainers,
            ItemReference itemRef,
            int itemCount,
            out int restItemCount
            )
        {
            restItemCount = 0;

            for (int i = 0; i < itemContainers.Length; i++)
            {
                ref ItemContainerUnmanaged itemContainerUnmanaged = ref itemContainers.ElementAt(i);

                if (itemContainerUnmanaged.Item != itemRef)
                    continue;

                long itemCountSum = (long)itemContainerUnmanaged.ItemCount + itemCount;

                if (itemCountSum > int.MaxValue)
                    restItemCount = (int)(itemCountSum - int.MaxValue);

                itemContainerUnmanaged.ItemCount += itemCount;
                return true;
            }

            return false;
        }

        private static NativeArray<ItemContainerUnmanaged> GetItemContainersCompacted(
            IInventory inventory,
            int inventoryID,
            AllocatorManager.AllocatorHandle allocator
            )
        {
            var unmanagedItemContainers = new NativeList<ItemContainerUnmanaged>(inventory.ContainerCount, allocator);

            foreach (var itemContainer in inventory)
            {
                if (!itemContainer.Item.TryGetValue(out IItem? item))
                    continue;

                ItemReference itemRef = item.GetUnmanagedReference();

                if (TryAddItemToItemContainers(
                    unmanagedItemContainers,
                    itemRef,
                    itemContainer.ItemCount,
                    out int restItemCount
                    ))
                {
                    if (restItemCount >= 1)
                    {
                        var restItems = new ItemContainerUnmanaged
                        {
                            InventoryRef = new InventoryReference { InventoryID = inventoryID },
                            Item = itemRef,
                            ItemCount = restItemCount
                        };

                        unmanagedItemContainers.Add(restItems);
                    }

                    continue;
                }

                unmanagedItemContainers.Add(itemContainer.ToUnmanaged(inventoryID));
            }

            return unmanagedItemContainers.AsArray();
        }

        private static void AddInventoryContainers(
            IInventory inventory,
            InventoryReference inventoryRef,
            in DynamicBuffer<ItemContainerUnmanaged> itemContainerPool,
            bool compactItemContainers
            )
        {
            if (inventory.ItemCount <= 0)
                return;

            if (compactItemContainers)
            {
                NativeArray<ItemContainerUnmanaged> itemContainers = GetItemContainersCompacted(
                    inventory,
                    inventoryRef,
                    Allocator.Temp
                    );

                for (int i = 0; i < itemContainers.Length; i++)
                    itemContainerPool.Add(itemContainers[i]);

                itemContainers.Dispose();
            }
            else
            {
                foreach (var itemContainer in inventory)
                {
                    ItemContainerUnmanaged itemContainerUnmanaged = itemContainer.ToUnmanaged(inventoryRef.InventoryID);
                    itemContainerPool.Add(itemContainerUnmanaged);
                }
            }
        }

        private static void RemoveInventoryContainers(
            InventoryReference inventoryRef,
            in DynamicBuffer<ItemContainerUnmanaged> itemContainerPool
            )
        {
            using var inventoryContainerIndexes = itemContainerPool.GetInventoryContainerIndexes(inventoryRef, Allocator.Temp);

            for (int i = inventoryContainerIndexes.Length - 1; i >= 0; i--)
            {
                int inventoryContainerIdx = inventoryContainerIndexes[i];
                itemContainerPool.RemoveAtSwapBack(inventoryContainerIdx);
            }
        }
    }
}
