using Unity.Entities;

#nullable enable
namespace CCEnvs.UnityX.ECS.Items
{
    public struct InventoryUnmanagedPutItemQuery : IBufferElementData
    {
        public InventoryReferenceUnmanged InventoryRef;

        public ItemUnmanged Item;

        public int ItemCount;
    }
}
