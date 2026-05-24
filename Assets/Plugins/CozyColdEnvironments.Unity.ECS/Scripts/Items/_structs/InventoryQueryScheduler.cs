using CCEnvs.Attributes;
using CCEnvs.UnityX.ECS.Items;
using CCEnvs.UnityX.Items;
using Unity.Burst;
using Unity.Collections;
using Unity.Mathematics;

#nullable enable
namespace CCEnvs.UnityX.ECS
{
    public readonly struct InventoryQueryScheduler
    {
        private static NativeList<InventoryPutItemQuery> putItemQueries;
        private static NativeList<InventoryRemoveItemQuery> removeItemQueries;

        [BurstCompile]
        public static void Schedule(InventoryPutItemQuery putItemQuery)
        {
            putItemQueries.Add(putItemQuery);
        }

        [BurstCompile]
        public static void Schedule(InventoryRemoveItemQuery removeItemQuery)
        {
            removeItemQueries.Add(removeItemQuery);
        }

        public static void ExecutePutItemQuries()
        {  
            using var processedIndices = new NativeList<int>(putItemQueries.Length, Allocator.Temp);

            for (int i = 0; i < putItemQueries.Length; i++)
            {
                ref readonly InventoryPutItemQuery query = ref putItemQueries.ElementAt(i);

                if (!InventoryRegistry.TryGet(query.InventoryRef, out IInventory? inventory))
                    continue;

                if (query.ItemCount >= 1
                    &&
                    inventory.PutItem(query.Item.ToManaged(), query.ItemCount).TryGetValue(out IItemContainerInfo? restItems))
                {
                    putItemQueries[i] = new InventoryPutItemQuery
                    {
                        Item = query.Item,
                        ItemCount = restItems.ItemCount
                    };

                    continue;
                }

                processedIndices.Add(i);
            }

            RemovePutItemQueries(processedIndices);
        }

        public static void ExecuteRemoveItemQueries()
        {
            var processedIndices = new NativeList<int>(removeItemQueries.Length, Allocator.Temp);

            for (int i = 0; i < removeItemQueries.Length; i++)
            {
                ref readonly InventoryRemoveItemQuery query = ref removeItemQueries.ElementAt(i);

                if (!InventoryRegistry.TryGet(query.InventoryRef, out IInventory? inventory))
                    continue;

                bool isProcessed = query.ItemCount <= 0;

                if (!isProcessed)
                {
                    if (!query.IsPartialRemove)
                    {
                        if (inventory.TakeItem(query.Item.ToManaged(), query.ItemCount).IsNone)
                            continue;

                        isProcessed = true;
                    }
                    else
                    {
                        IItem item = query.Item.ToManaged();
                        int inventoryItemCount = inventory.GetItemCount(item);

                        if (inventoryItemCount <= 0)
                            continue;

                        int takeItemCount = math.min(query.ItemCount, inventoryItemCount);

                        if (!inventory.TakeItem(item, takeItemCount).TryGetValue(out IItemContainerInfo? takedItems))
                            continue;

                        var tQuery = query;
                        tQuery.ItemCount -= takedItems.ItemCount;

                        isProcessed = tQuery.ItemCount >= 0;
                    }

                    if (isProcessed)
                        processedIndices.Add(i);
                }
            }

            RemoveRemoveItemQueries(processedIndices);
        }

        [BurstCompile]
        private static void RemovePutItemQueries(in NativeList<int> indices)
        {
            for (int i = 0; i < indices.Length; i++)
                putItemQueries.RemoveAtSwapBack(indices[i]);

            indices.Dispose();
        }


        [BurstCompile]
        private static void RemoveRemoveItemQueries(in NativeList<int> indices)
        {
            for (int i = 0; i < indices.Length; i++)
                removeItemQueries.RemoveAtSwapBack(indices[i]);

            indices.Dispose();
        }

        [BurstCompile]
        [OnInstallExecutable]
        private static void OnInstall()
        {
            putItemQueries.Dispose();
            removeItemQueries.Dispose();

            putItemQueries = new NativeList<InventoryPutItemQuery>(32, Allocator.Persistent);
            removeItemQueries = new NativeList<InventoryRemoveItemQuery>(32, Allocator.Persistent);
        }
    }
}
