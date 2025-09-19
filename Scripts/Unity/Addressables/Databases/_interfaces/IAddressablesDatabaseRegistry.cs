using System;
using Object = UnityEngine.Object;

#nullable enable
namespace CCEnvs.Unity.AddrsAssets.Databases
{
    public interface IAddressablesDatabaseRegistry : IDisposable
    {
        Object GetAsset(AssetRegistryKey key);

        T GetAsset<T>(AssetRegistryKey key);
    }
}
