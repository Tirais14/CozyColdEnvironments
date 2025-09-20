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
    public class AddressablesDatabase<T> : IAddressablesDatabase<T>
        where T : Object
    {
        private readonly Dictionary<AssetKey, T> db = new();
        private bool disposedValue;
        private AsyncOperationHandle<IList<T>> loadHandle;

        public Type AssetType { get; } = typeof(T);
        public IEnumerable<AssetKey> Keys => db.Keys;
        public IEnumerable<T> Values => db.Values;
        public int Count => db.Count;
        public T this[AssetKey key] => db[key];

        public void AddAssets(IEnumerable<KeyValuePair<AssetKey, Object>> items)
        {
            CC.Validate.ArgumentNull(items, nameof(items));

            try
            {
                IEnumerable<KeyValuePair<AssetKey, T>> casted = items.SelectValue(
                    x => x.As<T>()).AsEnumerable();

                db.AddRange(casted);
            }
            catch (Exception ex)
            {
                CCDebug.PrintException(ex);
            }
        }

        public async UniTask LoadAssets(AssetLabels assetLabels,
                                        Func<Object, object?>? getUniqueIndentifier = null)
        {
            CC.Validate.Argument(loadHandle,
                                 nameof(loadHandle),
                                 loadHandle.IsDefault());
            CC.Validate.Argument(assetLabels,
                                 nameof(assetLabels),
                                 assetLabels.IsNotDefault());

            try
            {
                loadHandle = await AddressableLoader.LoadAssetsByTagsAsync<T>(
                    (x) => OnAssetLoaded(x, getUniqueIndentifier),
                    assetLabels.ToArray());
            }
            catch (Exception ex)
            {
                CCDebug.PrintException(ex);
            }

            TrimExcessOnLoaded().Timeout(TimeSpan.FromMinutes(3), DelayType.UnscaledDeltaTime)
                                .Forget(ex => CCDebug.PrintException(ex));
        }

        public T GetAsset(AssetKey key)
        {
            CC.Validate.ArgumentNull(key, nameof(key));

            return db[key];
        }

        public async UniTask<IAddressablesDatabase<TNew>> ConvertTo<TNew>(
            DbItemConverter<T, TNew> dbItemConverter,
            DbConverter<T, TNew> dbConverter,
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

        public IEnumerator<KeyValuePair<AssetKey, T>> GetEnumerator() => db.GetEnumerator();

        protected virtual void Dispose(bool disposing)
        {
            if (disposedValue)
                return;

            if (disposing)
                loadHandle.Release();

            disposedValue = true;
        }

        protected virtual void OnAssetLoaded(T asset, Func<Object, object?>? getUniqueIndentifier)
        {
            object? uniqueIndentifier = null;

            try
            {
                uniqueIndentifier = getUniqueIndentifier?.Invoke(asset);
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
            await UniTask.WaitUntil(() => loadHandle.IsDone);

            db.TrimExcess();
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        bool IReadOnlyDictionary<AssetKey, T>.TryGetValue(AssetKey key, out T value) => db.TryGetValue(key, out value);
    }
}
