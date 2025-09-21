using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using Object = UnityEngine.Object;

#nullable enable
namespace CCEnvs.Unity.AddrsAssets.Databases
{
    public delegate UniTask<KeyValuePair<AssetKey, TNew>> DbItemConverter<TPrevious, TNew>(TPrevious prevItem)
        where TPrevious : Object
        where TNew : Object;
    public delegate UniTask<IAddressablesDatabase<TNew>> DbConverter<TPrevious, TNew>(IAddressablesDatabase<TPrevious> prevDb)
        where TPrevious : Object
        where TNew : Object;
    public delegate object? UniqueIndentifierGetter(Object obj);

    public interface IAddressablesDatabase
        : IDisposable,
        IEnumerable,
        ILoadable
    {
        object this[AssetKey key] { get; }

        void AddAssets(IEnumerable<KeyValuePair<AssetKey, Object>> items);

        UniTask LoadAssetsAsync(AssetLabels assetLabels,
                                UniqueIndentifierGetter? uniqueIndentifierGetter = null);

        Type AssetType { get; }

        Object GetAsset(AssetKey key);

        T GetAsset<T>(AssetKey key);
    }
    public interface IAddressablesDatabase<TAsset>
        : IAddressablesDatabase,
        IReadOnlyDictionary<AssetKey, TAsset>, 
        ITrimmable

        where TAsset : Object
    {
        object IAddressablesDatabase.this[AssetKey key] => this[key];

        new TAsset GetAsset(AssetKey key);

        Object IAddressablesDatabase.GetAsset(AssetKey key) => GetAsset(key);

        UniTask<IAddressablesDatabase<TNew>> ConvertAsync<TNew>(
            DbItemConverter<TAsset, TNew> dbItemConverter,
            DbConverter<TAsset, TNew> dbConverter,
            bool disposePreviousDb)
            where TNew : Object;
    }
}
