using CCEnvs.Collections;
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

        IEnumerable<Identifier> Keys { get; }
        IEnumerable<Object> Values { get; }
        Type AssetType { get; }
        Func<Object, Identifier>? AssetIdFactory { get; set; }

        void Add(Object asset);

        UniTask LoadAssetsByLabelsAsync<T>(string[] labels,
            Func<T, Object[]>? converter = null)
            where T : Object;

        AddressablesDatabaseSearch Search();
    }
    public interface IAddressablesDatabase<TAsset>
        : IAddressablesDatabase,
        ICCDictionary<Identifier, TAsset>

        where TAsset : Object
    {
        Result<Object> IAddressablesDatabase.this[Identifier key] {
            get => this[key].Cast<Object>();
        }

        IEnumerable<Object> IAddressablesDatabase.Values {
            get => this.As<IAddressablesDatabase>().Values;
        }

        void Add(TAsset asset);

        void IAddressablesDatabase.Add(Object asset)
        {
            Add(asset.As<TAsset>());
        }
    }
}
