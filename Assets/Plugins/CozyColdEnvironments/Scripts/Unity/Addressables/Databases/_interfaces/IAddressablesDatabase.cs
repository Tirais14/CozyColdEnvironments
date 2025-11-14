using CCEnvs.Collections;
using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
        Result<object> this[Identifier key] { get; set; }

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
        new Result<TAsset> this[Identifier id] { get; set; }

        new IEnumerable<Identifier> Keys { get; }
        new IEnumerable<TAsset> Values { get; }

        Result<object> IAddressablesDatabase.this[Identifier key] {
            get => this[key].Cast<object>();
            set => this[key] = value.Cast<TAsset>();
        }

        IEnumerable<object> IAddressablesDatabase.Values {
            get => Values.Cast<object>();
        }

        void Add(TAsset asset);

        void IAddressablesDatabase.Add(object asset)
        {
            Add(asset.As<TAsset>());
        }
    }
}
