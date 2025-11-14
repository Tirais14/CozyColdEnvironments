using CCEnvs.Collections;
using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Object = UnityEngine.Object;

#nullable enable
namespace CCEnvs.Unity.AddrsAssets.Databases
{
    public interface IAddressablesDatabase
        : IDisposable,
        IEnumerable
    {
        Result<object> this[Identifier key] { get; }

        IEnumerable<Identifier> Keys { get; }
        IEnumerable<object> Values { get; }
        Type AssetType { get; }
        Func<object, Identifier>? AssetIdFactory { get; set; }

        void Add(object asset);

        AddressablesDatabaseSearch Search();
    }
    public interface IAddressablesDatabase<TAsset>
        : IAddressablesDatabase,
        ICCDictionary<Identifier, TAsset>
    {
        Result<object> IAddressablesDatabase.this[Identifier key] {
            get => this[key].Cast<object>();
        }

        IEnumerable<object> IAddressablesDatabase.Values {
            get => this.As<IAddressablesDatabase>().Values;
        }

        void Add(TAsset asset);

        void IAddressablesDatabase.Add(object asset)
        {
            Add(asset.As<TAsset>());
        }
    }
}
