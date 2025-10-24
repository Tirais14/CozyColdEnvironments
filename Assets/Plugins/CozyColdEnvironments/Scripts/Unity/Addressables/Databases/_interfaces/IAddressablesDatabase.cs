using CCEnvs.Language;
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
        DatabaseQuery Q { get; }

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

        Maybe<Object> FindAsset(AssetKey key);
        Maybe<T> FindAsset<T>(AssetKey key);

        Object GetAsset(AssetKey key);
        T GetAsset<T>(AssetKey key);

        DatabaseQuery Query();
    }
    public interface IAddressablesDatabase<TAsset>
        : IAddressablesDatabase,
        IReadOnlyDictionary<AssetKey, TAsset>,
        ITrimmable

        where TAsset : Object
    {
        new TAsset this[AssetKey key] { get; }

        new IEnumerable<AssetKey> Keys { get; }
        new IEnumerable<TAsset> Values { get; }

        Object IAddressablesDatabase.this[AssetKey key] => this[key];

        IEnumerable<AssetKey> IAddressablesDatabase.Keys => Keys;
        IEnumerable<Object> IAddressablesDatabase.Values => Values;

        UniTask LoadAssetsAsync<TSub>(AssetLabels assetLabels) where TSub : TAsset;
        UniTask LoadAssetsAsync<TAnyAsset>(AssetLabels assetLabels, Func<TAnyAsset, TAsset[]> converter)
            where TAnyAsset : Object;

        void AddAsset(TAsset asset);

        void AddAssets(IEnumerable<TAsset> assets);

        new Maybe<TAsset> FindAsset(AssetKey key);

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

        Maybe<Object> IAddressablesDatabase.FindAsset(AssetKey key)
        {
            return FindAsset(key).Map(x => x.As<Object>())!;
        }

        Object IAddressablesDatabase.GetAsset(AssetKey key) => GetAsset(key);

        UniTask<IAddressablesDatabase<TNew>> ConvertAsync<TNew>(
            ConverterAsync<TAsset, TNew> dbItemConverter,
            ConverterAsync<TAsset, TNew> dbConverter,
            bool disposePreviousDb)
            where TNew : Object;
    }
}
