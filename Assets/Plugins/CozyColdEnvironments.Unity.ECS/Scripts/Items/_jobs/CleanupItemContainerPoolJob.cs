using Unity.Burst;
using Unity.Entities;

#nullable enable
namespace CCEnvs.UnityX.ECS.Items
{
    [BurstCompile]
    public partial struct CleanupItemContainerPoolJob : IJobEntity
    {
        public readonly void Execute(ref DynamicBuffer<ItemContainerUnmanaged> itemContainers)
        {
            for (int i = itemContainers.Length - 1; i >= 0; i--)
                if (!itemContainers[i].InventoryRef.HasValue)
                    itemContainers.RemoveAtSwapBack(i);
        }
    }
}
