using CCEnvs.Language;
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
        DatabaseQuery Query { get; }
        DatabaseQuery Q { get; }

        void RegisterDatabase(IAddressablesDatabase database);

        bool UnregisterDatabase(AssetDatabaseKey key);

        Ghost<IAddressablesDatabase> FindDatabase(AssetDatabaseKey key);
        Ghost<T> FindDatabase<T>(AssetDatabaseKey key)
            where T : IAddressablesDatabase;

        IAddressablesDatabase GetDatabase(AssetDatabaseKey key);
        T GetDatabase<T>(AssetDatabaseKey key)
            where T : IAddressablesDatabase;

        Ghost<Object?> FindAsset(AssetDatabaseKey dbKey, AssetKey key);
        Ghost<T?> FindAsset<T>(AssetDatabaseKey dbKey, AssetKey key);

        Object GetAsset(AssetDatabaseKey dbKey, AssetKey assetkey);
        T GetAsset<T>(AssetDatabaseKey dbKey, AssetKey assetkey);
    }
}
