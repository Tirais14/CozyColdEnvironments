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

    public interface IAddressablesDatabase
        : IDisposable,
        IEnumerable
    {
        object this[AssetKey key] { get; }

        void AddAssets(IEnumerable<KeyValuePair<AssetKey, Object>> items);

        UniTask LoadAssets(AssetLabels assetLabels,
                           Func<Object, object?>? getUniqueIndentifier = null);

        Type AssetType { get; }

        Object GetAsset(AssetKey key);
    }
    public interface IAddressablesDatabase<T>
        : IAddressablesDatabase,
        IReadOnlyDictionary<AssetKey, T>, 
        ITrimmable

        where T : Object
    {
        object IAddressablesDatabase.this[AssetKey key] => this[key];

        new T GetAsset(AssetKey key);

        Object IAddressablesDatabase.GetAsset(AssetKey key) => GetAsset(key);

        UniTask<IAddressablesDatabase<TNew>> ConvertTo<TNew>(
            DbItemConverter<T, TNew> dbItemConverter,
            DbConverter<T, TNew> dbConverter,
            bool disposePreviousDb)
            where TNew : Object;
    }
}
