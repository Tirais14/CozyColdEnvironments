using Unity.Entities;

#nullable enable
namespace CCEnvs.UnityX.ECS.Items
{
    [InternalBufferCapacity(4)]
    public struct InventoryReferenceUnmanged : IBufferElementData
    {
        public int InventoryID;
    }
}
