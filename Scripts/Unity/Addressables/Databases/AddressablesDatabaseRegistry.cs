using CCEnvs.Diagnostics;
using CCEnvs.Language;
using CCEnvs.Linq;
using CCEnvs.Reflection;
using CCEnvs.Unity.Components;
using CCEnvs.ZLinqExt;
using System;
using System.Collections;
using System.Collections.Generic;
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
        private Stopwatch? stopwatch;
        private bool disposedValue;

        public event Action? OnStartLoading;
        public event Action? OnLoaded;

        public IEnumerable<AssetDatabaseKey> Keys => databases.Keys;
        public IEnumerable<IAddressablesDatabase> Values => databases.Values;
        public int Count => databases.Count;
        public bool IsLoading => databases.Values.Any(db => db.IsLoading);
        public bool IsLoaded => !IsLoading && Count > 0;
        public IAddressablesDatabase this[AssetDatabaseKey key] => databases[key];
        public IAddressablesDatabase this[Type dbAssetType] => GetDatabase(dbAssetType);
        public IAddressablesDatabase this[Type dbAssetType, object dbID] => GetDatabase(dbAssetType, dbID);

        protected override void OnAwake()
        {
            base.OnAwake();

            BindEvents();
        }

        public void RegisterDatabase(AssetDatabaseKey key, IAddressablesDatabase database)
        {
            databases.Add(key, database);
        }
        public void RegisterDatabase(IAddressablesDatabase database)
        {
            databases.Add(new AssetDatabaseKey(database.AssetType), database);
        }

        public bool UnregisterDatabase(AssetDatabaseKey key) => databases.Remove(key);

        public IAddressablesDatabase GetDatabase(AssetDatabaseKey key) => databases[key];
        public IAddressablesDatabase GetDatabase(Type assetType,
                                                 object? dbID = null)
        {
            return GetDatabase(new AssetDatabaseKey());
        }
        public T GetDatabase<T>(AssetDatabaseKey key)
            where T : IAddressablesDatabase
        {
            return GetDatabase(key).As<T>();
        }
        public T GetDatabase<T>(Type dbAssetType,
                                object? dbID = null)
            where T : IAddressablesDatabase
        {
            return GetDatabase(new AssetDatabaseKey(dbAssetType, dbID)).As<T>();
        }

        public IAddressablesDatabase? FindDatabase(Type assetType, bool throwIfNotFound = false)
        {
            CC.Guard.NullArgument(assetType, nameof(assetType));

            var result = Values.AsValueEnumerable()
                               .FirstOrDefault(db => db.AssetType == assetType)
                               .ToEither(Values.ZL().FirstOrDefault(db => db.AssetType.IsType(assetType)))
                               .Resolve<IAddressablesDatabase>();

            if (throwIfNotFound && result is null)
                throw new DatabaseNotFoundException(assetType);

            return result;
        }
        public T? FindDatabase<T>(Type assetType, bool throwIfNotFound = false)
            where T : IAddressablesDatabase
        {
            return FindDatabase(assetType, throwIfNotFound).AsOrDefault<T>();
        }

        /// <exception cref="AssetNotFoundException"></exception>
        public AssetKey? FindAssetKey(Type dbAssetType,
                                      string assetName,
                                      bool ignoreCase = false,
                                      bool throwIfNotFound = false)
        {
            var result = FindDatabase(dbAssetType, throwIfNotFound)
                ?.FindAssetKey(assetName, ignoreCase);

            return result;

        }
        /// <exception cref="AssetNotFoundException"></exception>
        public AssetKey? FindAssetKey(Type dbAssetType,
                                      object assetID,
                                      bool throwIfNotFound = false)
        {
            var result = FindDatabase(dbAssetType, throwIfNotFound)
                ?.FindAssetKey(assetID, throwIfNotFound);

            return result;
        }

        public Object? FindAsset(Type dbAssetType,
                                 string assetName,
                                 bool ignoreCase = false,
                                 bool throwIfNotFound = false)
        {
            var result = FindDatabase(dbAssetType, throwIfNotFound)
                ?.FindAsset(assetName, ignoreCase, throwIfNotFound);

            return result;
        }
        public Object? FindAsset(Type dbAssetType,
                                 object assetID,
                                 bool throwIfNotFound = false)
        {
            var result = FindDatabase(dbAssetType, throwIfNotFound)?.FindAsset(assetID);

            return result;
        }
        public T? FindAsset<T>(string assetName,
                               Type? dbAssetType = null,
                               bool ignoreCase = false,
                               bool throwIfNotFound = false)
        {
            return throwIfNotFound
                   ? 
                   FindAsset(dbAssetType ?? typeof(T), assetName, ignoreCase, throwIfNotFound).As<T>()
                   :
                   FindAsset(dbAssetType ?? typeof(T), assetName, ignoreCase, throwIfNotFound).AsOrDefault<T>();
        }
        public T? FindAsset<T>(object assetID,
                               Type? dbAssetType = null,
                               bool throwIfNotFound = false)
        {
            return throwIfNotFound
                   ?
                   FindAsset(dbAssetType ?? typeof(T), assetID, throwIfNotFound).As<T>()
                   :
                   FindAsset(dbAssetType ?? typeof(T), assetID, throwIfNotFound).AsOrDefault<T>();
        }

        public Object GetAsset(AssetDatabaseKey dbKey, AssetKey assetkey)
        {
            IAddressablesDatabase db = databases[dbKey];

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

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        bool IReadOnlyDictionary<AssetDatabaseKey, IAddressablesDatabase>.TryGetValue(
            AssetDatabaseKey key,
            out IAddressablesDatabase value)
        {
            return databases.TryGetValue(key, out value);
        }
    }
}
