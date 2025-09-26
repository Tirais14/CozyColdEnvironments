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
                        object? assetID,
                        object? dbID = null);
        Object GetAsset(Type dbAssetType,
                        string assetName,
                        object? dbID = null);
        Object GetAsset(Type dbAssetType,
                        object assetID,
                        object? dbID = null);
        T GetAsset<T>(AssetDatabaseKey dbKey, AssetKey assetkey);
        /// <param name="dbAssetType"> if null would be used <see cref="{T}"/></param>
        /// <param name="uniqueIndentifier">Any object which used as ID</param>
        T GetAsset<T>(string? assetName,
                      object? assetID,
                      Type? dbAssetType = null,
                      object? dbID = null);
        /// <param name="dbAssetType"> if null would be used <see cref="{T}"/></param>
        /// <param name="uniqueIndentifier">Any object which used as ID</param>
        T GetAsset<T>(string assetName,
                      Type? dbAssetType = null,
                      object? dbID = null);
        /// <param name="dbAssetType"> if null would be used <see cref="{T}"/></param>
        /// <param name="uniqueIndentifier">Any object which used as ID</param>
        T GetAsset<T>(object assetID,
                      Type? dbAssetType = null,
                      object? dbID = null);
    }
}
