#nullable enable
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

namespace CCEnvs.UnityX.ECS.Items    
{
    public static class ItemContainerDynamicBufferExtensions
    {
        [BurstCompile]
        public static bool ContainsItem(
            this in DynamicBuffer<ItemContainerUnmanaged> itemContainers,
            ItemUnmanged item
            )
        {
            for (int i = 0; i < itemContainers.Length; i++)
            {
                ItemContainerUnmanaged itemContainer = itemContainers[i];

                if (itemContainer.IsEmpty
                    ||
                    itemContainer.Item != item
                    )
                    continue;

                return true;
            }

            return false;
        }

        [BurstCompile]
        public static bool ContainsItem(
            this in DynamicBuffer<ItemContainerUnmanaged> itemContainers,
            ItemUnmanged item,
            int itemCount
            )
        {
            for (int i = 0; i < itemContainers.Length; i++)
            {
                ItemContainerUnmanaged itemContainer = itemContainers[i];

                if (itemContainer.IsEmpty
                    ||
                    itemContainer.Item != item
                    ||
                    itemContainer.ItemCount < itemCount
                    )
                    continue;

                return true;
            }

            return false;
        }

        [BurstCompile]
        public static bool ContainsItems<TList>(
            this in DynamicBuffer<ItemContainerUnmanaged> itemContainers,
            TList compareItems
            )
            where TList : unmanaged, INativeList<ItemUnmanged>
        {
            if (itemContainers.Length == 0
                ||
                compareItems.Length == 0)
            {
                return false;
            }

            using var foundItems = new NativeBitArray(compareItems.Length, Allocator.Temp);
            foundItems.SetBits(0, false, compareItems.Length);

            for (int i = 0; i < itemContainers.Length; i++)
            {
                ItemContainerUnmanaged itemContainer = itemContainers[i];

                for (int j = 0; j < compareItems.Length; j++)
                {
                    ItemUnmanged compareItem = compareItems[j];

                    if (!itemContainer.ContainsItem(compareItem))
                        continue;

                    foundItems.Set(j, true);
                }
            }

            return foundItems.TestAll(0, compareItems.Length);
        }

        [BurstCompile]
        public static bool ContainsItemsWithCount<TList>(
            this in DynamicBuffer<ItemContainerUnmanaged> itemContainers,
            TList compareItemContainers
            )
            where TList : unmanaged, INativeList<ItemContainerUnmanaged>
        {
            if (itemContainers.Length == 0
                ||
                compareItemContainers.Length == 0)
            {
                return false;
            }

            using var foundItems = new NativeBitArray(compareItemContainers.Length, Allocator.Temp);
            foundItems.SetBits(0, false, compareItemContainers.Length);

            for (int i = 0; i < itemContainers.Length; i++)
            {
                ItemContainerUnmanaged itemContainer = itemContainers[i];

                for (int j = 0; j < compareItemContainers.Length; j++)
                {
                    ItemContainerUnmanaged compareItemContainer = compareItemContainers[j];

                    if (!itemContainer.ContainsItem(compareItemContainer.Item, compareItemContainer.ItemCount))
                        continue;

                    foundItems.Set(j, true);
                }
            }

            return foundItems.TestAll(0, compareItemContainers.Length);
        }
    }
}
