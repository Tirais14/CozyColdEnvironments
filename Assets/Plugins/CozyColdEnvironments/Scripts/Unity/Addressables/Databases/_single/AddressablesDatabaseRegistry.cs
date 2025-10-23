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
        : CCBehaviourStaticQ<TThis>,
        IAddressablesDatabaseRegistry

        where TThis : CCBehaviourStatic, IAddressablesDatabaseRegistry
    {
        private readonly Dictionary<AssetDatabaseKey, IAddressablesDatabase> databases = new();
        private readonly DatabaseQuery askQuery = new();

        [SerializeField]
        [Tooltip("All loading tasks will be registered in CC.NeccessaryTasks")]
        private AddressablesDatabaseLoadInfo[] loadOrder;

        private Stopwatch? stopwatch;
        private bool disposedValue;

        public event Action? OnStartLoading;
        public event Action? OnLoaded;

        public IAddressablesDatabase this[AssetDatabaseKey key] => databases[key];

        public IEnumerable<AssetDatabaseKey> Keys => databases.Keys;
        public IEnumerable<IAddressablesDatabase> Values => databases.Values;
        public int Count => databases.Count;
        public bool IsLoading => databases.Values.Any(db => db.IsLoading);
        public bool IsLoaded => !IsLoading && Count > 0;

        protected override void Awake()
        {
            base.Awake();

            BindEvents();
            TryLoadByOrder();
        }

        public void RegisterDatabase(IAddressablesDatabase database)
        {
            databases.Add(new AssetDatabaseKey(database.AssetType, database.ID), database);
        }

        public bool UnregisterDatabase(AssetDatabaseKey key) => databases.Remove(key);

        public IAddressablesDatabase GetDatabase(AssetDatabaseKey key) => databases[key];
        public T GetDatabase<T>(AssetDatabaseKey key)
            where T : IAddressablesDatabase
        {
            return GetDatabase(key).As<T>();
        }

        public Object GetAsset(AssetDatabaseKey dbKey, AssetKey assetkey)
        {
            IAddressablesDatabase db;
            try
            {
                db = databases[dbKey];
            }
            catch (KeyNotFoundException)
            {
                throw new DatabaseNotFoundException(dbKey);
            }

            return db.GetAsset(assetkey);
        }
        public T GetAsset<T>(AssetDatabaseKey dbKey, AssetKey assetkey)
        {
            return GetAsset(dbKey, assetkey).As<T>();
        }

        public void Dispose() => Dispose(disposing: true);

        public IEnumerator<KeyValuePair<AssetDatabaseKey, IAddressablesDatabase>> GetEnumerator()
        {
            return databases.GetEnumerator();
        }

        public bool ContainsKey(AssetDatabaseKey key)
        {
            return databases.ContainsKey(key);
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
                databases.Values.CForEach(x => x.Dispose());
                databases.Clear();
                databases.TrimExcess();
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
            return databases.TryGetValue(key, out value);
        }

        public IAddressablesDatabase FindDatabase(AssetDatabaseKey key)
        {
            throw new NotImplementedException();
        }

        public T FindDatabase<T>(AssetDatabaseKey key) where T : IAddressablesDatabase
        {
            throw new NotImplementedException();
        }

        public Ghost<Object?> FindAsset(AssetDatabaseKey dbKey, AssetKey key)
        {
            throw new NotImplementedException();
        }

        public Ghost<T?> FindAsset<T>(AssetDatabaseKey dbKey, AssetKey key)
        {
            throw new NotImplementedException();
        }

        public DatabaseQuery Ask()
        {
            return askQuery.Reset().In(databases.Values);
        }
    }
}
