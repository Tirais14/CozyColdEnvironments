using CCEnvs.Disposables;
using CCEnvs.TypeMatching;
using ObservableCollections;
using R3;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading;

#nullable enable
namespace CCEnvs.UnityX.Items
{
    public class InventoryRegistry
    {
        public static IReadOnlyObservableDictionary<long, IInventory> Inventories => inventories;

        private static readonly ObservableDictionary<long, IInventory> inventories = new();

        private static readonly Dictionary<long, IDisposable> inventorySubs = new();

        private static readonly object inventorySubsGate = new();

        private static ReactiveCommand<KeyValuePair<long, IInventory>>? onInventoryChanged;

        public static LightDisposable<long> Register(long id, IInventory inventory)
        {
            CC.Guard.IsNotNull(inventory, nameof(inventory));

            inventories.Add(id, inventory);
            OnRegister(id, inventory);

            return CCDisposable.CreateLight(id, static (id) => Unregister(id));
        }

        public static bool TryRegister(long id, IInventory inventory, out LightDisposable<long> handle)
        {
            if (Contains(id))
            {
                handle = default;
                return false;
            }

            handle = Register(id, inventory);
            return true;
        }

        public static bool Unregister(long id)
        {
            if (!inventories.Remove(id))
                return false;

            OnUnregister(id);
            return true;
        }

        public static bool Contains(long id) => inventories.ContainsKey(id);

        public static IInventory Get(long id) => inventories[id];
        public static T Get<T>(long id)
            where T : IInventory
        {
            return (T)inventories[id];
        }

        public static bool TryGet(long id, [NotNullWhen(true)] out IInventory? inventory)
        {
            return inventories.TryGetValue(id, out inventory);
        }
        public static bool TryGet<T>(long id, [NotNullWhen(true)] out T? inventory)
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

        public static Observable<KeyValuePair<long, IInventory>> ObserveRegister(CancellationToken cancellationToken = default)
        {
            var replaceEv = inventories.ObserveDictionaryReplace(cancellationToken)
                .Select(pair => KeyValuePair.Create(pair.Key, pair.NewValue));

            return inventories.ObserveDictionaryAdd(cancellationToken)
                .Select(pair => KeyValuePair.Create(pair.Key, pair.Value))
                .Merge(replaceEv);
        }

        public static Observable<KeyValuePair<long, IInventory>> ObserveUnregister(CancellationToken cancellationToken = default)
        {
            var replaceEv = inventories.ObserveDictionaryReplace(cancellationToken)
                .Select(pair => KeyValuePair.Create(pair.Key, pair.OldValue));

            return inventories.ObserveDictionaryRemove(cancellationToken)
                .Select(pair => KeyValuePair.Create(pair.Key, pair.Value))
                .Merge(replaceEv);
        }

        public static Observable<KeyValuePair<long, IInventory>> ObserveInventoryChanged(CancellationToken cancellationToken = default)
        {
            onInventoryChanged ??= new ReactiveCommand<KeyValuePair<long, IInventory>>();
            return onInventoryChanged;
        }

        public static Observable<KeyValuePair<long, IInventory>> ObserveAny(CancellationToken cancellationToken = default)
        {
            return ObserveInventoryChanged(cancellationToken)
                .Merge(ObserveRegister(cancellationToken))
                .Merge(ObserveUnregister(cancellationToken));
        }

        private static void OnRegister(long id, IInventory inventory)
        {
            lock (inventorySubsGate)
                inventorySubs[id] = inventory.ObserveItemCount()
                    .Where(static _ => onInventoryChanged is not null)
                    .Subscribe((id, inventory), static (_, args) => onInventoryChanged!.Execute(KeyValuePair.Create(args.id, args.inventory)));
        }

        private static void OnUnregister(long id)
        {
            lock (inventorySubsGate)
                if (inventorySubs.TryGetValue(id, out var sub))
                    sub.Dispose();
        }
    }
}
