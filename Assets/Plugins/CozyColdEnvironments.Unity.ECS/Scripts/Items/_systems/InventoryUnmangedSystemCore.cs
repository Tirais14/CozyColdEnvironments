using CCEnvs.Threading;
using CCEnvs.Unity.Items;
using R3;
using System;
using System.Threading;
using Unity.Collections;
using Unity.Entities;

#nullable enable
namespace CCEnvs.Unity.ECS.Items
{
    public static class InventoryUnmangedSystemCore
    {
        public static bool IsAnyInventoryChanged { get; private set; }

        private static CancellationTokenSource? destroyCancellationTokenSource;
        private static IDisposable? anySub;

        public static void Create()
        {
            Destroy();

            destroyCancellationTokenSource = new CancellationTokenSource();

            anySub = InventoryRegistry.ObserveAny(destroyCancellationTokenSource.Token)
                .Subscribe(_ => IsAnyInventoryChanged = true);

            IsAnyInventoryChanged = true;
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
        }

        public static void ResetIsAnyInventoryChanged()
        {
            IsAnyInventoryChanged = false;
        }

        public static bool TryUpdateInventoryContent(
            in InventoryReferenceUnmanged inventoryRef,
            in DynamicBuffer<ItemUnmanged> items
            )
        {
            if (!InventoryRegistry.TryGet(inventoryRef.InventoryID, out var inventory))
            {
                typeof(InventoryUnmangedSystemCore).PrintError($"Cannot find inventory. InventoryID: {inventoryRef.InventoryID}");
                return false;
            }

            items.Clear();

            using var addedItems = new NativeHashSet<ItemUnmanged>(64, Allocator.Temp);

            foreach (var itemContainer in inventory)
            {
                if (itemContainer.IsEmpty
                    ||
                    !itemContainer.Item.TryGetValue(out var item))
                {
                    continue;
                }

                var itemUnmanaged = new ItemUnmanged
                {
                    ID = item.ID
                };

                if (addedItems.Contains(itemUnmanaged))
                    continue;

                items.Add(itemUnmanaged);
            }

            return true;
        }
    }
}
