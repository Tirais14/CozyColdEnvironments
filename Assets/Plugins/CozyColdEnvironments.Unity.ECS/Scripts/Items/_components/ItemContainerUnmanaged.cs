using Unity.Entities;

#nullable enable
namespace CCEnvs.UnityX.ECS.Items
{
    [InternalBufferCapacity(16)]
    public struct ItemContainerUnmanaged : IBufferElementData
    {
        public ItemUnmanged Item;
        public int ItemCount;
    }
}
