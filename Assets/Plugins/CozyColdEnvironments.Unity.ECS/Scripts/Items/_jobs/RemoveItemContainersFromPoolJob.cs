using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

#nullable enable
namespace CCEnvs.UnityX.ECS.Items
{
    [BurstCompile]
    public partial struct RemoveItemContainersFromPoolJob : IJobEntity
    {
        [ReadOnly]
        public NativeHashMap<int, NativeArray<int>> InventoryContainerIndicesMap;

        public void Execute(
            in DynamicBuffer<InventoryReference> inventoryRefs,
            in DynamicBuffer<ItemContainerUnmanaged> itemContainerPool
            )
        {
            for (int i = 0; i < inventoryRefs.Length; i++)
            {
                if (!InventoryContainerIndicesMap.TryGetValue(inventoryRefs[i], out NativeArray<int> inventoryContainerIndices))
                    return;

                for (int j = 0; j < inventoryContainerIndices.Length; j++)
                    itemContainerPool.RemoveAtSwapBack(inventoryContainerIndices[j]);
            }
        }
    }
}
