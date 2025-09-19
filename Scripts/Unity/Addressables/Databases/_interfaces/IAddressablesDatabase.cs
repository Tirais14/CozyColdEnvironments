using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using Object = UnityEngine.Object;

#nullable enable
namespace CCEnvs.Unity.AddrsAssets.Databases
{
    public interface IAddressablesDatabase : IDisposable, IEnumerable
    {
        UniTask LoadAssets(AssetLabels assetLabels,
                           Func<Object, object?>? getUniqueIndentifier = null);

        Type AssetType { get; }

        Object GetAsset(AssetKey key);
    }
    public interface IAddressablesDatabase<T> : IAddressablesDatabase, IEnumerable<T>
        where T : Object
    {
        new T GetAsset(AssetKey key);

        Object IAddressablesDatabase.GetAsset(AssetKey key) => GetAsset(key);
    }
}
