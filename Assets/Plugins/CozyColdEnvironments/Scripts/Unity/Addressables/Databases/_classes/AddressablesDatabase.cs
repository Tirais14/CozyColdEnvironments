using CCEnvs.Diagnostics;
using CCEnvs.FuncLanguage;
using CCEnvs.Reflection;
using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Linq;
using SuperLinq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
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
        protected readonly List<AsyncOperationHandle> loadHandles = new(0);
        private readonly Dictionary<AssetKey, TAsset> collection;
        private readonly System.Diagnostics.Stopwatch stopwatch = new();
        private readonly DatabaseQuery query = new();
        private bool disposedValue;
        private int loadingCount;

        public event Action OnStartLoading = null!;
        public event Action OnLoaded = null!;

        public TAsset this[AssetKey key] => GetAsset(key);

        public UniID ID { get; private set; }
        public IEnumerable<AssetKey> Keys => collection.Keys;
        public IEnumerable<TAsset> Values => collection.Values;
        public int Count => collection.Count;
        public bool IsLoading => loadingCount > 0;
        public bool IsLoaded => !IsLoading && Count > 0;
        public Type AssetType { get; } = typeof(TAsset);
        public Func<Object, AssetKey>? KeyFactory { get; set; }
        public Func<Object, int>? IDFactory { get; set; }
        public Func<string, string>? AssetNameProcessor { get; set; } = DefaultAssetNameProcessor;
        public DatabaseQuery Q => Query();

        public AddressablesDatabase(UniID id, int capacity)
        {
            ID = id;
            collection = new Dictionary<AssetKey, TAsset>(capacity);
        }

        public AddressablesDatabase(int capacity)
            :
            this(id: default, capacity)
        {
        }

        public AddressablesDatabase(UniID id)
            :
            this(id, 4)
        {
        }

        public AddressablesDatabase() : this(capacity: 4)
        {
        }

        public AddressablesDatabase(IEnumerable<KeyValuePair<AssetKey, TAsset>> values)
            :
            this()
        {
            collection = new Dictionary<AssetKey, TAsset>(values);
        }

        public static string DefaultAssetNameProcessor(string assetName)
        {
            return assetName.Delete("(Clone)");
        }

        public void AddAsset(TAsset asset)
        {
            CC.Guard.IsNotNull(asset, nameof(asset));

            try
            {
                AssetKey key = KeyFactory is null ? CreateAssetKey(asset) : KeyFactory(asset);

                collection.Add(key, asset);
                this.PrintLog($"Asset {asset.GetType().GetFullName()} added. {nameof(AssetKey)}: {key}.");
            }
            catch (Exception ex)
            {
                this.PrintException(ex);
            }
        }

        public DatabaseQuery Query() => query.Reset().In(this);

        public void AddAssets(IEnumerable<TAsset> assets)
        {
            CC.Guard.IsNotNull(assets, nameof(assets));

            try
            {
                foreach (var asset in assets)
                    AddAsset(asset);
            }
            catch (Exception ex)
            {
                this.PrintException(ex);
            }
        }

        public async UniTask LoadAssetsAsync<TAnyAsset>(AssetLabels assetLabels,
            Func<TAnyAsset, TAsset[]> converter)
            where TAnyAsset : Object
        {
            CC.Guard.Argument(assetLabels.IsDefault(), nameof(assetLabels));

            try
            {
                loadingCount++;
                OnStartLoadingInternal();

                var handle = await AddressableLoader.LoadAssetsPrioritizedAsync<TAnyAsset>(
                    assetLabels,
                    mergeMode: assetLabels.MustBeAll
                               ?
                               Addressables.MergeMode.Intersection
                               :
                               Addressables.MergeMode.Union);

                loadHandles.Add(handle);
                handle.Result.ZL().SelectMany(x => converter(x)).ForEach(AddAsset);
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

        public async UniTask LoadAssetsAsync<TSub>(AssetLabels assetLabels)
            where TSub : TAsset
        {
            await LoadAssetsAsync<TSub>(assetLabels, (x) => Range.From(x));
        }
        public virtual async UniTask LoadAssetsAsync(AssetLabels assetLabels)
        {
            await LoadAssetsAsync<TAsset>(assetLabels);
        }

        public IAddressablesDatabase CutByType(Type assetType)
        {
            var newDB = InstanceFactory.Create(typeof(AddressablesDatabase<>).MakeGenericType(assetType))
                                       .As<IAddressablesDatabase>();

            this.AsReflected().CopyTypeDataTo(newDB, nameof(KeyFactory), nameof(IDFactory));

            var assets = collection.AsValueEnumerable()
                           .GroupBy(x => x.Value.GetType())
                           .First(x => x.Key == assetType)
                           .Select(x => x)
                           .Do(x => collection.Remove(x.Key))
                           .Select(x => x.Value);

            newDB.AddAssets(assets);
            return newDB;
        }
        public IAddressablesDatabase<T> CutByType<T>() where T : Object
        {
            return CutByType(typeof(T)).As<IAddressablesDatabase<T>>();
        }

        public IAddressablesDatabase[] CutByTypes()
        {
            return collection.Values.AsValueEnumerable()
                            .Select(x => x.GetType())
                            .Distinct()
                            .Select(assetType => CutByType(assetType))
                            .ToArray();
        }

        public Maybe<TAsset> FindAsset(AssetKey key)
        {
            if (collection.TryGetValue(key, out var asset))
                return asset;

            return (key.AssetID.IsDefault()
                   ?
                   collection.ZL().FirstOrDefault(x => x.Key.AssetName == key.AssetName)
                   :
                   collection.ZL().FirstOrDefault(x => x.Key.AssetID == key.AssetID))
                   .Maybe()
                   .Map(x => x.Value)!;
        }

        public Maybe<T> FindAsset<T>(AssetKey key)
        {
            return FindAsset(key).AsOrDefault<T>();
        }

        public TAsset GetAsset(AssetKey key)
        {
            if (collection.TryGetValue(key, out TAsset asset)
                ||
                new Maybe<TAsset>(FindAsset(key).Access()!).TryAccess(out asset!)
                )
                return asset;

            throw new AssetNotFoundException(key);
        }

        public T GetAsset<T>(AssetKey key)
        {
            return GetAsset(key).As<T>();
        }

        public async UniTask<IAddressablesDatabase<TNew>> ConvertAsync<TNew>(
            ConverterAsync<TAsset, TNew> dbItemConverter,
            ConverterAsync<TAsset, TNew> dbConverter,
            bool disposePreviousDb)
            where TNew : Object
        {
            CC.Guard.IsNotNull(dbConverter, nameof(dbConverter));
            CC.Guard.IsNotNull(dbItemConverter, nameof(dbItemConverter));

            var converesationTasks = collection.Values.AsValueEnumerable()
                                              .Select(x => dbItemConverter(x))
                                              .ToArray();

            var newDb = new AddressablesDatabase<TNew>();
            var converted = await converesationTasks.ToUniTaskAsyncEnumerable()
                                                    .SelectAwait(async task => await task)
                                                    .ToArrayAsync();

            if (disposePreviousDb)
                Dispose();

            newDb.AddAssets(converted);

            return newDb;
        }

        public bool ContainsKey(AssetKey key) => collection.ContainsKey(key);

        public void TrimExcess() => collection.TrimExcess();

        public void Dispose() => Dispose(disposing: true);

        public IEnumerator<KeyValuePair<AssetKey, TAsset>> GetEnumerator() => collection.GetEnumerator();

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

        protected AssetKey CreateAssetKey(Object asset)
        {
            string assetName = AssetNameProcessor?.Invoke(asset.name) ?? asset.name;
            int id = IDFactory?.Invoke(asset) ?? default;

            return new AssetKey(assetName, id);
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        bool IReadOnlyDictionary<AssetKey, TAsset>.TryGetValue(AssetKey key, out TAsset value)
        {
            return collection.TryGetValue(key, out value);
        }
    }
}
