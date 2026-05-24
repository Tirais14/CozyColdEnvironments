using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

#nullable enable
namespace CCEnvs.UnityX.ECS.Items
{
    [BurstCompile]
    public partial struct UpdateItemContainerPoolsJob : IJobEntity
    {
        [ReadOnly]
        public NativeHashMap<InventoryReference, NativeArray<ItemContainerUnmanaged>> InventoryContainersMap;

        public void Execute(
            in DynamicBuffer<InventoryReference> inventoryRefs,
            ref DynamicBuffer<ItemContainerUnmanaged> itemContainerPool
            )
        {
            if (inventoryRefs.Length == 0)
                return;

            itemContainerPool.Clear();

            for (int i = 0; i < inventoryRefs.Length; i++)
            {
                if (!InventoryContainersMap.TryGetValue(inventoryRefs[i], out NativeArray<ItemContainerUnmanaged> itemContainers))
                    continue;

                for (int j = 0; j < itemContainers.Length; j++)
                    itemContainerPool.Add(itemContainers[i]);
            }
        }
    }
}
