using CCEnvs.UnityX.Items;
using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

#nullable enable
namespace CCEnvs.UnityX.ECS.Items
{
    [InternalBufferCapacity(16)]
    public struct ItemContainerUnmanaged 
        :
        IBufferElementData, 
        IEquatable<ItemContainerUnmanaged>
    {
        public ItemReference Item;

        public int ItemCount;

        public MaybeUnmanaged<InventoryReference> InventoryRef;

        public readonly bool IsEmpty {
            [BurstCompile]
            get => ItemCount <= 0;
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
        public readonly bool ContainsItem(ItemReference item)
        {
            return Item.Equals(item);
        }
        [BurstCompile]
        public readonly bool ContainsItem(ItemReference item, int itemCount)
        {
            return ContainsItem(item) && ItemCount >= itemCount;
        }

        [BurstCompile]
        public readonly InventoryPutItemQuery ToInventoryPutItemQuery(long inventoryID)
        {
            return new InventoryPutItemQuery
            {
                InventoryRef = inventoryID,
                Item = Item,
                ItemCount = ItemCount
            };
        }

        [BurstCompile]
        public readonly InventoryRemoveItemQuery ToInventoryRemoveItemQuery(long inventoryID, bool isPartialRemoveAllowed = false)
        {
            return new InventoryRemoveItemQuery
            {
                InventoryRef = inventoryID,
                Item = Item,
                ItemCount = ItemCount,
                IsPartialRemoveAllowed = isPartialRemoveAllowed
            };
        }

        public readonly override bool Equals(object? obj)
        {
            return obj is ItemContainerUnmanaged unmanaged && Equals(unmanaged);
        }

        public readonly bool Equals(ItemContainerUnmanaged other)
        {
            return Item.Equals(other.Item)
                   &&
                   ItemCount == other.ItemCount
                   &&
                   InventoryRef.EqualsUnmanaged(other.InventoryRef);
        }

        public readonly override int GetHashCode()
        {
            return HashCode.Combine(Item, ItemCount, InventoryRef);
        }
    }

    public static class ItemContainerUnmanagedExtensions
    {
        public static NativeArray<ItemContainerUnmanaged> GetUnmanagedItemContainers(
            this IInventory source, 
            MaybeUnmanaged<long> inventoryID,
            AllocatorManager.AllocatorHandle allocator
            )
        {
            CC.Guard.IsNotNullSource(source);

            if (source.ItemCount == 0)
                return default;

            var unmanagedItemContainers = new NativeList<ItemContainerUnmanaged>(source.ContainerCount, allocator);

            foreach (var itemContainer in source)
            {
                if (itemContainer.IsEmpty 
                    || 
                    !itemContainer.Item.TryGetValue(out IItem? item))
                {
                    continue;
                }

                var unmanagedItemContainer = new ItemContainerUnmanaged
                {
                    InventoryRef = inventoryID.Reinterpret<InventoryReference>(),
                    Item = item.GetUnmanagedReference(),
                    ItemCount = itemContainer.ItemCount
                };

                unmanagedItemContainers.Add(unmanagedItemContainer);
            }

            return unmanagedItemContainers.AsArray();
        }

        public static ItemContainerUnmanaged ToUnmanaged(this IItemContainerInfo source)
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

        public static ItemContainerUnmanaged ToUnmanaged(
            this IItemContainerInfo source, 
            long inventoryID
            )
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
                InventoryRef = new InventoryReference { InventoryID = inventoryID },
                Item = item.ID,
                ItemCount = source.ItemCount
            };
        }

        [BurstCompile]
        public static NativeArray<ItemContainerUnmanaged> GetInventoryContainers<TList>(
            this TList itemContainers,
            long inventoryID,
            Allocator allocator
            )
            where TList : unmanaged, IIndexable<ItemContainerUnmanaged>
        {
            var filteredItemContainers = new NativeList<ItemContainerUnmanaged>(16, allocator);

            for (int i = 0; i < itemContainers.Length; i++)
            {
                ref readonly ItemContainerUnmanaged itemContainer = ref itemContainers.ElementAt(i);

                if (!itemContainer.InventoryRef.HasValue
                    ||
                    itemContainer.InventoryRef.Value != inventoryID)
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
            in InventoryReference inventoryRef,
            Allocator allocator
            )
            where TList : unmanaged, IIndexable<ItemContainerUnmanaged>
        {
            return itemContainers.GetInventoryContainers(inventoryRef.InventoryID, allocator);
        }

        [BurstCompile]
        public static NativeArray<long> GetInventoryContainerIndexes<TList>(
            this TList itemContainers,
            long inventoryID,
            Allocator allocator
            )
            where TList : unmanaged, IIndexable<ItemContainerUnmanaged>
        {
            var filteredItemContainerIDs = new NativeList<long>(16, allocator);

            for (int i = 0; i < itemContainers.Length; i++)
            {
                ref readonly ItemContainerUnmanaged itemContainer = ref itemContainers.ElementAt(i);

                if (!itemContainer.InventoryRef.HasValue
                    ||
                    itemContainer.InventoryRef.Value != inventoryID)
                {
                    continue;
                }

                filteredItemContainerIDs.Add(itemContainer.InventoryRef.Value);
            }

            return filteredItemContainerIDs.AsArray();
        }
        [BurstCompile]
        public static NativeArray<long> GetInventoryContainerIndexes<TList>(
            this TList itemContainers,
            in InventoryReference inventoryRef,
            Allocator allocator
            )
            where TList : unmanaged, IIndexable<ItemContainerUnmanaged>
        {
            return itemContainers.GetInventoryContainerIndexes(inventoryRef.InventoryID, allocator);
        }
    }
}
