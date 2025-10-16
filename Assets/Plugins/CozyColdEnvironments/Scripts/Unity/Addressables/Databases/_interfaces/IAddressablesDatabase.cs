using CCEnv;
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
        ILoadable,
        IIDMarked<UniID>
    {
        Type AssetType { get; }
        Func<Object, AssetKey>? KeyFactory { get; set; }
        Func<Object, int>? IDFactory { get; set; }
        Func<string, string>? AssetNameProcessor { get; set; }
        IEnumerable<AssetKey> Keys { get; }
        IEnumerable<Object> Values { get; }
        Object this[AssetKey key] { get; }
        Object this[string assetName, int assetID] { get; }
        Object this[string assetName] { get; }
        Object this[int assetID] { get; }

        void AddAsset(Object asset);

        void AddAssets(IEnumerable<Object> assets);

        UniTask LoadAssetsAsync(AssetLabels assetLabels);

        IAddressablesDatabase CutByType(Type assetType);
        /// <summary>
        /// Creates new <see cref="IAddressablesDatabase"/> with specified type, and delete it items from current
        /// </summary>
        IAddressablesDatabase<T> CutByType<T>() where T : Object;

        /// <summary>
        /// Do <see cref="CutByType{T}"/> for all types in database
        /// </summary>
        IAddressablesDatabase[] CutByTypes();

        AssetKey? FindAssetKey(string assetName,
                               bool ignoreCase = false,
                               bool throwIfNotFound = false);
        AssetKey? FindAssetKey(int assetID, bool throwIfNotFound = false);

        Object? FindAsset(string assetName,
                          bool ignoreCase = false,
                          bool throwIfNotFound = false);
        T? FindAsset<T>(string assetName,
                        bool ignoreCase = false,
                        bool throwIfNotFound = false);
        Object? FindAsset(int assetID, bool throwIfNotFound = false);
        T? FindAsset<T>(int assetID, bool throwIfNotFound = false);

        Object GetAsset(AssetKey key);
        Object GetAsset(string assetName);
        Object GetAsset(string assetName, int assetID);
        Object GetAsset(int assetID);
        T GetAsset<T>(AssetKey key);
        T GetAsset<T>(string assetName);
        T GetAsset<T>(string assetName, int assetID);
        T GetAsset<T>(int assetID);
    }
    public interface IAddressablesDatabase<TAsset>
        : IAddressablesDatabase,
        IReadOnlyDictionary<AssetKey, TAsset>,
        ITrimmable

        where TAsset : Object
    {
        new TAsset this[AssetKey key] { get; }
        new TAsset this[string assetName, int assetID] { get; }
        new TAsset this[string assetName] { get; }
        new TAsset this[int assetID] { get; }

        new IEnumerable<AssetKey> Keys { get; }
        new IEnumerable<TAsset> Values { get; }

        Object IAddressablesDatabase.this[AssetKey key] => this[key];
        Object IAddressablesDatabase.this[string assetName,int assetID] {
            get => this[assetName, assetID];
        }
        Object IAddressablesDatabase.this[string assetName] {
            get => this[assetName];
        }
        Object IAddressablesDatabase.this[int assetID] {
            get => this[assetID];
        }

        IEnumerable<AssetKey> IAddressablesDatabase.Keys => Keys;
        IEnumerable<Object> IAddressablesDatabase.Values => Values;

        UniTask LoadAssetsAsync<TSub>(AssetLabels assetLabels) where TSub : TAsset;
        UniTask LoadAssetsAsync<TAnyAsset>(AssetLabels assetLabels, Func<TAnyAsset, TAsset[]> converter)
            where TAnyAsset : Object;

        void AddAsset(TAsset asset);

        void AddAssets(IEnumerable<TAsset> assets);

        new TAsset? FindAsset(string assetName,
                              bool ignoreCase = false,
                              bool throwIfNotFound = false);
        new TAsset? FindAsset(int assetID, bool throwIfNotFound = false);

        new TAsset GetAsset(AssetKey key);
        new TAsset GetAsset(string assetName);
        new TAsset GetAsset(string assetName, int assetID);
        new TAsset GetAsset(int assetID);

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
        Object? IAddressablesDatabase.FindAsset(int assetID, bool throwIfNotFound)
        {
            return FindAsset(assetID, throwIfNotFound);
        }

        Object IAddressablesDatabase.GetAsset(AssetKey key) => GetAsset(key);
        Object IAddressablesDatabase.GetAsset(string assetName) => GetAsset(assetName);
        Object IAddressablesDatabase.GetAsset(string assetName, int assetID) => GetAsset(assetName, assetID);
        Object IAddressablesDatabase.GetAsset(int assetID) => GetAsset(assetID);

        UniTask<IAddressablesDatabase<TNew>> ConvertAsync<TNew>(
            ConverterAsync<TAsset, TNew> dbItemConverter,
            ConverterAsync<TAsset, TNew> dbConverter,
            bool disposePreviousDb)
            where TNew : Object;
    }
}
