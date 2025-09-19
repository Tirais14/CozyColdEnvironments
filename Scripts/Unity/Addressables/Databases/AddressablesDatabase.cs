using CCEnvs.Diagnostics;
using CCEnvs.Reflection;
using CCEnvs.Unity.AddrsAssets.Databases;
using Cysharp.Threading.Tasks;
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

        public async UniTask LoadAssets(AssetLabels assetLabels,
                                        Func<Object, object?>? getUniqueIndentifier = null)
        {
            CC.Validate.Argument(assetLabels,
                                 nameof(assetLabels),
                                 assetLabels.IsNotDefault());

            try
            {
                loadHandle = await AddressableLoader.LoadAssetsByTagsAsync<T>(
                    (x) => AddAsset(x, getUniqueIndentifier),
                    assetLabels.ToArray());
            }
            catch (Exception ex)
            {
                CCDebug.PrintException(ex);
            }

            TrimExcessAsync().Timeout(TimeSpan.FromMinutes(20), DelayType.UnscaledDeltaTime)
                             .Forget(ex => CCDebug.PrintException(ex));
        }

        public T GetAsset(AssetKey key)
        {
            CC.Validate.ArgumentNull(key, nameof(key));

            return db[key];
        }

        public IEnumerator<T> GetEnumerator() => db.Values.GetEnumerator();

        public void Dispose() => Dispose(disposing: true);

        protected virtual void Dispose(bool disposing)
        {
            if (disposedValue)
                return;

            if (disposing)
                loadHandle.Release();

            disposedValue = true;
        }

        private async UniTask TrimExcessAsync()
        {
            await UniTask.WaitUntil(() => loadHandle.IsDone);

            db.TrimExcess();
        }

        private void AddAsset(T asset, Func<Object, object?>? getUniqueIndentifier)
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

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
