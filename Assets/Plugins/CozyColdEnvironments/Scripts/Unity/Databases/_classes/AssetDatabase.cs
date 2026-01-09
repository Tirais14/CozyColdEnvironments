using CommunityToolkit.Diagnostics;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Object = UnityEngine.Object;

#nullable enable
#pragma warning disable S3881
namespace CCEnvs.Unity.Databases
{
    public class AssetDatabase<TAsset> : IAssetDatabase<TAsset>
    {
        public static Func<object, Identifier> DefaultAssetIdFactory { get; } = asset =>
        {
            if (asset is IIDMarked idMarked)
            {
                switch (idMarked.ID)
                {
                    case Identifier id:
                        return id;
                    case Enum en:
                        return en;
                    default:
                        break;
                }
            }

            return asset.To<Object>().name;
        };

        private readonly Dictionary<Identifier, TAsset> assets = new();
        private readonly Dictionary<TAsset, Action<TAsset>> assetDisposeHandles = new();
        private readonly AssetDatabaseQuery search = new();

        public TAsset this[Identifier id] {
            get
            {
                if (!assets.TryGetValue(id, out var value))
                    throw new InvalidOperationException($"Asset with id: {id} not found");

                return value;
            }
        }

        public IEnumerable<Identifier> Keys => assets.Keys;
        public IEnumerable<TAsset> Values => assets.Values;
        public int Count => assets.Count;
        public Type AssetType { get; } = typeof(TAsset);
        public Func<object, Identifier>? AssetIdFactory { get; set; } = DefaultAssetIdFactory;

        public AssetDatabase(int capacity)
        {
            assets = new Dictionary<Identifier, TAsset>(capacity);
        }

        public AssetDatabase()
        {
        }

        public void RegisterAsset(Identifier id, TAsset asset, Action<TAsset>? onDbDispose = null)
        {
            assets.Add(id, asset);

            if (onDbDispose is not null)
                assetDisposeHandles.Add(asset, onDbDispose);
        }

        public void RegisterAsset(TAsset asset, Action<TAsset>? onDbDispose = null)
        {
            Guard.IsNotNull(AssetIdFactory, nameof(AssetIdFactory));
            RegisterAsset(AssetIdFactory(asset!), asset, onDbDispose);
        }

        public bool UnregisterAsset(Identifier id)
        {
            return assets.Remove(id);
        }

        public bool TryGetAsset(Identifier id, [NotNullWhen(true)] out TAsset? asset)
        {
            return assets.TryGetValue(id, out asset);
        }

        public bool ContainsId(Identifier id)
        {
            return assets.ContainsKey(id);
        }

        public void UnregisterAll()
        {
            assets.Clear();
        }

        /// <returns>internal instance</returns>
        public AssetDatabaseQuery Search()
        {
            return search.Reset().From(this);
        }

        private bool disposed;
        public void Dispose() => Dispose(disposing: true);
        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
                return;

            if (disposing)
            {
                foreach (var item in assetDisposeHandles)
                    item.Value(item.Key);

                assets.Clear();
            }

            disposed = true;
        }

        public IEnumerator<KeyValuePair<Identifier, TAsset>> GetEnumerator() => assets.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
