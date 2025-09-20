using CCEnvs.Linq;
using CCEnvs.Reflection;
using CCEnvs.Unity.AddrsAssets.Databases;
using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using Object = UnityEngine.Object;
using static CCEnvs.Unity.AddrsAssets.AddressablesDatabaseRegistry;

#nullable enable
#pragma warning disable S3881
#pragma warning disable IDE1006
namespace CCEnvs.Unity.AddrsAssets
{
    internal static class AddressablesDatabaseRegistry
    {
        internal class AddressablesDatabaseRegistryReflected : Reflected
        {
            public Dictionary<AssetRegistryKey, IAddressablesDatabase> databases { get; }

            public AddressablesDatabaseRegistryReflected(object target,
                                                         Settings settings = Settings.None)
                :
                base(target, settings)
            {
                databases = Field("databases").GetValue<Dictionary<AssetRegistryKey, IAddressablesDatabase>>();
            }
        }
    }
    public abstract class AddressablesDatabaseRegistry<TThis>
        : MonoCCStatic<TThis>,
        IAddressablesDatabaseRegistry

        where TThis : MonoCCStatic, IAddressablesDatabaseRegistry
    {
        private readonly Dictionary<AssetRegistryKey, IAddressablesDatabase> databases = new();
        private AddressablesDatabaseRegistryReflected thisReflected = null!;
        private bool disposedValue;

        public static TThis Q => Instance;
        public IEnumerable<AssetRegistryKey> Keys => databases.Keys;
        public IEnumerable<IAddressablesDatabase> Values => databases.Values;
        public int Count => databases.Count;
        public IAddressablesDatabase this[AssetRegistryKey key] => databases[key];

        protected override void OnAwake()
        {
            base.OnAwake();
            thisReflected = new AddressablesDatabaseRegistryReflected(this);
            try
            {
                var task = RegisterDatabases();
                CC.NeccesaryTasks.RegisterTask(task);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void RegisterDatabase(AssetRegistryKey key, IAddressablesDatabase database)
        {
            thisReflected.databases.Add(key, database);
        }

        public bool UnregisterDatabase(AssetRegistryKey key) => databases.Remove(key);

        public IAddressablesDatabase GetDatabase(AssetRegistryKey key) => databases[key];
        public IAddressablesDatabase GetDatabase(Type assetType,
                                                 object? uniqueIndentifier = null)
        {
            return GetDatabase(new AssetRegistryKey());
        }
        public T GetDatabase<T>(AssetRegistryKey key)
            where T : IAddressablesDatabase
        {
            return GetDatabase(key).As<T>();
        }
        public T GetDatabase<T>(Type dbAssetType,
                                object? uniqueIndentifier = null)
            where T : IAddressablesDatabase
        {
            return GetDatabase(new AssetRegistryKey(
                assetKey: default,
                dbAssetType,
                uniqueIndentifier)).As<T>();
        }

        public Object GetAsset(AssetRegistryKey key)
        {
            IAddressablesDatabase db = thisReflected.databases[key];

            return db.GetAsset(key.AssetKey);
        }
        public Object GetAsset(Type dbAssetType,
                               string? assetName,
                               int assetID,
                               object? uniqueIndentifier = null)
        {
            return GetAsset(new AssetRegistryKey(
                new AssetKey(
                    assetName,
                    assetID,
                    uniqueIndentifier),
                dbAssetType));
        }
        public Object GetAsset(Type dbAssetType,
                               string assetName,
                               object? uniqueIndentifier = null)
        {
            return GetAsset(new AssetRegistryKey(
                new AssetKey(
                    assetName,
                    uniqueIndentifier),
                dbAssetType));
        }
        public Object GetAsset(Type dbAssetType,
                               int assetID,
                               object? uniqueIndentifier = null)
        {
            return GetAsset(new AssetRegistryKey(
                new AssetKey(
                    assetID,
                    uniqueIndentifier),
                dbAssetType));
        }
        public T GetAsset<T>(AssetRegistryKey key) => GetAsset(key).As<T>();
        public T GetAsset<T>(Type? dbAssetType,
                             string? assetName,
                             int assetID,
                             object? uniqueIndentifier = null)
        {
            return GetAsset<T>(new AssetRegistryKey(
                new AssetKey(
                    assetName,
                    assetID,
                    uniqueIndentifier),
                dbAssetType ?? typeof(T)));
        }
        public T GetAsset<T>(Type? dbAssetType,
                             string assetName,
                             object? uniqueIndentifier = null)
        {
            return GetAsset<T>(new AssetRegistryKey(
                new AssetKey(
                    assetName,
                    uniqueIndentifier),
                dbAssetType ?? typeof(T)));
        }
        public T GetAsset<T>(Type? dbAssetType,
                             int assetID,
                             object? uniqueIndentifier = null)
        {
            return GetAsset<T>(new AssetRegistryKey(
                new AssetKey(
                    assetID,
                    uniqueIndentifier),
                dbAssetType ?? typeof(T)));
        }

        public void Dispose() => Dispose(disposing: true);

        public IEnumerator<KeyValuePair<AssetRegistryKey, IAddressablesDatabase>> GetEnumerator()
        {
            return databases.GetEnumerator();
        }

        public bool ContainsKey(AssetRegistryKey key)
        {
            return databases.ContainsKey(key);
        }

        protected abstract UniTask<KeyValuePair<AssetRegistryKey, IAddressablesDatabase>[]> GetDatabases();

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

        bool IReadOnlyDictionary<AssetRegistryKey, IAddressablesDatabase>.TryGetValue(
            AssetRegistryKey key,
            out IAddressablesDatabase value)
        {
            return databases.TryGetValue(key, out value);
        }
    }
}
