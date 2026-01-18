using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

#nullable enable
namespace CCEnvs.Unity.Databases
{
    public interface IAssetDatabase
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

        bool UnregisterAsset(Identifier id, [NotNullWhen(true)] out object? asset, out Action<object>? handle);

        void UnregisterAll();

        bool ContainsId(Identifier id);

        bool TryGetAsset(Identifier id, [NotNullWhen(true)] out object? asset);

        AssetDatabaseQuery Query();
    }
    public interface IAssetDatabase<TAsset>
        : IAssetDatabase,
        IEnumerable<KeyValuePair<Identifier, TAsset>>
    {
        new TAsset this[Identifier id] { get; }

        new IEnumerable<Identifier> Keys { get; }
        new IEnumerable<TAsset> Values { get; }

        object IAssetDatabase.this[Identifier key] {
            get => this[key]!;
        }

        IEnumerable<object> IAssetDatabase.Values {
            get => Values.Cast<object>();
        }

        void RegisterAsset(TAsset asset, Action<TAsset>? onDbDispose = null);
        void RegisterAsset(Identifier id, TAsset asset, Action<TAsset>? onDbDispose = null);

        bool UnregisterAsset(Identifier id, [NotNullWhen(true)] out TAsset? asset, out Action<TAsset>? handle);

        bool TryGetAsset(Identifier id, [NotNullWhen(true)] out TAsset? asset);

        void IAssetDatabase.RegisterAsset(Identifier id, object asset, Action<object>? onDbDispose)
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

        void IAssetDatabase.RegisterAsset(object asset, Action<object>? onDbDispose)
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

        bool IAssetDatabase.UnregisterAsset(Identifier id, [NotNullWhen(true)] out object? asset, out Action<object>? handle)
        {
            var result = UnregisterAsset(id, out TAsset? assetTyped, out Action<TAsset>? handleTyped);

            asset = assetTyped;

            if (handleTyped is not null)
                handle = asset => handleTyped((TAsset)asset);
            else
                handle = null;

            return result;
        }

        bool IAssetDatabase.TryGetAsset(Identifier id, [NotNullWhen(true)] out object? asset)
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
