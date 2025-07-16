using System;
using System.Threading.Tasks;
using UnityEngine.AddressableAssets;
using UTIRLib.Diagnostics;
using UTIRLib.Extensions;

#nullable enable

namespace UTIRLib.DB
{
    /// <summary>
    /// Contains information about asset
    /// </summary>
    public class AssetDatabaseItem<T> : IAssetDatabaseItem<T>
    {
        protected readonly AddressableAssetInfo info;

        protected T? asset;
        protected Type? assetType;

        public AddressableAssetInfo Info => info;

        /// <summary>
        /// If not loaded - start laoding syncroniously
        /// </summary>
        public T? Asset => asset;

        public Type? AssetType => assetType;
        public bool IsAssetLoaded => asset.IsNotDefault();
        public string AssetName => info.Name;
        public string AssetGUID => info.AssetGUID;
        public string Address => info.Address;

        public AssetDatabaseItem(AddressableAssetInfo addressableAssetInfo) => info = addressableAssetInfo;

        /// <exception cref="Exception"></exception>
        public async Task<T> LoadAssetAsync()
        {
            if (IsAssetLoaded)
            {
                TirLibDebug.Warning($"Asset {asset.GetTypeName()} already loaded.", this);
                return GetAsset()!;
            }

            asset = await Addressables.LoadAssetAsync<T>(Address).Task;
            assetType = asset!.GetType();

            return asset;
        }

        public T LoadAsset()
        {
            Task<T> task = LoadAssetAsync();
            task.RunSynchronously();

            return task.Result;
        }

        public void ReleaseAsset()
        {
            if (IsAssetLoaded)
            {
                Addressables.Release(asset);
            }
            else TirLibDebug.Warning("Trying to release not loaded asset.", this);
        }

        public T? GetAsset()
        {
            TirLibDebug.Error($"Try to access not loaded asset: {AssetName}.", this);

            return asset;
        }
    }
}