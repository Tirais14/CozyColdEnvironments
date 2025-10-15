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
        IAddressablesDatabase this[Type dbAssetType] { get; }
        IAddressablesDatabase this[Type dbAssetType, object dbID] { get; }

        void RegisterDatabase(IAddressablesDatabase database);

        bool UnregisterDatabase(AssetDatabaseKey key);

        IAddressablesDatabase? FindDatabase(Type assetType,
                                            bool throwIfNotFound = false);
        IAddressablesDatabase? FindDatabase(Type assetType,
                                            object? dbID,
                                            bool throwIfNotFound = false);

        T? FindDatabase<T>(Type assetType,
                           bool throwIfNotFound = false)
            where T : IAddressablesDatabase;
        T? FindDatabase<T>(Type assetType,
                           object? dbID,
                           bool throwIfNotFound = false)
            where T : IAddressablesDatabase;

        AssetKey? FindAssetKey(Type dbAssetType,
                               string assetName,
                               bool ignoreCase = false,
                               bool throwIfNotFound = false);
        AssetKey? FindAssetKey(Type dbAssetType,
                               object assetID,
                               bool throwIfNotFound = false);

        Object? FindAsset(Type dbAssetType,
                          string assetName,
                          bool ignoreCase = false,
                          bool throwIfNotFound = false);
        Object? FindAsset(Type dbAssetType,
                          object assetID,
                          bool throwIfNotFound = false);
        Object? FindAsset(Type dbAssetType,
                          string assetName,
                          object? dbID,
                          bool ignoreCase = false,
                          bool throwIfNotFound = false);
        Object? FindAsset(Type dbAssetType,
                          object assetID,
                          object? dbID,
                          bool throwIfNotFound = false);

        T? FindAsset<T>(string assetName,
                        Type? dbAssetType = null,
                        bool ignoreCase = false,
                        bool throwIfNotFound = false);
        T? FindAsset<T>(object assetID,
                        Type? dbAssetType = null,
                        bool throwIfNotFound = false);
        T? FindAsset<T>(string assetName,
                        object? dbID,
                        Type? dbAssetType = null,
                        bool ignoreCase = false,
                        bool throwIfNotFound = false);
        T? FindAsset<T>(object assetID,
                        object? dbID,
                        Type? dbAssetType = null,
                        bool throwIfNotFound = false);

        IAddressablesDatabase GetDatabase(AssetDatabaseKey key);
        IAddressablesDatabase GetDatabase(Type assetType, object? dbID = null);
        T GetDatabase<T>(AssetDatabaseKey key)
            where T : IAddressablesDatabase;
        T GetDatabase<T>(Type dbAssetType, object? dbID = null) 
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
        T GetAsset<T>(string? assetName,
                      object? assetID,
                      Type? dbAssetType = null,
                      object? dbID = null);
        /// <param name="dbAssetType"> if null would be used <see cref="{T}"/></param>
        T GetAsset<T>(string assetName,
                      Type? dbAssetType = null,
                      object? dbID = null);
        /// <param name="dbAssetType"> if null would be used <see cref="{T}"/></param>
        T GetAsset<T>(object assetID,
                      Type? dbAssetType = null,
                      object? dbID = null);
    }
}
