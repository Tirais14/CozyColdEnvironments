using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ZLinq;
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

        AssetKey? FindAssetKey(string assetName,
                               bool ignoreCase = false,
                               bool throwIfNotFound = false);
        AssetKey? FindAssetKey(object assetID, bool throwIfNotFound = false);

        Object? FindAsset(string assetName,
                          bool ignoreCase = false,
                          bool throwIfNotFound = false);
        T? FindAsset<T>(string assetName,
                        bool ignoreCase = false,
                        bool throwIfNotFound = false);
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

        new TAsset? FindAsset(string assetName,
                              bool ignoreCase = false,
                              bool throwIfNotFound = false);
        new TAsset? FindAsset(object assetID, bool throwIfNotFound = false);

        new TAsset GetAsset(AssetKey key);

        void IAddressablesDatabase.AddAsset(Object asset)
        {
            CC.Guard.NullArgument(asset, nameof(asset));

            AddAsset(asset.As<TAsset>());
        }

        void IAddressablesDatabase.AddAssets(IEnumerable<Object> assets)
        {
            CC.Guard.NullArgument(assets, nameof(assets));

            AddAssets(assets.Select(x => x.As<TAsset>()));
        }

        Object? IAddressablesDatabase.FindAsset(string assetName,
                                                bool ignoreCase,
                                                bool throwIfNotFound)
        {
            return FindAsset(assetName, ignoreCase, throwIfNotFound);
        }
        Object? IAddressablesDatabase.FindAsset(object assetID, bool throwIfNotFound)
        {
            return FindAsset(assetID, throwIfNotFound);
        }

        Object IAddressablesDatabase.GetAsset(AssetKey key) => GetAsset(key);

        UniTask<IAddressablesDatabase<TNew>> ConvertAsync<TNew>(
            ConverterAsync<TAsset, TNew> dbItemConverter,
            ConverterAsync<TAsset, TNew> dbConverter,
            bool disposePreviousDb)
            where TNew : Object;
    }
}
