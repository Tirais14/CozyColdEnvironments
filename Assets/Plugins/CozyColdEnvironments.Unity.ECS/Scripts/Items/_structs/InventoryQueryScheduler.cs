using CCEnvs.Attributes;
using CCEnvs.Diagnostics;
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
        private readonly static SharedStatic<NativeList<InventoryPutItemQuery>> putItemQueries = SharedStatic<NativeList<InventoryPutItemQuery>>.GetOrCreate<InventoryQueryScheduler, PutItemQueriesContext>();
        private readonly static SharedStatic<NativeList<InventoryRemoveItemQuery>> removeItemQueries = SharedStatic<NativeList<InventoryRemoveItemQuery>>.GetOrCreate<InventoryQueryScheduler, RemoveItemQueriesContext>();

        public static ref NativeList<InventoryPutItemQuery> PutItemQueries => ref putItemQueries.Data;
        public static ref NativeList<InventoryRemoveItemQuery> RemoveItemQueries => ref removeItemQueries.Data;

        [BurstCompile]
        public static void Schedule(InventoryPutItemQuery putItemQuery)
        {
            PutItemQueries.Add(putItemQuery);
        }

        [BurstCompile]
        public static void Schedule(InventoryRemoveItemQuery removeItemQuery)
        {
            RemoveItemQueries.Add(removeItemQuery);
        }

        public static void ExecutePutItemQuries()
        {
            var processedIndices = new NativeList<int>(PutItemQueries.Length, Allocator.Temp);

            for (int i = 0; i < PutItemQueries.Length; i++)
            {
                ref readonly InventoryPutItemQuery query = ref PutItemQueries.ElementAt(i);

                if (!InventoryRegistry.TryGet(query.InventoryRef, out IInventory? inventory))
                    continue;

                if (query.ItemCount >= 1
                    &&
                    inventory.PutItem(query.Item.ToManaged(), query.ItemCount).TryGetValue(out IItemContainerInfo? restItems))
                {
                    InventoryPutItemQuery fitQuery = query;

                    fitQuery.ItemCount = restItems.ItemCount;
                    PutItemQueries[i] = fitQuery;

#if CC_DEBUG_ENABLED
                    InventoryPutItemQuery debugQuery = query;
                    debugQuery.ItemCount = query.ItemCount - restItems.ItemCount;

                    PrintPutItemQueryCompletionLog(debugQuery);
#endif

                    continue;
                }

                processedIndices.Add(i);

#if CC_DEBUG_ENABLED
                PrintPutItemQueryCompletionLog(query);
#endif
            }

            RemovePutItemQueries(processedIndices);
            processedIndices.Dispose();
        }

        public static void ExecuteRemoveItemQueries()
        {
            var processedIndices = new NativeList<int>(RemoveItemQueries.Length, Allocator.Temp);

            for (int i = 0; i < RemoveItemQueries.Length; i++)
            {
                ref readonly InventoryRemoveItemQuery query = ref RemoveItemQueries.ElementAt(i);

                if (!InventoryRegistry.TryGet(query.InventoryRef, out IInventory? inventory))
                    continue;

                bool isProcessed = query.ItemCount <= 0;

                if (!isProcessed)
                {
                    if (!query.IsPartialRemoveAllowed)
                    {
                        if (inventory.TakeItem(query.Item.ToManaged(), query.ItemCount).IsNone)
                            continue;

                        isProcessed = true;
                    }
                    else
                        ExecutePartiallyRemove(inventory, query, ref isProcessed);

                    if (isProcessed)
                        processedIndices.Add(i);

#if CC_DEBUG_ENABLED
                    if (isProcessed)
                        PrintRemoveItemQueryCompletionLog(query);
#endif
                }
            }

            RemoveRemoveItemQueries(processedIndices);
            processedIndices.Dispose();
        }

        private static void ExecutePartiallyRemove(
            IInventory inventory,
            in InventoryRemoveItemQuery query,
            ref bool isProcessed
            )
        {
            IItem item = query.Item.ToManaged();
            int inventoryItemCount = inventory.GetItemCount(item);

            if (inventoryItemCount <= 0)
                return;

            int takeItemCount = math.min(query.ItemCount, inventoryItemCount);

            if (!inventory.TakeItem(item, takeItemCount).TryGetValue(out IItemContainerInfo? takedItems))
                return;

            var fitQuery = query;
            fitQuery.ItemCount -= takedItems.ItemCount;

            isProcessed = fitQuery.ItemCount <= 0;

#if CC_DEBUG_ENABLED
            if (!isProcessed)
            {
                InventoryRemoveItemQuery debugQuery = query;
                debugQuery.ItemCount = takedItems.ItemCount;

                PrintRemoveItemQueryCompletionLog(debugQuery);
            }
#endif
        }

        [BurstCompile]
        private static void RemovePutItemQueries(in NativeList<int> indices)
        {
            for (int i = 0; i < indices.Length; i++)
                PutItemQueries.RemoveAtSwapBack(indices[i]);
        }


        [BurstCompile]
        private static void RemoveRemoveItemQueries(in NativeList<int> indices)
        {
            for (int i = 0; i < indices.Length; i++)
                RemoveItemQueries.RemoveAtSwapBack(indices[i]);
        }

#if CC_DEBUG_ENABLED
        private static void PrintPutItemQueryCompletionLog(in InventoryPutItemQuery query)
        {
            if (CCDebug.IsTypeEnabled<InventoryPutItemQuery>())
            {
                typeof(InventoryPutItemQuery).PrintLog(
                    ExceptionMessageBuilder.CreatePooled()
                    .AddMessage("Item added")
                    .AddProperty(nameof(query), query)
                    .ToStringAndDispose()
                    );
            }
        }

        private static void PrintRemoveItemQueryCompletionLog(in InventoryRemoveItemQuery query)
        {
            if (CCDebug.IsTypeEnabled<InventoryRemoveItemQuery>())
            {
                typeof(InventoryRemoveItemQuery).PrintLog(
                    ExceptionMessageBuilder.CreatePooled()
                    .AddMessage("Item removed")
                    .AddProperty(nameof(query), query)
                    .ToStringAndDispose()
                    );
            }
        }
#endif

        [BurstCompile]
        [OnInstallExecutable]
        private static void OnInstall()
        {
            PutItemQueries.Dispose();
            RemoveItemQueries.Dispose();

            PutItemQueries = new NativeList<InventoryPutItemQuery>(32, Allocator.Persistent);
            RemoveItemQueries = new NativeList<InventoryRemoveItemQuery>(32, Allocator.Persistent);
        }

        private readonly struct PutItemQueriesContext { }
        private readonly struct RemoveItemQueriesContext { }
    }
}
