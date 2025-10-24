using CCEnvs;
using CCEnvs.Diagnostics;
using CCEnvs.Language;
using CCEnvs.Linq;
using CCEnvs.Reflection;
using CCEnvs.Unity.Components;
using Cysharp.Threading.Tasks;
using SuperLinq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using UnityEngine;
using ZLinq;
using Object = UnityEngine.Object;

#nullable enable
#pragma warning disable S3881
#pragma warning disable IDE1006
namespace CCEnvs.Unity.AddrsAssets.Databases
{
    public abstract class AddressablesDatabaseRegistry<TThis>
        : CCBehaviourStaticIt<TThis>,
        IAddressablesDatabaseRegistry

        where TThis : CCBehaviourStatic, IAddressablesDatabaseRegistry
    {
        private readonly Dictionary<AssetDatabaseKey, IAddressablesDatabase> collection = new();
        private readonly DatabaseQuery query = new();

        [SerializeField]
        [Tooltip("All loading tasks will be registered in CC.NeccessaryTasks")]
        private AddressablesDatabaseLoadInfo[] loadOrder;

        private Stopwatch? stopwatch;
        private bool disposedValue;

        public event Action? OnStartLoading;
        public event Action? OnLoaded;

        public IAddressablesDatabase this[AssetDatabaseKey key] => GetDatabase(key);
        public Object this[AssetDatabaseKey dbKey, AssetKey key] =>  GetAsset(dbKey, key);

        public IEnumerable<AssetDatabaseKey> Keys => collection.Keys;
        public IEnumerable<IAddressablesDatabase> Values => collection.Values;
        public int Count => collection.Count;
        public bool IsLoading => collection.Values.Any(db => db.IsLoading);
        public bool IsLoaded => !IsLoading && Count > 0;
        public DatabaseQuery Q => Query();

        protected override void Awake()
        {
            base.Awake();

            BindEvents();
            TryLoadByOrder();
        }

        public DatabaseQuery Query() => query.Reset().In(this);

        public void RegisterDatabase(IAddressablesDatabase database)
        {
            collection.Add(new AssetDatabaseKey(database.AssetType, database.ID), database);
        }

        public bool UnregisterDatabase(AssetDatabaseKey key) => collection.Remove(key);

        public Ghost<IAddressablesDatabase> FindDatabase(AssetDatabaseKey key)
        {
            if (collection.TryGetValue(key, out IAddressablesDatabase db))
                return db.ToGhost();

            return Values.ZL()
                .FirstOrDefault(
                    db => key.DatabaseID.Map(id => db.ID == id)
                        .Value(true)
                        &&
                    key.AssetType.Map(type => type == key.AssetType)
                        .Value(true)
                        )!.ToGhost();
        }

        public Ghost<T> FindDatabase<T>(AssetDatabaseKey key) where T : IAddressablesDatabase
        {
            return FindDatabase(key).Map(db => db.As<T>());
        }

        public IAddressablesDatabase GetDatabase(AssetDatabaseKey key)
        {
            if (!collection.TryGetValue(key, out IAddressablesDatabase db))
            {
                Ghost<IAddressablesDatabase> t = FindDatabase(key);

                if (t.IsNone)
                    throw new DatabaseNotFoundException(key);
            }

            return db;
        }
        public T GetDatabase<T>(AssetDatabaseKey key)
            where T : IAddressablesDatabase
        {
            return GetDatabase(key).As<T>();
        }

        public Ghost<Object> FindAsset(AssetDatabaseKey dbKey, AssetKey key)
        {
            if (collection.TryGetValue(dbKey, out IAddressablesDatabase db)
                &&
                new Trapped<Object>(() => db[key]).Value() is Object asset
                )
                return asset;

            return FindDatabase(dbKey).Map(db => db.FindAsset(key).Value()!);
        }

        public Ghost<T> FindAsset<T>(AssetDatabaseKey dbKey, AssetKey key)
        {
            return FindAsset(dbKey, key).Map(x => x.AsOrDefault<T>()).Value();
        }

        public Object GetAsset(AssetDatabaseKey dbKey, AssetKey assetkey)
        {
            return GetDatabase(dbKey).GetAsset(assetkey);
        }
        public T GetAsset<T>(AssetDatabaseKey dbKey, AssetKey assetkey)
        {
            return GetAsset(dbKey, assetkey).As<T>();
        }

        public void Dispose() => Dispose(disposing: true);

        public IEnumerator<KeyValuePair<AssetDatabaseKey, IAddressablesDatabase>> GetEnumerator()
        {
            return collection.GetEnumerator();
        }

        public bool ContainsKey(AssetDatabaseKey key)
        {
            return collection.ContainsKey(key);
        }

        protected void BindEvents()
        {
            OnStartLoading += () =>
            {
                stopwatch ??= new Stopwatch();
                stopwatch.Start();
                this.PrintLog("Loading started");
            };

            OnLoaded += () =>
            {
                stopwatch!.Stop();
                this.PrintLog($"Loading finished in {stopwatch.Elapsed.TotalSeconds} seconds.");
            };
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposedValue) 
                return;

            if (disposing)
            {
                collection.Values.CForEach(x => x.Dispose());
                collection.Clear();
                collection.TrimExcess();
            }

            disposedValue = true;
        }

        private void TryLoadByOrder()
        {
            if (loadOrder.IsNullOrEmpty())
                return;

            var tasks = (from loadInfo in loadOrder
                         select (loadInfo, value: loadInfo.GetDatabaseType()) into dbType
                         select (dbType.loadInfo, value: InstanceFactory.Create(
                             dbType.value,
                             parameters: InstanceFactory.Parameters.ThrowIfNotFound)
                         .As<IAddressablesDatabase>()) into db
                         select (value: db.value.LoadAssetsAsync(db.loadInfo.AssetLabels), db: db.value)
                         ).Do(task => RegisterDatabase(task.db))
                         .Select(task => task.value);

            tasks.ForEach(CC.NeccesaryTasks.RegisterTask);
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        bool IReadOnlyDictionary<AssetDatabaseKey, IAddressablesDatabase>.TryGetValue(
            AssetDatabaseKey key,
            out IAddressablesDatabase value)
        {
            return collection.TryGetValue(key, out value);
        }
    }
}
