using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

#nullable enable
namespace CCEnvs.UnityX.ECS.Items
{
    public struct InventoryRemoveItemQuery : IBufferElementData, IEquatable<InventoryRemoveItemQuery>
    {
        public InventoryReference InventoryRef;

        public ItemReference Item;

        public int ItemCount;

        public bool IsPartialRemoveAllowed;

        public static bool operator ==(InventoryRemoveItemQuery left, InventoryRemoveItemQuery right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(InventoryRemoveItemQuery left, InventoryRemoveItemQuery right)
        {
            return !(left == right);
        }

        [BurstCompile]
        public readonly void Schedule() => InventoryQueryScheduler.Schedule(this);

        public readonly override string ToString()
        {
            return ToStringBuilder.CreatePooled()
                .AddProperty(nameof(InventoryRef), InventoryRef)
                .AddProperty(nameof(Item), Item)
                .AddProperty(nameof(ItemCount), ItemCount)
                .AddProperty(nameof(IsPartialRemoveAllowed), IsPartialRemoveAllowed)
                .ToStringAndDispose();
        }

        public readonly override bool Equals(object? obj)
        {
            return obj is InventoryRemoveItemQuery query && Equals(query);
        }

        public readonly bool Equals(InventoryRemoveItemQuery other)
        {
            return InventoryRef.Equals(other.InventoryRef) &&
                   Item.Equals(other.Item) &&
                   ItemCount == other.ItemCount &&
                   IsPartialRemoveAllowed == other.IsPartialRemoveAllowed;
        }

        public readonly override int GetHashCode()
        {
            return HashCode.Combine(InventoryRef, Item, ItemCount, IsPartialRemoveAllowed);
        }
    }

    public static class InventoryRemoveItemQueryExtensions
    {
        [BurstCompile]
        public static NativeArray<InventoryRemoveItemQuery> GetInventoryQueries(
            this NativeArray<InventoryRemoveItemQuery> queries,
            long inventoryID,
            Allocator allocator
            )
        {
            var filteredQueryList = new NativeList<InventoryRemoveItemQuery>(allocator);

            for (int i = 0; i < queries.Length; i++)
            {
                InventoryRemoveItemQuery query = queries[i];

                if (query.InventoryRef != inventoryID)
                    continue;

                filteredQueryList.Add(query);
            }

            if (filteredQueryList.IsEmpty)
            {
                filteredQueryList.Dispose();
                return default;
            }

            return filteredQueryList.AsArray();
        }

        [BurstCompile]
        public static int GetInventoryQueryCount(
            this NativeArray<InventoryRemoveItemQuery> queries,
            long inventoryID
            )
        {
            int count = 0;

            for (int i = 0; i < queries.Length; i++)
                if (queries[i].InventoryRef == inventoryID)
                    count++;

            return count;
        }

        [BurstCompile]
        public static bool HasInventoryQuery(
            this NativeArray<InventoryRemoveItemQuery> queries,
            long inventoryID
            )
        {
            for (int i = 0; i < queries.Length; i++)
                if (queries[i].InventoryRef == inventoryID)
                    return true;

            return false;
        }
    }
}
