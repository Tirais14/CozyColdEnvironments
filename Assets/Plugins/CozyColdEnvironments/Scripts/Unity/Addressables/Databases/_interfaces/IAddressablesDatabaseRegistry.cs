using CCEnvs;
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
        IAddressablesDatabase this[Type dbAssetType, UniID dbID] { get; }
        IAddressablesDatabase this[Type dbAssetType, int num0] { get; }
        IAddressablesDatabase this[Type dbAssetType, int num0, int num1] { get; }
        IAddressablesDatabase this[Type dbAssetType, string str0] { get; }
        IAddressablesDatabase this[Type dbAssetType, string str0, string str1] { get; }
        IAddressablesDatabase this[Type dbAssetType, int num0, string str0] { get; }
        IAddressablesDatabase this[Type dbAssetType, int num0, int num1, string str0, string str1] { get; }
        IAddressablesDatabase this[Type dbAssetType, Enum value] { get; }
        IAddressablesDatabase this[Type dbAssetType, Enum value, Enum value1] { get; }

        void RegisterDatabase(IAddressablesDatabase database);

        bool UnregisterDatabase(AssetDatabaseKey key);

        IAddressablesDatabase? FindDatabase(Type assetType,
                                            bool throwIfNotFound = false);
        IAddressablesDatabase? FindDatabase(Type assetType,
                                            UniID dbID,
                                            bool throwIfNotFound = false);

        T? FindDatabase<T>(Type assetType,
                           bool throwIfNotFound = false)
            where T : IAddressablesDatabase;
        T? FindDatabase<T>(Type assetType,
                           UniID dbID,
                           bool throwIfNotFound = false)
            where T : IAddressablesDatabase;

        AssetKey? FindAssetKey(Type dbAssetType,
                               string assetName,
                               bool ignoreCase = false,
                               bool throwIfNotFound = false);
        AssetKey? FindAssetKey(Type dbAssetType,
                               int assetID,
                               bool throwIfNotFound = false);

        Object? FindAsset(Type dbAssetType,
                          string assetName,
                          bool ignoreCase = false,
                          bool throwIfNotFound = false);
        Object? FindAsset(Type dbAssetType,
                          int assetID,
                          bool throwIfNotFound = false);
        Object? FindAsset(Type dbAssetType,
                          string assetName,
                          UniID dbID,
                          bool ignoreCase = false,
                          bool throwIfNotFound = false);
        Object? FindAsset(Type dbAssetType,
                          int assetID,
                          UniID dbID,
                          bool throwIfNotFound = false);

        T? FindAsset<T>(string assetName,
                        Type? dbAssetType = null,
                        bool ignoreCase = false,
                        bool throwIfNotFound = false);
        T? FindAsset<T>(int assetID,
                        Type? dbAssetType = null,
                        bool throwIfNotFound = false);
        T? FindAsset<T>(string assetName,
                        UniID dbID,
                        Type? dbAssetType = null,
                        bool ignoreCase = false,
                        bool throwIfNotFound = false);
        T? FindAsset<T>(int assetID,
                        UniID dbID,
                        Type? dbAssetType = null,
                        bool throwIfNotFound = false);

        IAddressablesDatabase GetDatabase(AssetDatabaseKey key);
        IAddressablesDatabase GetDatabase(Type assetType, UniID dbID = default);
        T GetDatabase<T>(AssetDatabaseKey key)
            where T : IAddressablesDatabase;
        T GetDatabase<T>(Type dbAssetType, UniID dbID = default) 
            where T : IAddressablesDatabase;

        Object GetAsset(AssetDatabaseKey dbKey, AssetKey assetkey);
        Object GetAsset(Type dbAssetType,
                        string? assetName,
                        int assetID,
                        UniID dbID = default);
        Object GetAsset(Type dbAssetType,
                        string assetName,
                        UniID dbID = default);
        Object GetAsset(Type dbAssetType,
                        int assetID,
                        UniID dbID = default);
        T GetAsset<T>(AssetDatabaseKey dbKey, AssetKey assetkey);
        /// <param name="dbAssetType"> if null would be used <see cref="{T}"/></param>
        T GetAsset<T>(string? assetName,
                      int assetID,
                      Type? dbAssetType = null,
                      UniID dbID = default);
        /// <param name="dbAssetType"> if null would be used <see cref="{T}"/></param>
        T GetAsset<T>(string assetName,
                      Type? dbAssetType = null,
                      UniID dbID = default);
        /// <param name="dbAssetType"> if null would be used <see cref="{T}"/></param>
        T GetAsset<T>(int assetID,
                      Type? dbAssetType = null,
                      UniID dbID = default);
    }
}
