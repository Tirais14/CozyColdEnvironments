using CCEnvs.Diagnostics;
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
    public class AddressablesDatabaseT<T> : IAddressableDatabase<T>
        where T : Object
    {
        private readonly Dictionary<AssetKey, T> db = new();
        private bool disposedValue;
        private AsyncOperationHandle<IList<T>> loadHandle;

        public async UniTask LoadAssets(AssetLabels assetLabels,
                                        Func<T, object?>? getUniqueIndentifier = null)
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

        private void AddAsset(T asset, Func<T, object?>? getUniqueIndentifier)
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

            db.Add(new AssetKey(asset, uniqueIndentifier), asset);
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
