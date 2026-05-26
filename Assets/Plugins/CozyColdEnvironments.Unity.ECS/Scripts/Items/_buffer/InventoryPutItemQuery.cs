using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

#nullable enable
namespace CCEnvs.UnityX.ECS.Items
{
    public struct InventoryPutItemQuery : IBufferElementData, IEquatable<InventoryPutItemQuery>
    {
        public InventoryReference InventoryRef;

        public ItemReference Item;

        public int ItemCount;

        public static bool operator ==(InventoryPutItemQuery left, InventoryPutItemQuery right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(InventoryPutItemQuery left, InventoryPutItemQuery right)
        {
            return !(left == right);
        }

        public readonly void Schedule() => InventoryQueryScheduler.Schedule(this);

        public readonly override string ToString()
        {
            return ToStringBuilder.CreatePooled()
                .AddProperty(nameof(InventoryRef), InventoryRef)
                .AddProperty(nameof(Item), Item)
                .AddProperty(nameof(ItemCount), ItemCount)
                .ToStringAndDispose();
        }

        public readonly override bool Equals(object? obj)
        {
            return obj is InventoryPutItemQuery query && Equals(query);
        }

        public readonly bool Equals(InventoryPutItemQuery other)
        {
            return InventoryRef.Equals(other.InventoryRef) &&
                   Item.Equals(other.Item) &&
                   ItemCount == other.ItemCount;
        }

        public readonly override int GetHashCode()
        {
            return HashCode.Combine(InventoryRef, Item, ItemCount);
        }
    }

    public static class InventoryUnmanagedPutItemQueryExtensions
    {
        [BurstCompile]
        public static NativeArray<InventoryPutItemQuery> GetInventoryQueries(
            this NativeArray<InventoryPutItemQuery> queries,
            int inventoryID,
            Allocator allocator
            )
        {
            var filteredQueryList = new NativeList<InventoryPutItemQuery>(allocator);

            for (int i = 0; i < queries.Length; i++)
            {
                InventoryPutItemQuery query = queries[i];

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
            this NativeArray<InventoryPutItemQuery> queries,
            int inventoryID
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
            this NativeArray<InventoryPutItemQuery> queries,
            int inventoryID
            )
        {
            for (int i = 0; i < queries.Length; i++)
                if (queries[i].InventoryRef == inventoryID)
                    return true;

            return false;
        }
    }
}
