using CCEnvs.Collections;
using CCEnvs.Diagnostics;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.AddressableAssets;
using Object = UnityEngine.Object;

#nullable enable
#pragma warning disable S3881
namespace CCEnvs.Unity.AddrsAssets.Databases
{
    public class AddressablesDatabase<TAsset> : IAddressablesDatabase<TAsset>
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

        private readonly CCDictionary<Identifier, TAsset> collection = new();
        private readonly System.Diagnostics.Stopwatch stopwatch = new();
        private readonly AddressablesDatabaseSearch search = new();

        public Result<TAsset> this[Identifier id] {
            get => collection[id];
            set => collection[id] = value;
        }

        public IEnumerable<Identifier> Keys => collection.Keys;
        public IEnumerable<TAsset> Values => collection.Values;
        public int Count => collection.Count;
        public Type AssetType { get; } = typeof(TAsset);
        public Func<object, Identifier>? AssetIdFactory { get; set; } = DefaultAssetIdFactory;

        bool ICollection<KeyValuePair<Identifier, TAsset>>.IsReadOnly => false;

        public AddressablesDatabase(int capacity)
        {
            collection = new CCDictionary<Identifier, TAsset>(capacity);
        }

        public AddressablesDatabase()
        {
        }

        public void Add(Identifier id, TAsset value)
        {
            collection.Add(id, value);
        }

        public void Add(KeyValuePair<Identifier, TAsset> item)
        {
            collection.Add(item);
        }

        public void Add(TAsset asset)
        {
            if (AssetIdFactory is null)
            {
                this.PrintError("Not found id factory.");
                return;
            }

            Add(AssetIdFactory(asset.To<Object>()), asset);
        }

        public bool Contains(KeyValuePair<Identifier, TAsset> item)
        {
            return collection.Contains(item);
        }

        public bool ContainsKey(Identifier key)
        {
            return collection.ContainsKey(key);
        }

        public bool Remove(KeyValuePair<Identifier, TAsset> item)
        {
            return collection.Remove(item);
        }

        public bool Remove(Identifier id)
        {
            return collection.Remove(id);
        }

        public void Clear()
        {
            collection.Clear();
        }

        public void CopyTo(KeyValuePair<Identifier, TAsset>[] array, int arrayIndex)
        {
            collection.CopyTo(array, arrayIndex);
        }

        public AddressablesDatabaseSearch Search()
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
                foreach (var asset in collection.Values)
                {
                    try
                    {
                        Addressables.Release(asset);
                    }
                    catch (Exception ex)
                    {
                        this.PrintExceptionAsLog(ex);
                    }
                }

                collection.Clear();
            }

            disposed = true;
        }

        public IEnumerator<KeyValuePair<Identifier, TAsset>> GetEnumerator() => collection.GetEnumerator();

        protected virtual void OnLoadingInternal()
        {
            this.PrintLog("Loading started.");
            stopwatch.Start();
        }

        protected virtual void OnLoadedInternal()
        {
            stopwatch.Stop();
            this.PrintLog($"Loading finished in {stopwatch.Elapsed.TotalMilliseconds} ms.");
            stopwatch.Reset();
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
