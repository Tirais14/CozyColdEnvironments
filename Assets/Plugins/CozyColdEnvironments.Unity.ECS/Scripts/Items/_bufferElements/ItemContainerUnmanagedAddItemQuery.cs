using CCEnvs.UnityX.ECS.Items;
using Unity.Entities;

#nullable enable
namespace CCEnvs.UnityX.ECS.Items
{
    public struct ItemContainerUnmanagedAddItemQuery : IBufferElementData
    {


        public ItemUnmanged Item;

        public int ItemCount;

        public static implicit operator ItemContainerUnmanagedAddItemQuery(ItemUnmanged item)
        {
            return new ItemContainerUnmanagedAddItemQuery { Item = item, ItemCount = 1 };
        }
    }
}
