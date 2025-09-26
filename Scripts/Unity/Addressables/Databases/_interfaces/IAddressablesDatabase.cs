using Cysharp.Threading.Tasks;
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
        Type AssetType { get; }
        Func<Object, AssetKey>? KeyFactory { get; set; }
        Func<Object, object?>? IDFactory { get; set; }
        IEnumerable<AssetKey> Keys { get; }
        IEnumerable<Object> Values { get; }
        object this[AssetKey key] { get; }

        void AddAsset(Object asset);

        void AddAssets(IEnumerable<Object> assets);

        UniTask LoadAssetsAsync(AssetLabels assetLabels);

        AssetKey? FindAssetKey(string assetName, bool ignoreCase = false, bool throwIfNotFound = false);
        AssetKey? FindAssetKey(object assetID, bool throwIfNotFound = false);

        Object? FindAsset(string assetName, bool ignoreCase = false, bool throwIfNotFound = false);
        T? FindAsset<T>(string assetName, bool ignoreCase = false, bool throwIfNotFound = false);
        Object? FindAsset(object assetID, bool throwIfNotFound = false);
        T? FindAsset<T>(object assetID, bool throwIfNotFound = false);

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

        new TAsset? FindAsset(string assetName, bool ignoreCase = false);
        new TAsset? FindAsset(object assetID);

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

        Object? IAddressablesDatabase.FindAsset(string assetName, bool ignoreCase = false)
        {
            return FindAsset(assetName, ignoreCase);
        }
        Object? IAddressablesDatabase.FindAsset(object assetID) => FindAsset(assetID);

        Object IAddressablesDatabase.GetAsset(AssetKey key) => GetAsset(key);

        UniTask<IAddressablesDatabase<TNew>> ConvertAsync<TNew>(
            ConverterAsync<TAsset, TNew> dbItemConverter,
            ConverterAsync<TAsset, TNew> dbConverter,
            bool disposePreviousDb)
            where TNew : Object;
    }
}
