using Unity.Entities;

#nullable enable
namespace CCEnvs.UnityX.ECS.Items
{
    [InternalBufferCapacity(64)]
    public struct InventoryUnmanagedPutItemQuery : IBufferElementData
    {
        public InventoryReferenceUnmanged InventoryRef;

        public ItemUnmanged Item;

        public int ItemCount;
    }
}
