using CCEnvs.Diagnostics;
using CCEnvs.Reflection;
using CommunityToolkit.Diagnostics;
using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using ZLinq;
using Object = UnityEngine.Object;

#nullable enable
#pragma warning disable S3881
namespace CCEnvs.Unity.AddrsAssets.Databases
{
    public class AddressablesDatabase<TAsset> : IAddressablesDatabase<TAsset>
        where TAsset : Object
    {
        public static Func<Object, Identifier> DefaultAssetIdFactory { get; } = asset =>
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

            return asset.name;
        };

        protected readonly List<AsyncOperationHandle> loadHandles = new(0);
        private readonly Dictionary<Identifier, TAsset> collection;
        private readonly System.Diagnostics.Stopwatch stopwatch = new();
        private readonly AddressablesDatabaseSearch search = new();
        private bool disposedValue;
        private int loadingCount;

        public event Action OnStartLoading = null!;
        public event Action OnLoaded = null!;

        public Result<TAsset> this[Identifier id] {
            get
            {
                if (collection.TryGetValue(id, out var asset))
                    return (asset, null);

                return (null, new AssetNotFoundException(this, id, AssetType));
            }
        }

        public Identifier ID { get; private set; }
        public IEnumerable<Identifier> Keys => collection.Keys;
        public IEnumerable<TAsset> Values => collection.Values;
        public int Count => collection.Count;
        public bool IsLoading => loadingCount > 0;
        public bool IsLoaded => !IsLoading && Count > 0;
        public Type AssetType { get; } = typeof(TAsset);
        public Func<Object, Identifier>? AssetIdFactory { get; set; } = DefaultAssetIdFactory;

        public IEnumerable<Identifier> IDs => throw new NotImplementedException();

        public IEnumerable<TAsset> Assets => throw new NotImplementedException();

        public AddressablesDatabase(Identifier id, int capacity)
        {
            ID = id;
            collection = new Dictionary<Identifier, TAsset>(capacity);
        }

        public AddressablesDatabase(int capacity)
            :
            this(id: default, capacity)
        {
        }

        public AddressablesDatabase(Identifier id)
            :
            this(id, 4)
        {
        }

        public AddressablesDatabase() : this(capacity: 4)
        {
        }

        public AddressablesDatabase(IEnumerable<KeyValuePair<Identifier, TAsset>> values)
            :
            this()
        {
            collection = new Dictionary<Identifier, TAsset>(values);
        }

        public void Add(Identifier id, TAsset asset)
        {
            collection.Add(id, asset);
        }

        public void Add(TAsset asset)
        {
            if (AssetIdFactory is null)
            {
                this.PrintError("Not found id factory.");
                return;
            }

            Add(AssetIdFactory(asset), asset);
        }

        public bool Remove(Identifier id)
        {
            return collection.Remove(id);
        }

        public bool Contains(Identifier id)
        {
            return collection.ContainsKey(id);
        }

        public AddressablesDatabaseSearch Search() => search;

        public async UniTask LoadAssetsByLabelsAsync<T>(string[] labels,
            Func<T, Object[]>? converter = null)
            where T : Object
        {
            Guard.IsNotNull(labels);
            if (labels.IsEmpty())
                return;
            if (typeof(TAsset).IsNotType<T>())
            {
                this.PrintError($"Invalid asset type: {typeof(T).GetFullName()}.");
                return;
            }

            converter ??= input => Range.From(input.As<T>());

            try
            {
                loadingCount++;
                OnStartLoadingInternal();

                var handle = await AddressableLoader.LoadAssetsPrioritizedAsync<T>(
                    labels,
                    mergeMode: Addressables.MergeMode.Intersection);

                loadHandles.Add(handle);

                handle.Result.ZL()
                    .SelectMany(x => converter(x))
                    .ForEach((asset) => Add(asset.As<TAsset>())
                    );
            }
            catch (Exception ex)
            {
                this.PrintException(ex);
            }
            finally
            {
                loadingCount--;
                OnLoadedInternal();
                TrimExcess();
            }
        }

        public void TrimExcess() => collection.TrimExcess();

        public void Dispose() => Dispose(disposing: true);

        public IEnumerator<KeyValuePair<Identifier, TAsset>> GetEnumerator() => collection.GetEnumerator();

        protected virtual void Dispose(bool disposing)
        {
            if (disposedValue)
                return;

            if (disposing)
            {
                collection.Clear();
                collection.TrimExcess();

                loadHandles.ForEach(x => x.Release());
                loadHandles.Clear();
                loadHandles.TrimExcess();
            }

            disposedValue = true;
        }

        protected virtual void OnStartLoadingInternal()
        {
            OnStartLoading?.Invoke();

            this.PrintLog("Loading started.");
            stopwatch.Start();
        }

        protected virtual void OnLoadedInternal()
        {
            OnLoaded?.Invoke();

            stopwatch.Stop();
            this.PrintLog($"Loading finished in {stopwatch.Elapsed.TotalMilliseconds} ms.");
            stopwatch.Reset();
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();


    }
}
