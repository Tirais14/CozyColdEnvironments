using CCEnvs.Diagnostics;
using CCEnvs.Linq;
using CCEnvs.Reflection;
using CCEnvs.Unity.AddrsAssets.Databases;
using CCEnvs.Unity.Timers;
using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using static CCEnvs.Unity.AddrsAssets.AddressablesDatabaseRegistry;
using Object = UnityEngine.Object;

#nullable enable
#pragma warning disable S3881
#pragma warning disable IDE1006
namespace CCEnvs.Unity.AddrsAssets
{
    internal static class AddressablesDatabaseRegistry
    {
        internal class AddressablesDatabaseRegistryReflected : Reflected
        {
            public Dictionary<AssetDatabaseKey, IAddressablesDatabase> databases { get; }

            public AddressablesDatabaseRegistryReflected(object target)
                :
                base(target, Settings.IncludeNonPublic)
            {
                databases = Field<Dictionary<AssetDatabaseKey, IAddressablesDatabase>>("databases").GetValue();
            }
        }
    }
    public abstract class AddressablesDatabaseRegistry<TThis>
        : CCBehaviourStaticQ<TThis>,
        IAddressablesDatabaseRegistry

        where TThis : CCBehaviourStatic, IAddressablesDatabaseRegistry
    {
        private readonly Dictionary<AssetDatabaseKey, IAddressablesDatabase> databases = new();
        protected Action<ILoadable> onStartLoading;
        protected Action<ILoadable> onLoaded;
        private TimerUpdate timer;
        private AddressablesDatabaseRegistryReflected thisReflected = null!;
        private bool disposedValue;

        public event Action<ILoadable> OnStartLoading {
            add => onStartLoading += value;
            remove => onStartLoading = (onStartLoading - value)!;
        }
        public event Action<ILoadable> OnLoaded {
            add => onLoaded += value;
            remove => onLoaded = (onLoaded - value)!;
        }

        public IEnumerable<AssetDatabaseKey> Keys => databases.Keys;
        public IEnumerable<IAddressablesDatabase> Values => databases.Values;
        public int Count => databases.Count;
        public bool IsLoaded => Count > 0;
        public IAddressablesDatabase this[AssetDatabaseKey key] => databases[key];

        protected override void OnAwake()
        {
            base.OnAwake();
            thisReflected = new AddressablesDatabaseRegistryReflected(this);
            timer = TimerUpdate.Create();
            timer.IsUnscaledInterval = true;
            try
            {
                var task = RegisterDatabases();
                CC.NeccesaryTasks.RegisterTask(task);
            }
            catch (Exception)
            {
                throw;
            }

            BindEvents();
        }

        public void RegisterDatabase(AssetDatabaseKey key, IAddressablesDatabase database)
        {
            thisReflected.databases.Add(key, database);
        }

        public bool UnregisterDatabase(AssetDatabaseKey key) => databases.Remove(key);

        public IAddressablesDatabase GetDatabase(AssetDatabaseKey key) => databases[key];
        public IAddressablesDatabase GetDatabase(Type assetType,
                                                 object? uniqueIndentifier = null)
        {
            return GetDatabase(new AssetDatabaseKey());
        }
        public T GetDatabase<T>(AssetDatabaseKey key)
            where T : IAddressablesDatabase
        {
            return GetDatabase(key).As<T>();
        }
        public T GetDatabase<T>(Type dbAssetType,
                                object? uniqueIndentifier = null)
            where T : IAddressablesDatabase
        {
            return GetDatabase(new AssetDatabaseKey(dbAssetType, uniqueIndentifier)).As<T>();
        }

        public Object GetAsset(AssetDatabaseKey dbKey, AssetKey assetkey)
        {
            IAddressablesDatabase db = thisReflected.databases[dbKey];

            return db.GetAsset(assetkey);
        }
        public Object GetAsset(Type dbAssetType,
                               string? assetName,
                               object? assetID,
                               object? dbID = null)
        {
            return GetAsset(new AssetDatabaseKey(dbAssetType, dbID),
                new AssetKey(assetName, assetID));
        }
        public Object GetAsset(Type dbAssetType,
                               string assetName,
                               object? dbID = null)
        {
            return GetAsset(new AssetDatabaseKey(dbAssetType, dbID),
                new AssetKey(assetName));
        }
        public Object GetAsset(Type dbAssetType,
                               object assetID,
                               object? dbID = null)
        {
            return GetAsset(new AssetDatabaseKey(dbAssetType, dbID),
                AssetKey.ByID(assetID));
        }
        public T GetAsset<T>(AssetDatabaseKey dbKey, AssetKey assetkey) => GetAsset(dbKey, assetkey).As<T>();
        public T GetAsset<T>(string? assetName,
                             object? assetID,
                             Type? dbAssetType = null,
                             object? dbID = null)
        {
            return GetAsset<T>(new AssetDatabaseKey(dbAssetType ?? typeof(T), dbID),
                new AssetKey(assetName, assetID));
        }
        public T GetAsset<T>(string assetName,
                             Type? dbAssetType = null,
                             object? dbID = null)
        {
            return GetAsset<T>(new AssetDatabaseKey(dbAssetType ?? typeof(T), dbID),
                new AssetKey(assetName));
        }
        public T GetAsset<T>(object assetID,
                             Type? dbAssetType = null,
                             object? dbID = null)
        {
            return GetAsset<T>(new AssetDatabaseKey(dbAssetType ?? typeof(T), dbID),
                AssetKey.ByID(assetID));
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
            onStartLoading += (x) =>
            {
                timer.StartTimer();
                CCDebug.PrintLog("Loading started", x);
            };

            onLoaded += (x) =>
            {
                timer.StopTimer();
                CCDebug.PrintLog($"Loading finished in {timer.Elapsed.TotalSecondsF()} seconds.", x);
                timer.ResetTimer();
            };
        }

        protected abstract UniTask<KeyValuePair<AssetDatabaseKey, IAddressablesDatabase>[]> GetDatabases();

        protected virtual void Dispose(bool disposing)
        {
            if (disposedValue) 
                return;

            if (disposing)
            {
                databases.Values.ForEach(x => x.Dispose());
                databases.Clear();
                databases.TrimExcess();
            }

            disposedValue = true;
        }

        private async UniTask RegisterDatabases()
        {
            (await GetDatabases()).ForEach(db => RegisterDatabase(db.Key, db.Value));
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        bool IReadOnlyDictionary<AssetDatabaseKey, IAddressablesDatabase>.TryGetValue(
            AssetDatabaseKey key,
            out IAddressablesDatabase value)
        {
            return databases.TryGetValue(key, out value);
        }
    }
}
