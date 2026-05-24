using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

#nullable enable
namespace CCEnvs.UnityX.ECS.Items
{
    public struct InventoryRemoveItemQuery : IBufferElementData
    {
        public InventoryReference InventoryRef;

        public ItemReference Item;

        public int ItemCount;

        public bool IsPartialRemoveAllowed;

        [BurstCompile]
        public readonly void Schedule() => InventoryQueryScheduler.Schedule(this);
    }

    public static class InventoryRemoveItemQueryExtensions
    {
        [BurstCompile]
        public static NativeArray<InventoryRemoveItemQuery> GetInventoryQueries(
            this NativeArray<InventoryRemoveItemQuery> queries,
            int inventoryID,
            Allocator allocator
            )
        {
            var filteredQueryList = new NativeList<InventoryRemoveItemQuery>(allocator);

            for (int i = 0; i < queries.Length; i++)
            {
                InventoryRemoveItemQuery query = queries[i];

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
            this NativeArray<InventoryRemoveItemQuery> queries,
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
            this NativeArray<InventoryRemoveItemQuery> queries,
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
