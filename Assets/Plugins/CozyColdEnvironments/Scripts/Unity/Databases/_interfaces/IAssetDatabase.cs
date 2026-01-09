using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

#nullable enable
namespace CCEnvs.Unity.Databases
{
    public interface IAddressablesDatabase
        : IDisposable,
        IEnumerable
    {
        object this[Identifier key] { get; }

        IEnumerable<Identifier> Keys { get; }
        IEnumerable<object> Values { get; }
        Type AssetType { get; }
        Func<object, Identifier>? AssetIdFactory { get; set; }

        void RegisterAsset(object asset, Action<object>? onDbDispose = null);
        void RegisterAsset(Identifier id, object asset, Action<object>? onDbDispose = null);

        bool UnregisterAsset(Identifier id);

        void UnregisterAll();

        bool ContainsId(Identifier id);

        bool TryGetAsset(Identifier id, [NotNullWhen(true)] out object? asset);

        AssetDatabaseQuery Search();
    }
    public interface IAssetDatabase<TAsset>
        : IAddressablesDatabase,
        IEnumerable<KeyValuePair<Identifier, TAsset>>
    {
        new TAsset this[Identifier id] { get; }

        new IEnumerable<Identifier> Keys { get; }
        new IEnumerable<TAsset> Values { get; }

        object IAddressablesDatabase.this[Identifier key] {
            get => this[key]!;
        }

        IEnumerable<object> IAddressablesDatabase.Values {
            get => Values.Cast<object>();
        }

        void RegisterAsset(TAsset asset, Action<TAsset>? onDbDispose = null);
        void RegisterAsset(Identifier id, TAsset asset, Action<TAsset>? onDbDispose = null);

        bool UnregisterAsset(Identifier id);

        bool TryGetAsset(Identifier id, [NotNullWhen(true)] out TAsset? asset);

        void IAddressablesDatabase.RegisterAsset(Identifier id, object asset, Action<object>? onDbDispose)
        {
            if (onDbDispose is not null)
            {
                RegisterAsset(id, (TAsset)asset, assetUntyped =>
                {
                    onDbDispose(assetUntyped!);
                });
            }
            else
                RegisterAsset(id, (TAsset)asset);
        }

        void IAddressablesDatabase.RegisterAsset(object asset, Action<object>? onDbDispose)
        {
            if (onDbDispose is not null)
            {
                RegisterAsset((TAsset)asset, assetUntyped =>
                {
                    onDbDispose(assetUntyped!);
                });
            }
            else
                RegisterAsset((TAsset)asset);
        }

        bool IAddressablesDatabase.UnregisterAsset(Identifier id)
        {
            return UnregisterAsset(id);
        }

        bool IAddressablesDatabase.TryGetAsset(Identifier id, [NotNullWhen(true)] out object? asset)
        {
            if (!TryGetAsset(id, out TAsset? assetTyped))
            {
                asset = null;
                return false;
            }

            asset = assetTyped;
            return true;
        }
    }
}
