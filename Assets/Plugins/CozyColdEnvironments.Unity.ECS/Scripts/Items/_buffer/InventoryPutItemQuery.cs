using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

#nullable enable
namespace CCEnvs.UnityX.ECS.Items
{
    public struct InventoryPutItemQuery : IBufferElementData
    {
        public InventoryReference InventoryRef;

        public ItemReference Item;

        public int ItemCount;

        public readonly void Schedule() => InventoryQueryScheduler.Schedule(this);
    }

    public static class InventoryUnmanagedPutItemQueryExtensions
    {
        [BurstCompile]
        public static NativeArray<InventoryPutItemQuery> GetInventoryQueries(
            this NativeArray<InventoryPutItemQuery> queries,
            int inventoryID,
            Allocator allocator
            )
        {
            var filteredQueryList = new NativeList<InventoryPutItemQuery>(allocator);

            for (int i = 0; i < queries.Length; i++)
            {
                InventoryPutItemQuery query = queries[i];

                if (query.InventoryRef != inventoryID)
                    continue;

                filteredQueryList.Add(query);
            }

            if (filteredQueryList.IsEmpty)
            {
                filteredQueryList.Dispose();
                return default;
            }

            return filteredQueryList.AsArray();
        }

        [BurstCompile]
        public static int GetInventoryQueryCount(
            this NativeArray<InventoryPutItemQuery> queries,
            int inventoryID
            )
        {
            int count = 0;

            for (int i = 0; i < queries.Length; i++)
                if (queries[i].InventoryRef == inventoryID)
                    count++;

            return count;
        }

        [BurstCompile]
        public static bool HasInventoryQuery(
            this NativeArray<InventoryPutItemQuery> queries,
            int inventoryID
            )
        {
            for (int i = 0; i < queries.Length; i++)
                if (queries[i].InventoryRef == inventoryID)
                    return true;

            return false;
        }
    }
}
