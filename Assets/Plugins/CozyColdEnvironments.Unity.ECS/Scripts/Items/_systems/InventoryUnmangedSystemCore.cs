using CCEnvs.Diagnostics;
using CCEnvs.Threading;
using CCEnvs.UnityX.Items;
using R3;
using System;
using System.Collections.Generic;
using System.Threading;
using Unity.Collections;
using Unity.Entities;

#nullable enable
namespace CCEnvs.UnityX.ECS.Items
{
    public static class InventoryUnmangedSystemCore
    {
        public static bool IsAnyInventoryChanged => (changedInventories?.Count ?? 0) > 0;

        public static IReadOnlyDictionary<int, IInventory> ChangedInventories => changedInventories ?? throw new InvalidOperationException($"{typeof(InventoryUnmangedSystemCore)} is not created");

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

        public static void AddInventoryContainers(
            IInventory inventory,
            InventoryReferenceUnmanged inventoryRef,
            in DynamicBuffer<ItemContainerUnmanaged> itemContainerPool
            )
        {
            foreach (var itemContainer in inventory)
            {
                ItemContainerUnmanaged itemContainerUnmanaged = itemContainer.ConvertToUnmanaged(inventoryRef.InventoryID);
                itemContainerPool.Add(itemContainerUnmanaged);
            }
        }

        public static void RemoveInventoryContainers(
            InventoryReferenceUnmanged inventoryRef,
            in DynamicBuffer<ItemContainerUnmanaged> itemContainerPool
            )
        {
            using var inventoryContainerIndexes = itemContainerPool.GetInventoryContainerIndexes(inventoryRef, Allocator.Temp);

            for (int i = 0; i < inventoryContainerIndexes.Length; i++)
            {
                int inventoryContainerIdx = inventoryContainerIndexes[i];
                itemContainerPool.RemoveAtSwapBack(inventoryContainerIdx);
            }
        }

        public static void UpdateInventoryContent(
            in DynamicBuffer<InventoryReferenceUnmanged> inventoryRefs,
            in DynamicBuffer<ItemContainerUnmanaged> itemContainerPool
            )
        {
            for (int i = 0; i < inventoryRefs.Length; i++)
            {
                InventoryReferenceUnmanged inventoryRef = inventoryRefs[i];

                if (!ChangedInventories.TryGetValue(inventoryRef.InventoryID, out var inventory))
                    continue;

                RemoveInventoryContainers(inventoryRef, itemContainerPool);
                AddInventoryContainers(inventory, inventoryRef, itemContainerPool);
            }
        }

        public static void CollectGarbageItemContainerPool(
            in DynamicBuffer<ItemContainerUnmanaged> itemContainerPool
            )
        {
            for (int i = 0; i < itemContainerPool.Length; i++)
            {
                ref readonly ItemContainerUnmanaged itemContainer = ref itemContainerPool.ElementAt(i);

                if (!itemContainer.InventoryID.HasValue)
                    itemContainerPool.RemoveAtSwapBack(i);
            }
        }

        public static void ProcessPutItemQueries(
            in NativeArray<InventoryUnmanagedPutItemQuery> queriesView,
            DynamicBuffer<InventoryUnmanagedPutItemQuery> queries
            )
        {
            for (int i = 0; i < queriesView.Length; i++)
            {
                InventoryUnmanagedPutItemQuery query = queriesView[i];

                if (query.InventoryRef.TryMaterialize(out var inventory)
                    &&
                    inventory.PutItem(query.Item.ConvertToManaged(), query.ItemCount).TryGetValue(out var restItems))
                {
                    query.ItemCount = restItems.ItemCount;

                    if (CCDebug.IsTypeEnabled(typeof(InventoryUnmangedSystemCore)))
                    {
                        var msg = ExceptionMessageBuilder.CreatePooled()
                            .AddMessage("Item not fitted")
                            .AddProperty(nameof(inventory), inventory)
                            .AddProperty(nameof(query.Item), query.Item)
                            .AddProperty("Rest Item Count", query.ItemCount)
                            .ToStringAndDispose();

                        typeof(InventoryUnmangedSystemCore).PrintLog(msg);
                    }

                    queries[i] = query;
                }
                else
                    queries.RemoveAtSwapBack(i);
            }
        }

        public static void ProcessRemoveItemQueries(
            in NativeArray<InventoryUnmanagedRemoveItemQuery> queriesView,
            in DynamicBuffer<InventoryUnmanagedRemoveItemQuery> queries
            )
        {
            for (int i = 0; i < queriesView.Length; i++)
            {
                InventoryUnmanagedRemoveItemQuery query = queriesView[i];

                if (!query.InventoryRef.TryMaterialize(out IInventory? inventory)
                    ||
                    inventory.TakeItem(query.Item.ConvertToManaged(), query.ItemCount).IsSome)
                {
                    queries.RemoveAtSwapBack(i);
                }
            }
        }
    }
}
