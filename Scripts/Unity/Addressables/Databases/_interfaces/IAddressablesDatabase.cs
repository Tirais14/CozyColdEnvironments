using CCEnvs.Linq;
using Cysharp.Threading.Tasks;
using DG.Tweening.Core;
using LinqAF;
using System;
using System.Collections;
using System.Collections.Generic;
using Object = UnityEngine.Object;

#nullable enable
namespace CCEnvs.Unity.AddrsAssets.Databases
{
    public interface IAddressablesDatabase
        : IDisposable,
        IEnumerable,
        ILoadable
    {
        Func<Object, AssetKey>? KeyFactory { get; set; }
        Func<Object, object?>? IDFactory { get; set; }
        object this[AssetKey key] { get; }

        void AddAsset(Object asset);

        void AddAssets(IEnumerable<Object> assets);

        UniTask LoadAssetsAsync(AssetLabels assetLabels);

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

        void AddAsset(TAsset asset);

        void AddAssets(IEnumerable<TAsset> assets);

        new TAsset GetAsset(AssetKey key);

        void IAddressablesDatabase.AddAsset(Object asset)
        {
            CC.Validate.ArgumentNull(asset, nameof(asset));

            AddAsset(asset.As<TAsset>());
        }

        void IAddressablesDatabase.AddAssets(IEnumerable<Object> assets)
        {
            CC.Validate.ArgumentNull(assets, nameof(assets));

            AddAssets(assets.Select(x => x.As<TAsset>()).AsEnumerable());
        }

        Object IAddressablesDatabase.GetAsset(AssetKey key) => GetAsset(key);

        UniTask<IAddressablesDatabase<TNew>> ConvertAsync<TNew>(
            ConverterAsync<TAsset, TNew> dbItemConverter,
            ConverterAsync<TAsset, TNew> dbConverter,
            bool disposePreviousDb)
            where TNew : Object;
    }
}
