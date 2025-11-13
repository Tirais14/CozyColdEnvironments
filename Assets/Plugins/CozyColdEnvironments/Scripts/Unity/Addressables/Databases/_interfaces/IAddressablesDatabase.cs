using Cysharp.Threading.Tasks;
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
        ILoadable,
        IIDMarked<Identifier>
    {
        Result<Object> this[Identifier key] { get; }

        IEnumerable<Identifier> IDs { get; }
        IEnumerable<Object> Assets { get; }
        Type AssetType { get; }
        Func<Object, Identifier>? AssetIdFactory { get; set; }

        void Add(Object asset);
        void Add(Identifier id, Object asset);

        bool Remove(Identifier id);

        bool Contains(Identifier id);

        UniTask LoadAssetsByLabelsAsync<T>(string[] labels,
            Func<T, Object[]>? converter = null)
            where T : Object;

        AddressablesDatabaseSearch Search();
    }
    public interface IAddressablesDatabase<TAsset>
        : IAddressablesDatabase,
        IEnumerable<KeyValuePair<Identifier, TAsset>>

        where TAsset : Object
    {
        new Result<TAsset> this[Identifier key] { get; }

        new IEnumerable<Identifier> IDs { get; }
        new IEnumerable<TAsset> Assets { get; }

        Result<Object> IAddressablesDatabase.this[Identifier key] {
            get => this[key].Cast<Object>();
        }

        IEnumerable<Object> IAddressablesDatabase.Assets => Assets;

        void Add(TAsset asset);
        void Add(Identifier id, TAsset asset);

        void IAddressablesDatabase.Add(Object asset)
        {
            Add(asset.As<TAsset>());
        }
        void IAddressablesDatabase.Add(Identifier id, Object asset)
        {
            Add(id, asset.As<TAsset>());
        }
    }
}
