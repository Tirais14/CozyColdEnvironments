using CCEnv;
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

        [SerializeField]
        [Tooltip("All loading tasks will be registered in CC.NeccessaryTasks")]
        private AddressablesDatabaseLoadInfo[] loadOrder;

        private Stopwatch? stopwatch;
        private bool disposedValue;

        public event Action? OnStartLoading;
        public event Action? OnLoaded;

        public IAddressablesDatabase this[AssetDatabaseKey key] => databases[key];
        public IAddressablesDatabase this[Type dbAssetType] => GetDatabase(dbAssetType);
        public IAddressablesDatabase this[Type dbAssetType,
                                          UniID dbID] => GetDatabase(dbAssetType, dbID);
        
        public IAddressablesDatabase this[Type dbAssetType,
                                          int num0,
                                          int num1,
                                          string str0,
                                          string str1] => this[dbAssetType, new UniID(num0, num1, str0, str1)];

        public IAddressablesDatabase this[Type dbAssetType,
                                          int num0,
                                          string str0] => this[dbAssetType, new UniID(num0, str0)];

        public IAddressablesDatabase this[Type dbAssetType,
                                          string str0,
                                          string str1] => this[dbAssetType, new UniID(str0, str1)];

        public IAddressablesDatabase this[Type dbAssetType,
                                          string str0] => this[dbAssetType, new UniID(str0)];

        public IAddressablesDatabase this[Type dbAssetType,
                                          int num0,
                                          int num1] => this[dbAssetType, new UniID(num0, num1)];

        public IAddressablesDatabase this[Type dbAssetType,
                                          int num0] => this[dbAssetType, new UniID(num0)];

        public IAddressablesDatabase this[Type dbAssetType,
                                          Enum value] {
            get => this[dbAssetType, UniID.FromEnum(value)];
        }

        public IAddressablesDatabase this[Type dbAssetType,
                                          Enum value,
                                          Enum value1] {
            get => this[dbAssetType, UniID.FromEnum(value, value1)];
        }

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
        public IAddressablesDatabase GetDatabase(Type assetType,
                                                 UniID dbID = default)
        {
            return GetDatabase(new AssetDatabaseKey(assetType, dbID));
        }
        public T GetDatabase<T>(AssetDatabaseKey key)
            where T : IAddressablesDatabase
        {
            return GetDatabase(key).As<T>();
        }
        public T GetDatabase<T>(Type dbAssetType,
                                UniID dbID = default)
            where T : IAddressablesDatabase
        {
            return GetDatabase(new AssetDatabaseKey(dbAssetType, dbID)).As<T>();
        }

        public IAddressablesDatabase? FindDatabase(Type assetType,
                                                   bool throwIfNotFound = false)
        {
            CC.Guard.NullArgument(assetType, nameof(assetType));

            var result = Values.ZL()
                               .FirstOrDefault(db => db.AssetType == assetType)
                               .Either(Values.ZL().FirstOrDefault(db => db.AssetType.IsType(assetType)))
                               .Resolve<IAddressablesDatabase>();

            if (throwIfNotFound && result is null)
                throw new DatabaseNotFoundException(assetType);

            return result;
        }
        public IAddressablesDatabase? FindDatabase(Type assetType,
                                                   UniID dbID,
                                                   bool throwIfNotFound = false)
        {
            CC.Guard.NullArgument(assetType, nameof(assetType));

            var result = Values.ZL()
                               .FirstOrDefault(db => db.AssetType == assetType && db.ID == dbID)
                               .Either(Values.ZL().FirstOrDefault(db => db.AssetType.IsType(assetType)))
                               .Resolve<IAddressablesDatabase>();

            if (throwIfNotFound && result is null)
                throw new DatabaseNotFoundException(assetType);

            return result;
        }
        public T? FindDatabase<T>(Type assetType,
                                  bool throwIfNotFound = false)
            where T : IAddressablesDatabase
        {
            return FindDatabase(assetType, throwIfNotFound).AsOrDefault<T>();
        }
        public T? FindDatabase<T>(Type assetType,
                                  UniID dbID,
                                  bool throwIfNotFound = false)
            where T : IAddressablesDatabase
        {
            return FindDatabase(assetType, dbID, throwIfNotFound).AsOrDefault<T>();
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
                                      int assetID,
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
                                 int assetID,
                                 bool throwIfNotFound = false)
        {
            var result = FindDatabase(dbAssetType, throwIfNotFound)?.FindAsset(assetID);

            return result;
        }
        public Object? FindAsset(Type dbAssetType,
                                 string assetName,
                                 UniID dbID,
                                 bool ignoreCase = false,
                                 bool throwIfNotFound = false)
        {
            IAddressablesDatabase db;
            if (!throwIfNotFound)
            {
                try
                {
                    db = GetDatabase(dbAssetType, dbID);
                }
                catch (DatabaseNotFoundException)
                {
                    return null;
                }
            }
            else
                db = GetDatabase(dbAssetType, dbID);

            return db.FindAsset(assetName,
                                ignoreCase,
                                throwIfNotFound);
        }
        public Object? FindAsset(Type dbAssetType,
                                 int assetID,
                                 UniID dbID,
                                 bool throwIfNotFound = false)
        {
            IAddressablesDatabase db;
            if (!throwIfNotFound)
            {
                try
                {
                    db = GetDatabase(dbAssetType, dbID);
                }
                catch (DatabaseNotFoundException)
                {
                    return null;
                }
            }
            else
                db = GetDatabase(dbAssetType, dbID);

            return db.FindAsset(assetID, throwIfNotFound);
        }

        public T? FindAsset<T>(string assetName,
                               Type? dbAssetType = null,
                               bool ignoreCase = false,
                               bool throwIfNotFound = false)
        {
            var result = FindAsset(dbAssetType ?? typeof(T), assetName, ignoreCase, throwIfNotFound);

            return throwIfNotFound
                   ?
                   result.As<T>()
                   :
                   result.AsOrDefault<T>();
        }
        public T? FindAsset<T>(int assetID,
                               Type? dbAssetType = null,
                               bool throwIfNotFound = false)
        {
            var result = FindAsset(dbAssetType ?? typeof(T), assetID, throwIfNotFound);

            return throwIfNotFound
                   ?
                   result.As<T>()
                   :
                   result.AsOrDefault<T>();
        }
        public T? FindAsset<T>(string assetName,
                               UniID dbID,
                               Type? dbAssetType = null,
                               bool ignoreCase = false,
                               bool throwIfNotFound = false)
        {
            var result = FindAsset(dbAssetType ?? typeof(T),
                                   assetName,
                                   dbID,
                                   ignoreCase,
                                   throwIfNotFound);

            return throwIfNotFound
                   ?
                   result.As<T>()
                   :
                   result.AsOrDefault<T>();
        }
        public T? FindAsset<T>(int assetID,
                               UniID dbID,
                               Type? dbAssetType = null,
                               bool throwIfNotFound = false)
        {
            var result = FindAsset(dbAssetType ?? typeof(T),
                                   assetID,
                                   dbID,
                                   throwIfNotFound);

            return throwIfNotFound 
                   ? 
                   result.As<T>()
                   : 
                   result.AsOrDefault<T>();
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
        public Object GetAsset(Type dbAssetType,
                               string? assetName,
                               int assetID,
                               UniID dbID = default)
        {
            return GetAsset(new AssetDatabaseKey(dbAssetType, dbID),
                new AssetKey(assetName, assetID));
        }
        public Object GetAsset(Type dbAssetType,
                               string assetName,
                               UniID dbID = default)
        {
            return GetAsset(new AssetDatabaseKey(dbAssetType, dbID),
                new AssetKey(assetName));
        }
        public Object GetAsset(Type dbAssetType,
                               int assetID,
                               UniID dbID = default)
        {
            return GetAsset(new AssetDatabaseKey(dbAssetType, dbID),
                new AssetKey(assetID));
        }
        public T GetAsset<T>(AssetDatabaseKey dbKey, AssetKey assetkey) => GetAsset(dbKey, assetkey).As<T>();
        public T GetAsset<T>(string? assetName,
                             int assetID,
                             Type? dbAssetType = null,
                             UniID dbID = default)
        {
            return GetAsset<T>(new AssetDatabaseKey(dbAssetType ?? typeof(T), dbID),
                new AssetKey(assetName, assetID));
        }
        public T GetAsset<T>(string assetName,
                             Type? dbAssetType = null,
                             UniID dbID = default)
        {
            return GetAsset<T>(new AssetDatabaseKey(dbAssetType ?? typeof(T), dbID),
                new AssetKey(assetName));
        }
        public T GetAsset<T>(int assetID,
                             Type? dbAssetType = null,
                             UniID dbID = default)
        {
            return GetAsset<T>(new AssetDatabaseKey(dbAssetType ?? typeof(T), dbID),
                new AssetKey(assetID));
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
    }
}
