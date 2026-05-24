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

        [BurstDiscard]
        public static void AddInventoryContainersIfAnyChanged(
            ref NativeHashMap<InventoryReference, NativeArray<ItemContainerUnmanaged>> results,
            in DynamicBuffer<InventoryReference> inventoryReferences,
            AllocatorManager.AllocatorHandle allocator
            )
        {
            if (!results.IsCreated)
                return;

            bool hasChangedInventories = false;

            for (int i = 0; i < inventoryReferences.Length; i++)
            {
                if (!InventoryRegistry.Contains(inventoryReferences[i]))
                    continue;

                hasChangedInventories = true;
                break;
            }

            if (!hasChangedInventories)
                return;

            for (int i = 0; i < inventoryReferences.Length; i++)
            {
                InventoryReference inventoryRef = inventoryReferences[i];

                if (!inventoryRef.TryMaterialize(out IInventory? inventory))
                    continue;

                NativeArray<ItemContainerUnmanaged> itemContainers = inventory.GetUnmanagedItemContainers(inventoryRef.InventoryID, allocator);

                if (!itemContainers.IsCreated || itemContainers.Length == 0)
                    continue;

                results[inventoryRef] = itemContainers;
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
    }
}
