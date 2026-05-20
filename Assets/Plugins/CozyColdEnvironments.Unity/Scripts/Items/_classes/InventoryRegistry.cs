using CCEnvs.Disposables;
using CCEnvs.TypeMatching;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

#nullable enable
namespace CCEnvs.Unity.Items
{
    public static class InventoryRegistry
    {
        public static IReadOnlyDictionary<int, IInventory> Inventories => inventories;

        private static readonly Dictionary<int, IInventory> inventories = new();

        public static LightDisposable<int> Register(int id, IInventory inventory)
        {
            CC.Guard.IsNotNull(inventory, nameof(inventory));

            inventories.Add(id, inventory);

            return CCDisposable.CreateLight(id, (id) => Unregister(id));
        }

        public static bool Unregister(int id)
        {
            return inventories.Remove(id);
        }

        public static IInventory Get(int id) => inventories[id];
        public static T Get<T>(int id)
            where T : IInventory
        {
            return (T)inventories[id];
        }

        public static bool TryGet(int id, [NotNullWhen(true)] out IInventory? inventory)
        {
            return inventories.TryGetValue(id, out inventory);
        }
        public static bool TryGet<T>(int id, [NotNullWhen(true)] out T? inventory)
        {
            if (!TryGet(id, out var inventoryUntyped)
                ||
                inventoryUntyped.IsNot<T>(out inventory))
            {
                inventory = default;
                return false;
            }

            return true;
        }
    }
}
