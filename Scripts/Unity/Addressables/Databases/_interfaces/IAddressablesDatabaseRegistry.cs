using System;
using System.Collections.Generic;
using Object = UnityEngine.Object;

#nullable enable
namespace CCEnvs.Unity.AddrsAssets.Databases
{
    public interface IAddressablesDatabaseRegistry 
        : IReadOnlyDictionary<AssetRegistryKey, IAddressablesDatabase>,
        IDisposable
    {
        void RegisterDatabase(AssetRegistryKey key, IAddressablesDatabase database);

        bool UnregisterDatabase(AssetRegistryKey key);

        IAddressablesDatabase GetDatabase(AssetRegistryKey key);
        IAddressablesDatabase GetDatabase(Type assetType, object? uniqueIndentifier = null);
        T GetDatabase<T>(AssetRegistryKey key)
            where T : IAddressablesDatabase;
        T GetDatabase<T>(Type dbAssetType, object? uniqueIndentifier = null) 
            where T : IAddressablesDatabase;

        Object GetAsset(AssetRegistryKey key);
        Object GetAsset(Type dbAssetType,
                        string? assetName,
                        int assetID,
                        object? uniqueIndentifier = null);
        Object GetAsset(Type dbAssetType,
                        string assetName,
                        object? uniqueIndentifier = null);
        Object GetAsset(Type dbAssetType,
                        int assetID,
                        object? uniqueIndentifier = null);
        T GetAsset<T>(AssetRegistryKey key);
        /// <param name="dbAssetType"> if null would be used <see cref="{T}"/></param>
        /// <param name="uniqueIndentifier">Any object which used as ID</param>
        T GetAsset<T>(Type? dbAssetType,
                      string? assetName,
                      int assetID,
                      object? uniqueIndentifier = null);
        /// <param name="dbAssetType"> if null would be used <see cref="{T}"/></param>
        /// <param name="uniqueIndentifier">Any object which used as ID</param>
        T GetAsset<T>(Type? dbAssetType,
                      string assetName,
                      object? uniqueIndentifier = null);
        /// <param name="dbAssetType"> if null would be used <see cref="{T}"/></param>
        /// <param name="uniqueIndentifier">Any object which used as ID</param>
        T GetAsset<T>(Type? dbAssetType,
                      int assetID,
                      object? uniqueIndentifier = null);
    }
}
