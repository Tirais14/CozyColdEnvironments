using System;
using System.Collections.Generic;
using Object = UnityEngine.Object;

#nullable enable
namespace CCEnvs.Unity.AddrsAssets.Databases
{
    public interface IAddressablesDatabaseRegistry 
        : IReadOnlyDictionary<AssetDatabaseKey, IAddressablesDatabase>,
        IDisposable,
        ILoadable
    {
        void RegisterDatabase(AssetDatabaseKey key, IAddressablesDatabase database);

        bool UnregisterDatabase(AssetDatabaseKey key);

        IAddressablesDatabase GetDatabase(AssetDatabaseKey key);
        IAddressablesDatabase GetDatabase(Type assetType, object? uniqueIndentifier = null);
        T GetDatabase<T>(AssetDatabaseKey key)
            where T : IAddressablesDatabase;
        T GetDatabase<T>(Type dbAssetType, object? uniqueIndentifier = null) 
            where T : IAddressablesDatabase;

        Object GetAsset(AssetDatabaseKey dbKey, AssetKey assetkey);
        Object GetAsset(Type dbAssetType,
                        string? assetName,
                        int? assetID,
                        object? dbUniqueIdentifier = null,
                        object? assetUniqueIdentifier = null);
        Object GetAsset(Type dbAssetType,
                        string assetName,
                        object? dbUniqueIdentifier = null,
                        object? assetUniqueIdentifier = null);
        Object GetAsset(Type dbAssetType,
                        int assetID,
                        object? dbUniqueIdentifier = null,
                        object? assetUniqueIdentifier = null);
        T GetAsset<T>(AssetDatabaseKey dbKey, AssetKey assetkey);
        /// <param name="dbAssetType"> if null would be used <see cref="{T}"/></param>
        /// <param name="uniqueIndentifier">Any object which used as ID</param>
        T GetAsset<T>(string? assetName,
                      int? assetID,
                      Type? dbAssetType = null,
                      object? dbUniqueIdentifier = null,
                      object? assetUniqueIdentifier = null);
        /// <param name="dbAssetType"> if null would be used <see cref="{T}"/></param>
        /// <param name="uniqueIndentifier">Any object which used as ID</param>
        T GetAsset<T>(string assetName,
                      Type? dbAssetType = null,
                      object? dbUniqueIdentifier = null,
                      object? assetUniqueIdentifier = null);
        /// <param name="dbAssetType"> if null would be used <see cref="{T}"/></param>
        /// <param name="uniqueIndentifier">Any object which used as ID</param>
        T GetAsset<T>(int assetID,
                      Type? dbAssetType = null,
                      object? dbUniqueIdentifier = null,
                      object? assetUniqueIdentifier = null);
    }
}
