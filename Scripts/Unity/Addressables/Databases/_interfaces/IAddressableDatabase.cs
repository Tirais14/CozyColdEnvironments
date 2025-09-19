using CCEnvs.Unity.AddrsAssets;
using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

#nullable enable
namespace CCEnvs.Unity.AddrsAssets.Databases
{
    public interface IAddressableDatabase : IDisposable, IEnumerable
    {
    }
    public interface IAddressableDatabase<T> : IAddressableDatabase, IEnumerable<T>
        where T : Object
    {
        UniTask LoadAssets(AssetLabels assetLabels,
                           Func<T, object?>? getUniqueIndentifier = null);

        T GetAsset(AssetKey key);
    }
}
