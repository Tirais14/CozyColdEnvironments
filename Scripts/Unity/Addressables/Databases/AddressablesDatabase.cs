using CCEnvs.Collections;
using CCEnvs.Diagnostics;
using CCEnvs.Linq;
using CCEnvs.Reflection;
using CCEnvs.Unity.AddrsAssets.Databases;
using Cysharp.Threading.Tasks;
using LinqAF;
using Cysharp.Threading.Tasks.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.ResourceManagement.AsyncOperations;
using Object = UnityEngine.Object;

#nullable enable
#pragma warning disable S3881
namespace CCEnvs.Unity.AddrsAssets
{
    public class AddressablesDatabase<TAsset> : IAddressablesDatabase<TAsset>
        where TAsset : Object
    {
        private readonly Dictionary<AssetKey, TAsset> db;
        private readonly List<AsyncOperationHandle<IList<TAsset>>> loadHandles = new(0);
        private bool disposedValue;

        public Type AssetType { get; } = typeof(TAsset);
        public IEnumerable<AssetKey> Keys => db.Keys;
        public IEnumerable<TAsset> Values => db.Values;
        public int Count => db.Count;
        public TAsset this[AssetKey key] => db[key];

        public AddressablesDatabase(int capacity) => db = new(capacity);

        public AddressablesDatabase() : this(capacity: 0)
        {
        }

        public AddressablesDatabase(IEnumerable<KeyValuePair<AssetKey, TAsset>> values)
        {
            db = new Dictionary<AssetKey, TAsset>(values);
        }

        public void AddAssets(IEnumerable<KeyValuePair<AssetKey, Object>> items)
        {
            CC.Validate.ArgumentNull(items, nameof(items));

            try
            {
                IEnumerable<KeyValuePair<AssetKey, TAsset>> casted = items.SelectValue(
                    x => x.As<TAsset>()).AsEnumerable();

                db.AddRange(casted);
            }
            catch (Exception ex)
            {
                CCDebug.PrintException(ex);
            }
        }

        public async UniTask LoadAssetsAsync(AssetLabels assetLabels,
            UniqueIndentifierGetter? uniqueIndentifierGetter = null)
        {
            CC.Validate.Argument(assetLabels,
                                 nameof(assetLabels),
                                 assetLabels.IsNotDefault());

            try
            {
                var handle = await AddressableLoader.LoadAssetsByTagsAsync<TAsset>(
                    (x) => OnAssetLoaded(x, uniqueIndentifierGetter),
                    assetLabels.ToArray());

                loadHandles.Add(handle);
            }
            catch (Exception ex)
            {
                CCDebug.PrintException(ex);
            }

            TrimExcessOnLoaded().Timeout(TimeSpan.FromMinutes(3), DelayType.UnscaledDeltaTime)
                                .Forget(ex => CCDebug.PrintException(ex));
        }

        public TAsset GetAsset(AssetKey key)
        {
            CC.Validate.ArgumentNull(key, nameof(key));

            return db[key];
        }
        public T GetAsset<T>(AssetKey key)
        {
            return GetAsset(key).As<T>();
        }

        public async UniTask<IAddressablesDatabase<TNew>> ConvertAsync<TNew>(
            DbItemConverter<TAsset, TNew> dbItemConverter,
            DbConverter<TAsset, TNew> dbConverter,
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

            newDb.AddAssets(converted.SelectValue(x => (Object)x));

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

        protected virtual void OnAssetLoaded(TAsset asset,
            UniqueIndentifierGetter? uniqueIndentifierGetter)
        {
            object? uniqueIndentifier = null;

            try
            {
                uniqueIndentifier = uniqueIndentifierGetter?.Invoke(asset);
            }
            catch (Exception ex)
            {
                CCDebug.PrintException(ex);
            }

            var assetKey = new AssetKey(asset, uniqueIndentifier);
            db.Add(assetKey, asset);
            CCDebug.PrintLog($"Asset {asset.GetType().GetFullName()} loaded with key = {assetKey}", this);
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
