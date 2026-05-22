using CCEnvs.UnityX.Items;
using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

#nullable enable
namespace CCEnvs.UnityX.ECS.Items
{
    [InternalBufferCapacity(128)]
    public struct ItemContainerUnmanaged : IBufferElementData, IEquatable<ItemContainerUnmanaged>
    {
        public ItemUnmanged Item;

        public int ItemCount;

        public NullableUnmanaged<int> InventoryID;

        public readonly bool IsEmpty {
            [BurstCompile]
            get => ItemCount > 0;
        }

        public static bool operator ==(ItemContainerUnmanaged left, ItemContainerUnmanaged right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(ItemContainerUnmanaged left, ItemContainerUnmanaged right)
        {
            return !(left == right);
        }

        [BurstCompile]
        public readonly bool ContainsItem() => !IsEmpty;
        [BurstCompile]
        public readonly bool ContainsItem(ItemUnmanged item)
        {
            return Item.Equals(item);
        }
        [BurstCompile]
        public readonly bool ContainsItem(ItemUnmanged item, int itemCount)
        {
            return ContainsItem(item) && ItemCount >= itemCount;
        }

        public readonly override bool Equals(object? obj)
        {
            return obj is ItemContainerUnmanaged unmanaged && Equals(unmanaged);
        }

        public readonly bool Equals(ItemContainerUnmanaged other)
        {
            return Item.Equals(other.Item)
                   &&
                   ItemCount == other.ItemCount;
        }

        public readonly override int GetHashCode()
        {
            return HashCode.Combine(Item, ItemCount);
        }
    }

    public static class ItemContainerUnmanagedExtensions
    {
        public static ItemContainerUnmanaged ConvertToUnmanaged(this IItemContainerInfo source)
        {
            CC.Guard.IsNotNullSource(source);

            if (source.IsEmpty
                ||
                !source.Item.TryGetValue(out var item))
            {
                return default;
            }

            return new ItemContainerUnmanaged
            {
                Item = item.ID,
                ItemCount = source.ItemCount
            };
        }

        public static ItemContainerUnmanaged ConvertToUnmanaged(this IItemContainerInfo source, int inventoryID)
        {
            CC.Guard.IsNotNullSource(source);

            if (source.IsEmpty
                ||
                !source.Item.TryGetValue(out var item))
            {
                return default;
            }

            return new ItemContainerUnmanaged
            {
                InventoryID = inventoryID,
                Item = item.ID,
                ItemCount = source.ItemCount
            };
        }

        [BurstCompile]
        public static NativeArray<ItemContainerUnmanaged> GetInventoryContainers<TList>(
            this TList itemContainers,
            int inventoryID,
            Allocator allocator = Allocator.TempJob
            )
            where TList : unmanaged, IIndexable<ItemContainerUnmanaged>
        {
            var filteredItemContainers = new NativeList<ItemContainerUnmanaged>(16, allocator);

            for (int i = 0; i < itemContainers.Length; i++)
            {
                ref readonly ItemContainerUnmanaged itemContainer = ref itemContainers.ElementAt(i);

                if (!itemContainer.InventoryID.HasValue
                    ||
                    itemContainer.InventoryID != inventoryID)
                {
                    continue;
                }

                filteredItemContainers.Add(itemContainer);
            }

            return filteredItemContainers.AsArray();
        }

        [BurstCompile]
        public static NativeArray<ItemContainerUnmanaged> GetInventoryContainers<TList>(
            this TList itemContainers,
            in InventoryReferenceUnmanged inventoryRef,
            Allocator allocator = Allocator.TempJob
            )
            where TList : unmanaged, IIndexable<ItemContainerUnmanaged>
        {
            return itemContainers.GetInventoryContainers(inventoryRef.InventoryID, allocator);
        }

        [BurstCompile]
        public static NativeArray<int> GetInventoryContainerIndexes<TList>(
            this TList itemContainers,
            int inventoryID,
            Allocator allocator = Allocator.TempJob
            )
            where TList : unmanaged, IIndexable<ItemContainerUnmanaged>
        {
            var filteredItemContainerIDs = new NativeList<int>(16, allocator);

            for (int i = 0; i < itemContainers.Length; i++)
            {
                ref readonly ItemContainerUnmanaged itemContainer = ref itemContainers.ElementAt(i);

                if (!itemContainer.InventoryID.HasValue
                    ||
                    itemContainer.InventoryID != inventoryID)
                {
                    continue;
                }

                filteredItemContainerIDs.Add(itemContainer.InventoryID.Value);
            }

            return filteredItemContainerIDs.AsArray();
        }
        [BurstCompile]
        public static NativeArray<int> GetInventoryContainerIndexes<TList>(
            this TList itemContainers,
            in InventoryReferenceUnmanged inventoryRef,
            Allocator allocator = Allocator.TempJob
            )
            where TList : unmanaged, IIndexable<ItemContainerUnmanaged>
        {
            return itemContainers.GetInventoryContainerIndexes(inventoryRef.InventoryID, allocator);
        }

        [BurstCompile]
        public static bool ContainsItem<ItemContainerPool>(
            this ItemContainerPool itemContainerPool,
            ItemUnmanged item,
            NullableUnmanaged<int> inventoryID = default
            )
            where ItemContainerPool : unmanaged, IIndexable<ItemContainerUnmanaged>
        {
            for (int i = 0; i < itemContainerPool.Length; i++)
            {
                ref readonly ItemContainerUnmanaged itemContainer = ref itemContainerPool.ElementAt(i);

                if (inventoryID.HasValue
                    &&
                    (!itemContainer.InventoryID.HasValue
                    ||
                    itemContainer.InventoryID.Value != inventoryID.Value))
                {
                    continue;
                }

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
        public static bool ContainsItemsWithCount<TItemContainers, TCompareItemContainer>(
            this TItemContainers itemContainers,
            TCompareItemContainer compareItemContainers
            )
            where TItemContainers : unmanaged, INativeList<ItemContainerUnmanaged>
            where TCompareItemContainer : unmanaged, INativeList<ItemContainerUnmanaged>
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
