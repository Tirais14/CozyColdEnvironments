using CCEnvs.Diagnostics;
using CCEnvs.Linq;
using CCEnvs.Reflection;
using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Linq;
using LinqAF;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine.ResourceManagement.AsyncOperations;
using Object = UnityEngine.Object;

#nullable enable
#pragma warning disable S3881
namespace CCEnvs.Unity.AddrsAssets.Databases
{
    public class AddressablesDatabase<TAsset> : IAddressablesDatabase<TAsset>
        where TAsset : Object
    {
        private readonly System.Diagnostics.Stopwatch stopwatch = new();
        private readonly Dictionary<AssetKey, TAsset> db;
        private readonly List<AsyncOperationHandle<IList<TAsset>>> loadHandles = new(0);
        private bool disposedValue;

        public event Action<ILoadable> OnStartLoading = null!;
        public event Action<ILoadable> OnLoaded = null!;

        public IEnumerable<AssetKey> Keys => db.Keys;
        public IEnumerable<TAsset> Values => db.Values;
        public int Count => db.Count;
        public bool IsLoaded => Count > 0;
        public Type AssetType { get; } = typeof(TAsset);
        public Func<Object, AssetKey>? KeyFactory { get; set; }
        public Func<Object, object?>? IDFactory { get; set; }
        public TAsset this[AssetKey key] => db[key];

        IEnumerable<Object> IAddressablesDatabase.Values => db.Values;

        public AddressablesDatabase(int capacity)
        {
            db = new(capacity);
            BindEvents();
        }

        public AddressablesDatabase() : this(capacity: 4)
        {
        }

        public AddressablesDatabase(IEnumerable<KeyValuePair<AssetKey, TAsset>> values)
            :
            this()
        {
            db = new Dictionary<AssetKey, TAsset>(values);
        }

        public void AddAsset(TAsset asset)
        {
            CC.Validate.ArgumentNull(asset, nameof(asset));

            try
            {
                AssetKey key;
                if (KeyFactory is not null)
                    key = KeyFactory(asset);
                else if (IDFactory is not null)
                    key = new AssetKey(asset.name, IDFactory(asset));
                else
                    key = new AssetKey(asset);

                db.Add(key, asset);
                CCDebug.PrintLog($"Asset {asset.GetType().GetFullName()} loaded. {nameof(AssetKey)}: {key}", this);
            }
            catch (Exception ex)
            {
                this.PrintException(ex);
            }
        }

        public void AddAssets(IEnumerable<TAsset> assets)
        {
            CC.Validate.ArgumentNull(assets, nameof(assets));

            try
            {
                assets.ForEach(AddAsset);
            }
            catch (Exception ex)
            {
                CCDebug.PrintException(ex);
            }
        }

        public async UniTask LoadAssetsAsync(AssetLabels assetLabels)
        {
            CC.Validate.Argument(assetLabels,
                                 nameof(assetLabels),
                                 assetLabels.IsNotDefault());

            try
            {
                OnStartLoading(this);

                var handle = await AddressableLoader.LoadAssetsByLabelsAsync<TAsset>(
                    assetLabels.ToArray(),
                    callback: AddAsset);

                loadHandles.Add(handle);
            }
            catch (Exception ex)
            {
                CCDebug.PrintException(ex);
            }
            finally
            {
                OnLoaded(this);
            }
            
            TrimExcessOnLoaded().Timeout(TimeSpan.FromMinutes(3), DelayType.UnscaledDeltaTime)
                                .Forget(ex => CCDebug.PrintException(ex));
        }

        public AssetKey? FindAssetKey(string assetName,
                                      bool ignoreCase = false,
                                      bool throwIfNotFound = false)
        {
            CC.Validate.StringArgument(assetName, nameof(assetName));

            assetName = ignoreCase ? assetName.ToLower() : assetName;

            foreach (var key in db.Keys)
            {
                if (key.AssetName is not null
                    &&
                    (ignoreCase ? key.AssetName.ToLower() : key.AssetName).StartsWith(assetName)
                    )
                    return key;
            }

            if (throwIfNotFound)
                throw new AssetNotFoundException(assetName, ignoreCase);

            return null;
        }
        public AssetKey? FindAssetKey(object assetID, bool throwIfNotFound = false)
        {
            CC.Validate.ArgumentNull(assetID, nameof(assetID));

            foreach (var key in db.Keys)
            {
                if (key.AssetID is not null && assetID.Equals(assetID))
                    return key;
            }

            if (throwIfNotFound)
                throw new AssetNotFoundException(assetID);

            return null;
        }

        public TAsset? FindAsset(string assetName,
                                 bool ignoreCase = false,
                                 bool throwIfNotFound = false)
        {
            CC.Validate.StringArgument(assetName, nameof(assetName));

            AssetKey? key = FindAssetKey(assetName, ignoreCase, throwIfNotFound);

            if (key is null)
                return null;

            return GetAsset(key.Value);
        }
        public T? FindAsset<T>(string assetName,
                               bool ignoreCase = false,
                               bool throwIfNotFound = false)
        {
            return throwIfNotFound
                   ?
                   FindAsset(assetName, ignoreCase, throwIfNotFound).As<T>()
                   :
                   FindAsset(assetName, ignoreCase, throwIfNotFound).AsOrDefault<T>();
        }
        public TAsset? FindAsset(object assetID, bool throwIfNotFound = false)
        {
            CC.Validate.ArgumentNull(assetID, nameof(assetID));

            AssetKey? key = FindAssetKey(assetID, throwIfNotFound);

            if (key is null)
                return null;

            return GetAsset(key.Value);
        }
        public T? FindAsset<T>(object assetID, bool throwIfNotFound = false)
        {
            return throwIfNotFound
                   ?
                   FindAsset(assetID, throwIfNotFound).As<T>()
                   :
                   FindAsset(assetID, throwIfNotFound).AsOrDefault<T>();
        }

        public TAsset GetAsset(AssetKey key)
        {
            CC.Validate.ArgumentNull(key, nameof(key));

            if (!db.TryGetValue(key, out TAsset asset))
                throw new AssetNotFoundException(key);

            return asset;
        }
        public T GetAsset<T>(AssetKey key)
        {
            return GetAsset(key).As<T>();
        }

        public async UniTask<IAddressablesDatabase<TNew>> ConvertAsync<TNew>(
            ConverterAsync<TAsset, TNew> dbItemConverter,
            ConverterAsync<TAsset, TNew> dbConverter,
            bool disposePreviousDb)
            where TNew : Object
        {
            CC.Validate.ArgumentNull(dbConverter, nameof(dbConverter));
            CC.Validate.ArgumentNull(dbItemConverter, nameof(dbItemConverter));

            var converesationTasks = db.Values.Select(x => dbItemConverter(x)).ToArray();
            var newDb = new AddressablesDatabase<TNew>();
            var converted = await converesationTasks.ToUniTaskAsyncEnumerable()
                                                    .SelectAwait(async task => await task)
                                                    .ToArrayAsync();

            if (disposePreviousDb)
                Dispose();

            newDb.AddAssets(converted);

            return newDb;
        }

        public bool ContainsKey(AssetKey key) => db.ContainsKey(key);

        public void TrimExcess() => db.TrimExcess();

        public void Dispose() => Dispose(disposing: true);

        public IEnumerator<KeyValuePair<AssetKey, TAsset>> GetEnumerator() => db.GetEnumerator();

        protected virtual void Dispose(bool disposing)
        {
            if (disposedValue)
                return;

            if (disposing)
            {
                db.Clear();
                db.TrimExcess();

                loadHandles.ForEach(x => x.Release());
                loadHandles.Clear();
                loadHandles.TrimExcess();
            }

            disposedValue = true;
        }

        private void BindEvents()
        {
            OnStartLoading += (x) =>
            {
                CCDebug.PrintLog("Loading started.", x);
                stopwatch.Start();
            };

            OnLoaded += (x) =>
            {
                stopwatch.Stop();
                CCDebug.PrintLog($"Loading finished in {stopwatch.Elapsed.Seconds} seconds.", x);
                stopwatch.Reset();
            };
        }

        private async UniTask TrimExcessOnLoaded()
        {
            await UniTask.WaitUntil(() => loadHandles.All(handle => handle.IsDone));

            db.TrimExcess();
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        bool IReadOnlyDictionary<AssetKey, TAsset>.TryGetValue(AssetKey key, out TAsset value)
        {
            return db.TryGetValue(key, out value);
        }
    }
}
