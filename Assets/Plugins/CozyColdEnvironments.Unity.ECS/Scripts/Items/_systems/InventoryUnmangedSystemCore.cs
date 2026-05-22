using CCEnvs.Threading;
using CCEnvs.UnityX.Items;
using R3;
using System;
using System.Threading;
using Unity.Collections;
using Unity.Entities;

#nullable enable
namespace CCEnvs.UnityX.ECS.Items
{
    public static class InventoryUnmangedSystemCore
    {
        public static bool IsAnyInventoryChanged { get; private set; }

        public static NativeList<int> ChangedInventoryIDs { get; private set; }

        private static CancellationTokenSource? destroyCancellationTokenSource;
        private static IDisposable? anySub;

        public static void Create()
        {
            Destroy();

            destroyCancellationTokenSource = new CancellationTokenSource();

            anySub = InventoryRegistry.ObserveAny(destroyCancellationTokenSource.Token)
                .Subscribe(_ => IsAnyInventoryChanged = true);

            IsAnyInventoryChanged = true;

            ChangedInventoryIDs = new NativeList<int>(Allocator.Persistent);
        }

        public static void Destroy()
        {
            if (anySub.IsNotNull())
            {
                anySub.Dispose();
                anySub = null;
            }

            if (destroyCancellationTokenSource is null)
                return;

            destroyCancellationTokenSource.CancelAndDispose();
            destroyCancellationTokenSource = null;

            IsAnyInventoryChanged = false;

            ChangedInventoryIDs.Dispose();
        }

        public static void ResetIsAnyInventoryChanged()
        {
            IsAnyInventoryChanged = false;
        }

        public static void UpdateInventoryContent(
            IInventory inventory,
            in DynamicBuffer<ItemContainerUnmanaged> items
            )
        {
            CC.Guard.IsNotNull(inventory, nameof(inventory));

            items.Clear();

            using var addedItems = new NativeHashSet<ItemContainerUnmanaged>(64, Allocator.Temp);

            foreach (var itemContainer in inventory)
            {
                if (itemContainer.IsEmpty
                    ||
                    !itemContainer.Item.TryGetValue(out var item))
                {
                    continue;
                }

                var itemContainerUnamnged = new ItemContainerUnmanaged
                {
                    Item = new ItemUnmanged { ID = item.ID },
                    ItemCount = itemContainer.ItemCount,
                };

                if (addedItems.Contains(itemContainerUnamnged))
                    continue;

                items.Add(itemContainerUnamnged);
            }
        }

        public static void ProcessPutItemQueries(
            IInventory inventory,
            in DynamicBuffer<InventoryUnmanagedPutItemQuery> queries
            )
        {
            CC.Guard.IsNotNull(inventory, nameof(inventory));

            for (int i = 0; i < queries.Length; i++)
            {
                InventoryUnmanagedPutItemQuery query = queries[i];

                inventory.PutItem(query.)
            }
        }
    }
}
