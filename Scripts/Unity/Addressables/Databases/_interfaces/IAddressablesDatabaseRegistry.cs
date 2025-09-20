using System;
using System.Collections.Generic;
using Object = UnityEngine.Object;

#nullable enable
namespace CCEnvs.Unity.AddrsAssets.Databases
{
    public interface IAddressablesDatabaseRegistry : IEnumerable<IAddressablesDatabase>, IDisposable
    {
        void RegisterDatabase(AssetRegistryKey key, IAddressablesDatabase database);

        bool UnregisterDatabase(AssetRegistryKey key);

        Object GetAsset(AssetRegistryKey key);

        T GetAsset<T>(AssetRegistryKey key);
    }
}
