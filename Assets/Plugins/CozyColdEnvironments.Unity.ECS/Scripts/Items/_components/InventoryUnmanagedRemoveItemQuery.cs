using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

#nullable enable
namespace CCEnvs.UnityX.ECS.Items
{
    [InternalBufferCapacity(64)]
    public struct InventoryUnmanagedRemoveItemQuery : IBufferElementData
    {
        public InventoryReferenceUnmanged InventoryRef;

        public ItemUnmanged Item;

        public int ItemCount;
    }

    public static class InventoryUnmanagedRemoveItemQueryExtensions
    {
        [BurstCompile]
        public static NativeArray<InventoryUnmanagedRemoveItemQuery> GetInventoryQueries(
            this NativeArray<InventoryUnmanagedRemoveItemQuery> queries,
            int inventoryID,
            Allocator allocator
            )
        {
            var filteredQueryList = new NativeList<InventoryUnmanagedRemoveItemQuery>(allocator);

            for (int i = 0; i < queries.Length; i++)
            {
                InventoryUnmanagedRemoveItemQuery query = queries[i];

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
            this NativeArray<InventoryUnmanagedRemoveItemQuery> queries,
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
            this NativeArray<InventoryUnmanagedRemoveItemQuery> queries,
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
