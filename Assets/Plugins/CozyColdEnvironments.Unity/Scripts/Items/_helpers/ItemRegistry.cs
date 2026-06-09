using CCEnvs.Disposables;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

#nullable enable
namespace CCEnvs.UnityX.Items
{
    public static class ItemRegistry
    {
        private static readonly Dictionary<int, IItem> items = new();

        public static object ItemsGate { get; } = new();

        public static IReadOnlyDictionary<int, IItem> Items => items;

        public static LightDisposable<int> Register(IItem item)
        {
            CC.Guard.IsNotNull(item, nameof(item));

            lock (ItemsGate)
                items.Add(item.ID, item);

            return CCDisposable.CreateLight(item.ID, static id => Unregister(id));
        }

        public static bool TryRegister(IItem item, out LightDisposable<int> handle)
        {
            CC.Guard.IsNotNull(item, nameof(item));

            lock (ItemsGate)
            {
                if (!items.TryAdd(item.ID, item))
                {
                    handle = default;
                    return false;
                }
            }

            handle = CCDisposable.CreateLight(item.ID, static id => Unregister(id));
            return true;
        }

        public static bool Unregister(int id)
        {
            lock (ItemsGate)
                return items.Remove(id);
        }

        public static bool Contains(int id)
        {
            lock (ItemsGate)
                return items.ContainsKey(id);
        }

        public static bool TryGet(int id, [NotNullWhen(true)] out IItem? item)
        {
            lock (ItemsGate)
                return items.TryGetValue(id, out item);
        }

        public static IItem Get(int id)
        {
            lock (ItemsGate)
                return items[id];
        }
    }
}
