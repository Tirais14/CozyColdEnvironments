using CCEnvs.Collections;
using CCEnvs.FuncLanguage;
using CCEnvs.Linq;
using CCEnvs.Pools;
using CCEnvs.Rx;
using CCEnvs.Threading;
using Cysharp.Threading.Tasks;
using ObservableCollections;
using R3;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
#if ZLINQ_PLUGIN
using ZLinq;
#endif

#pragma warning disable S3236
#nullable enable
namespace CCEnvs.UnityX.Items
{
    public class Inventory : IInventory, IDisposable
    {
        internal readonly Dictionary<IItem, List<IItemContainer>> occupiedContainers = new();

        private readonly ObservableDictionary<int, IItemContainer> containers;

        private readonly Dictionary<IItemContainer, CompositeDisposable> containerDisposables;
        private readonly Dictionary<IItemContainer, int> containerIDs;

        private readonly ReactiveProperty<int> itemCount = new();

        private readonly CancellationTokenSource disposeCancellationTokenSource = new();

        private IDisposable? containerAddBinding;
        private IDisposable? containerRemoveBinding;
        private IDisposable? containerReplaceBinding;
        private IDisposable? containersClearBinding;

        private bool autoSize;

        public IItemContainer this[int id] => containers[id];

        public bool IsEmpty => ItemCount <= 0;
        public bool IsFull => FreeSpace <= 0;
        public bool AutoSize {
            get => autoSize && ContainerSample.IsNotNull();
            set => autoSize = value;
        }

        public int FreeSpace { get; private set; }
        public int ContainerCount => containers.Count;
        public int EmptyContainerCount {
            get
            {
                int occupiedContainerCount = 0;

                foreach (var contaienrs in occupiedContainers.Values)
                    occupiedContainerCount += contaienrs.Count;

                return Math.Max(ContainerCount - occupiedContainerCount, 0);
            }
        }
        public int OccupiedContainerCount => ContainerCount - EmptyContainerCount;
        public int ItemCount => itemCount.Value;

        public IReadOnlyDictionary<int, IItemContainer> Containers => containers;

        public IItemContainer? ContainerSample { get; set; }

        protected CancellationToken DisposeCancellationToken => disposeCancellationTokenSource.Token;

        Maybe<IInventory> IItemContainerInfoItemless.ParentInventory { get => null!; set => _ = value; }

        int IItemContainerInfoItemless.Capacity {
            get => ContainerCount;
            set => SetContainerCount(value);
        }

        public Inventory(
            int collectionCapacity = 4,
            IEqualityComparer<int>? idComparer = null,
            IEqualityComparer<IItemContainer?>? containerComparer = null,
            IEnumerable<IItemContainer>? initialContainers = null
            )
        {
            containers = new ObservableDictionary<int, IItemContainer>(collectionCapacity, idComparer);
            containerDisposables = new Dictionary<IItemContainer, CompositeDisposable>(collectionCapacity, containerComparer);
            containerIDs = new Dictionary<IItemContainer, int>(collectionCapacity, containerComparer);

            BindContainerAdd();
            BindContainerRemove();
            BindContainerReplace();
            BindContainersClear();

            if (initialContainers.IsNotNull())
                AddContainers(initialContainers);
        }

        public Inventory(
            ICollection<IItemContainer> initialContainers,
            IEqualityComparer<int>? idComparer = null,
            IEqualityComparer<IItemContainer?>? containerComparer = null
            )
            :
            this(
                initialContainers.Count,
                idComparer: idComparer,
                containerComparer: containerComparer,
                initialContainers: initialContainers
                )
        {
        }

        public static Inventory CreateWith(
            int containerCount,
            IEqualityComparer<int>? idComparer = null,
            IEqualityComparer<IItemContainer?>? containerComparer = null
            )
        {
            var inventory = new Inventory(
                collectionCapacity: containerCount,
                idComparer: idComparer,
                containerComparer: containerComparer
                );

            for (int i = 0; i < containerCount; i++)
                inventory.AddContainer(new ItemContainer());

            return inventory;
        }

        public static Inventory CreateWith<TItemContainer>(
            int containerCount,
            IEqualityComparer<int>? idComparer = null,
            IEqualityComparer<IItemContainer?>? containerComparer = null
            )
            where TItemContainer : IItemContainer, new()
        {
            var inventory = new Inventory(
                collectionCapacity: containerCount,
                idComparer: idComparer,
                containerComparer: containerComparer
                );

            for (int i = 0; i < containerCount; i++)
                inventory.AddContainer(new TItemContainer());

            return inventory;
        }

        ~Inventory() => Dispose();

        public bool ContainsItem() => ItemCount >= 1;
        public bool ContainsItem(IItem? item)
        {
            if (item.IsNull())
                return ContainsItem();

            return occupiedContainers.ContainsKey(item);
        }
        public bool ContainsItem(IItem? item, int count)
        {
            if (ItemCount <= 0)
                return false;

            return GetItemCount(item) >= count;
        }

        public bool TryGetContainer(int id, [NotNullWhen(true)] out IItemContainer? container)
        {
            return containers.TryGetValue(id, out container);
        }

        public void ResetContainers()
        {
            foreach (var (_, cnt) in containers)
                cnt.Clear();
        }

        public ReadOnlyItemContainer PutItem(IItem? item, int count = 1)
        {
            if (item.IsNull() || count <= 0)
                return ReadOnlyItemContainer.Empty;

            if (AutoSize)
                EnsureFreeSpace(GetItemCount(item) + count);

            static bool putItem(IItem item, ref int count, IItemContainer container)
            {
                count = container.PutItem(item, count).ItemCount;
                return count >= 1;
            }

            using (var sameItemContainers = GetContainersWithItemPooled(item, ignoreFull: true))
                foreach (var container in sameItemContainers)
                    if (!putItem(item, ref count, container))
                        return ReadOnlyItemContainer.Empty;

            using (var emptyContainers = GetEmptyContainersPooled())
                foreach (var container in emptyContainers)
                    if (!putItem(item, ref count, container))
                        return ReadOnlyItemContainer.Empty;

            return new ReadOnlyItemContainer(item, count);
        }
        public ReadOnlyItemContainer PutItem(IItemContainerInfo? containerInfo)
        {
            if (containerInfo.IsNull())
                return ReadOnlyItemContainer.Empty;

            return PutItem(containerInfo.Item.GetValue(), containerInfo.ItemCount);
        }
        public ReadOnlyItemContainer PutItem<TItemContainerInfo>(TItemContainerInfo containerInfo)
            where TItemContainerInfo : struct, IItemContainerInfo
        {
            if (containerInfo.IsEmpty)
                return ReadOnlyItemContainer.Empty;

            return PutItem(containerInfo.Item.GetValue(), containerInfo.ItemCount);
        }

        public ReadOnlyItemContainer PutItemFrom(IItemContainer? container, int count)
        {
            CC.Guard.IsNotNull(container, nameof(container));

            if (count <= 0 || container.IsEmpty)
                return ReadOnlyItemContainer.Empty;

            return container.PutItem(PutItem(container.TakeItem(count)));
        }
        public ReadOnlyItemContainer PutItemFrom(IItemContainer? container)
        {
            if (container.IsNull())
                return ReadOnlyItemContainer.Empty;

            return PutItemFrom(container, container.ItemCount);
        }

        public ReadOnlyItemContainer TakeItem(IItem? item, int count)
        {
            if (item.IsNull()
                ||
                count <= 0
                ||
                !occupiedContainers.TryGetValue(item, out List<IItemContainer> containers))
            {
                return ReadOnlyItemContainer.Empty;
            }

            var takedItems = ReadOnlyItemContainer.Empty;

            for (int i = 0; i < containers.Count; i++)
            {
                takedItems = containers[i].TakeItem(count);
                count -= takedItems.ItemCount;

                if (count <= 0)
                    break;
            }

            return takedItems;
        }

        public void EnsureFreeSpace(
            int tragetSpace,
            IItem? forItem = null,
            IItemContainer? cloneExample = null
            )
        {
#if CC_DEBUG_ENABLED
            var loopFuse = LoopFuse.Create(15000);
#endif

            while (GetFreeSpace(forItem) < tragetSpace)
            {
#if CC_DEBUG_ENABLED
                loopFuse.MoveNextThrow();
#endif

                InstantiateContainers(1, cloneExample);
            }
        }

        public int GetFreeSpace(IItem? item)
        {
            if (item.IsNull())
                return FreeSpace;

            int freeSpace = 0;

            foreach (var cnt in FilterContainersWithItem(item, ignoreFull: true))
                freeSpace += cnt.FreeSpace;

            return freeSpace;
        }

        public int GetItemCount(IItem? item)
        {
            if (item.IsNull()
                ||
                !occupiedContainers.TryGetValue(item, out var cnts))
            {
                return ItemCount;
            }

            int count = 0;

            foreach (var cnt in cnts)
                count += cnt.ItemCount;

            return count;
        }

        public IEnumerable<IItemContainer> FilterContainersWithItem(
            IItem? item,
            bool ignoreFull = true
            )
        {
            if (item.IsNull()
                ||
                !occupiedContainers.TryGetValue(item, out var containers))
            {
                yield break;
            }

            for (int i = 0; i < containers.Count; i++)
            {
                IItemContainer container = containers[i];

                if (ignoreFull && container.IsFull)
                    continue;

                yield return container;
            }
        }

        public PooledList<IItemContainer> GetContainersWithItemPooled(
            IItem item,
            bool ignoreFull = true
            )
        {
            if (!occupiedContainers.TryGetValue(item, out List<IItemContainer> containersWithItem))
                return default;

            var results = new PooledList<IItemContainer>(containersWithItem.Count);

            for (int i = 0; i < containersWithItem.Count; i++)
            {
                IItemContainer container = containersWithItem[i];

                if (ignoreFull && container.IsFull)
                    continue;

                results.Add(container);
            }

            return results;
        }

        public CCEnvs.Pools.PooledList<IItemContainer> GetEmptyContainersPooled()
        {
            if (containers.IsEmpty())
                return default;

            var results = new PooledList<IItemContainer>(containers.Count);

            foreach (var (_, container) in containers)
                if (container.IsEmpty)
                    results.Add(container);

            return results;
        }

        public IEnumerable<IItemContainer> FilterEmptyContainers()
        {
            foreach (var (_, cnt) in containers)
            {
                if (!cnt.IsEmpty)
                    continue;

                yield return cnt;
            }
        }

        public void AddContainer(IItemContainer container)
        {
            CC.Guard.IsNotNull(container, nameof(container));

            if (container.IsReadOnlyContainer)
                throw new ArgumentException($"Container cannot be readonly. Container: {container}");

            var id = ResolveID(container);

            containers[id] = container;
        }

        public void AddContainers(IEnumerable<IItemContainer> containers)
        {
            CC.Guard.IsNotNull(containers, nameof(containers));

            foreach (var container in containers)
                if (container.IsNotNull() && !container.IsReadOnlyContainer)
                    AddContainer(container);
        }

        public bool RemoveContainer(int id)
        {
            return containers.Remove(id);
        }
        public bool RemoveContainer(IItemContainer container)
        {
            if (!container.GetContainerID().TryGetValue(out int cntID))
                return false;

            return RemoveContainer(cntID);
        }

        public void InstantiateContainers(
            int count,
            IItemContainer? cloneExmaple = null
            )
        {
            InstantiateContainersCore(count, results: null, cloneExmaple);
        }

        public void InstantiateContainers(
            int count,
            out IList<IItemContainer> results,
            IItemContainer? cloneExmaple = null
            )
        {
            results = new List<IItemContainer>(count);
            InstantiateContainersCore(count, (List<IItemContainer>)results, cloneExmaple);
        }

        public void SetContainerCount(
            int count,
            IItemContainer? cloneExample = null
            )
        {
            count = Math.Max(count, 0);
            int delta = count - ContainerCount;

            if (delta < 0)
                RemoveCount(delta);
            else if (delta > 0)
                InstantiateContainers(delta, cloneExample);
        }

        public void SetContainerCount(
            int count,
            out IList<IItemContainer> changed,
            IItemContainer? cloneExample = null
            )
        {
            if (count >= 1)
            {
                int delta = count - ContainerCount;

                if (delta < 0)
                {
                    RemoveCount(delta, out changed);
                    return;
                }
                else if (delta > 0)
                {
                    InstantiateContainers(delta, out changed, cloneExample);
                    return;
                }
            }

            changed = Array.Empty<IItemContainer>();
        }

        public void RemoveCount(int removeCount)
        {
            RemoveCountCore(removeCount, null);
        }
        public void RemoveCount(int removeCount, out IList<IItemContainer> removed)
        {
            removed = new List<IItemContainer>(removeCount);
            RemoveCountCore(removeCount, (List<IItemContainer>)removed);
        }

        public bool CanPut() => !IsFull;

        public bool CanPut(IItem? item)
        {
            if (item.IsNull() || FreeSpace <= 0)
                return false;

            return GetFreeSpace(item) > 0;
        }
        public bool CanPut(IItem? item, int count)
        {
            if (item.IsNull() || FreeSpace <= 0)
                return false;

            return GetFreeSpace(item) >= count;
        }

        public Maybe<int> GetContainerID(IItemContainer cnt)
        {
            CC.Guard.IsNotNull(cnt, nameof(cnt));

            if (!containerIDs.TryGetValue(cnt, out var id))
                return Maybe<int>.None;

            return id;
        }

        public IEnumerable<ReadOnlyItemContainer> GetCompactedContainersQuery()
        {
            foreach (var containers in occupiedContainers.Values)
            {
                IItem? item = null;
                int itemCount = 0;
                bool hasItem = false;

                for (int i = 0; i < containers.Count; i++)
                {
                    if (!hasItem)
                    {
                        item = containers[i].Item.GetValue();
                        hasItem = true;
                    }

                    itemCount += containers[i].ItemCount;
                }

                if (item.IsNull() || itemCount <= 0)
                    continue;

                yield return new ReadOnlyItemContainer(item, itemCount);
            }
        }

        public IList<ReadOnlyItemContainer> GetCompactedContainers()
        {
            return GetCompactedContainersQuery().ToArray();
        }

        public IEnumerable<IItemContainer> GetOccupiedContainersQuery()
        {
            foreach (var containers in occupiedContainers.Values)
                for (int i = 0; i < containers.Count; i++)
                    yield return containers[i];
        }

        public IList<IItemContainer> GetOccupiedContainers()
        {
            return GetOccupiedContainersQuery().ToArray();
        }

        public void CopyItemFrom(IItemContainerInfo itemContainer)
        {
            PutItem(itemContainer.Item.GetValue(), itemContainer.ItemCount);
        }

        public Observable<int> ObserveItemCount() => itemCount;

        public Observable<InventoryContainerAddEvent> ObserveContainerAdd()
        {
            return containers.ObserveDictionaryAdd()
                .Select(container => new InventoryContainerAddEvent { ID = container.Key, Container = container.Value });
        }

        public Observable<InventoryContainerRemoveEvent> ObserveContainerRemove()
        {
            return containers.ObserveDictionaryRemove()
                .Select(container => new InventoryContainerRemoveEvent { ID = container.Key, Container = container.Value });
        }

        public Observable<InventoryContainerReplaceEvent> ObserveContainerReplace()
        {
            return containers.ObserveDictionaryReplace()
                .Select(container => new InventoryContainerReplaceEvent { ID = container.Key, OldContainer = container.NewValue, NewContainer = container.NewValue });
        }

        public Observable<Unit> ObserveClear() => containers.ObserveClear();

        public void Clear() => containers.Clear();

        private int disposed;
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected virtual void Dispose(bool disposing)
        {
            if (Interlocked.Exchange(ref disposed, 1) != 0)
                return;

            if (disposing)
            {
                disposeCancellationTokenSource.CancelAndDispose();

                try
                {
                    containerDisposables?.SelectValue().DisposeEach(bufferized: true);
                }
                catch (Exception ex)
                {
                    this.PrintException(ex);
                }

                try
                {
                    containers?.SelectValue()
                        .OfType<IDisposable>()
                        .DisposeEach(bufferized: true);
                }
                catch (Exception ex)
                {
                    this.PrintException(ex);
                }

                try
                {
                    itemCount?.Dispose();
                    containerAddBinding?.Dispose();
                    containerRemoveBinding?.Dispose();
                    containerReplaceBinding?.Dispose();
                    containersClearBinding?.Dispose();

                    containers?.Clear();
                    occupiedContainers?.Clear();
                }
                catch (Exception ex)
                {
                    this.PrintException(ex);
                }
            }
        }

        protected virtual int ResolveID(IItemContainer itemContainer)
        {
            var ids = new List<int>(containers.Count);

            foreach (var (cntID, _) in containers)
                ids.Add(cntID);

            if (Do.TryFindHoleInRange(start: 0, ContainerCount, ids, out int hole))
                return hole;

            return ContainerCount;
        }

        protected virtual void InstantiateContainersCore(
            int count,
            List<IItemContainer>? results,
            IItemContainer? cloneSample = null
            )
        {
            if (count <= 0)
                return;

            cloneSample = cloneSample.IfNull(ContainerSample)
                .IfNull(containers, static (containers) => containers.SelectValue().FirstOrDefault());

            if (cloneSample.IsNull())
            {
#if CC_DEBUG_ENABLED
                this.PrintWarning($"Cannot instantiate containers without {ContainerSample}");
#endif
                return;
            }

            if (cloneSample.IsReadOnlyContainer)
                throw new ArgumentException($"Item container cannot be readonly. Container: {cloneSample}");

            bool collectResults = results is not null;

            for (int i = 0; i < count; i++)
            {
                IItemContainer cloned = cloneSample.ShallowClone();
                cloned.Clear();

                AddContainer(cloned);

                if (collectResults)
                    results!.Add(cloned);
            }
        }

        protected virtual void RemoveCountCore(int removeCount, List<IItemContainer>? removed)
        {
            if (removeCount <= 0)
                return;

            bool collectRemoved = removed is not null;

            bool removeContainer(IItemContainer container)
            {
                if (RemoveContainer(container))
                    removeCount--;

                if (collectRemoved)
                    removed!.Add(container);

                return removeCount >= 1;
            }

            foreach (var emptyContainer in FilterEmptyContainers())
                if (!removeContainer(emptyContainer))
                    return;

            foreach (var container in containers.SelectValue().Reverse())
                if (!removeContainer(container))
                    return;
        }

        protected virtual void OnContainerAdd(DictionaryAddEvent<int, IItemContainer> addEv)
        {
            int id = addEv.Key;
            IItemContainer? cnt = addEv.Value;

            BindContainerItemCount(cnt);
            BindContainerItem(cnt);
            ResolveOccupied(cnt);

            cnt.ParentInventory = this;
            FreeSpace += cnt.FreeSpace;
            containerIDs[cnt] = id;
        }

        protected virtual void OnContainerItemChanged((IItem? Previous, IItem? Current) items, IItemContainer cnt)
        {
            var (previous, current) = items;

            if (previous.IsNotNull()
                &&
                occupiedContainers.TryGetValue(previous, out var occupiedCnts))
            {
                occupiedCnts.Remove(cnt);
            }

            if (current.IsNotNull())
                occupiedContainers.GetOrCreateNew(current).Add(cnt);
        }

        protected virtual void OnContainerRemove(DictionaryRemoveEvent<int, IItemContainer> removeEv)
        {
            var cnt = removeEv.Value;

            if (containerDisposables.TryGetValue(cnt, out var disposables))
                disposables.Dispose();

            FreeSpace = Math.Max(FreeSpace - cnt.FreeSpace, 0);
            itemCount.Value = Math.Max(itemCount.Value - cnt.ItemCount, 0);

            if (!cnt.IsEmpty
                &&
                cnt.Item.TryGetValue(out var item)
                &&
                occupiedContainers.TryGetValue(item, out var cnts))
            {
                cnts.Remove(cnt);
            }

            containerIDs.Remove(cnt);
        }

        protected virtual void OnContainerReplace(DictionaryReplaceEvent<int, IItemContainer> replaceEv)
        {
            var id = replaceEv.Key;
            var oldCnt = replaceEv.OldValue;
            var newCnt = replaceEv.NewValue;

            var removeEv = new DictionaryRemoveEvent<int, IItemContainer>(id, oldCnt);
            OnContainerRemove(removeEv);

            var addEv = new DictionaryAddEvent<int, IItemContainer>(id, newCnt);
            OnContainerAdd(addEv);
        }

        protected virtual void OnContainersClear(Unit _)
        {
            occupiedContainers.Clear();
            containerIDs.Clear();
            containerDisposables.SelectValue().DisposeEach(bufferized: true);
            containerDisposables.Clear();
            FreeSpace = 0;
            itemCount.Value = 0;
        }

        private void BindContainerAdd()
        {
            containerAddBinding = containers.ObserveDictionaryAdd(DisposeCancellationToken)
                .Subscribe(OnContainerAdd);
        }

        private void BindContainerItemCount(IItemContainer cnt)
        {
            var disposables = containerDisposables.GetOrCreateNew(cnt);

            cnt.ObserveItemCount()
                .Pairwise()
                .SelectDelta()
                .Subscribe(OnContainerItemCountChanged)
                .AddTo(disposables);
        }

        private void ResolveOccupied(IItemContainer cnt)
        {
            if (cnt.IsEmpty || !cnt.Item.TryGetValue(out var item))
                return;

            occupiedContainers.GetOrCreateNew(item).Add(cnt);
        }

        private void OnContainerItemCountChanged(int itemCountDelta)
        {
            itemCount.Value = Math.Clamp(itemCount.Value + itemCountDelta, 0, int.MaxValue);
        }

        private void BindContainerItem(IItemContainer cnt)
        {
            var disposables = containerDisposables.GetOrCreateNew(cnt);

            cnt.ObserveItem()
                .Unmaybe()
                .Pairwise()
                .Subscribe(cnt, OnContainerItemChanged)
                .AddTo(disposables);
        }

        private void BindContainerRemove()
        {
            containerRemoveBinding = containers.ObserveDictionaryRemove(DisposeCancellationToken)
                 .Subscribe(OnContainerRemove);
        }

        private void BindContainerReplace()
        {
            containerReplaceBinding = containers.ObserveDictionaryReplace(DisposeCancellationToken)
                .Subscribe(OnContainerReplace);
        }

        private void BindContainersClear()
        {
            containersClearBinding = containers.ObserveClear(DisposeCancellationToken)
                .Subscribe(OnContainersClear);
        }

        ReadOnlyItemContainer IItemAccessor.TakeItem(int count) => ReadOnlyItemContainer.Empty;
        ReadOnlyItemContainer IItemAccessor.TakeItem() => ReadOnlyItemContainer.Empty;

        Maybe<int> IItemContainerInfoItemless.GetContainerID() => Maybe<int>.None;
    }

    public class Inventory<TItem, TItemContainer>
        :
        IInventory<TItem, TItemContainer>,
        IDisposable

        where TItem : IItem
        where TItemContainer : IItemContainer
    {
        private readonly Inventory internalInventory;

        private readonly DictionaryView<int, IItemContainer, TItemContainer> containersView;

        public TItemContainer this[int id] {
            get => (TItemContainer)internalInventory[id];
        }

        public IReadOnlyDictionary<int, TItemContainer> Containers => containersView;

        public bool AutoSize {
            get => internalInventory.AutoSize;
            set => internalInventory.AutoSize = value;
        }
        public bool IsEmpty => internalInventory.IsEmpty;
        public bool IsFull => internalInventory.IsFull;

        public int ContainerCount => internalInventory.ContainerCount;
        public int EmptyContainerCount => internalInventory.EmptyContainerCount;
        public int OccupiedContainerCount => internalInventory.OccupiedContainerCount;
        public int ItemCount => internalInventory.ItemCount;
        public int FreeSpace => internalInventory.FreeSpace;

        public TItemContainer? ContainerSample { get; set; }

        IItemContainer IInventory.this[int id] => internalInventory[id];

        IReadOnlyDictionary<int, IItemContainer> IInventory.Containers => internalInventory.Containers;

        int IItemContainerInfoItemless.Capacity {
            get => ((IInventory)internalInventory).Capacity;
            set => ((IInventory)internalInventory).Capacity = value;
        }

        Maybe<IInventory> IItemContainerInfoItemless.ParentInventory {
            get => ((IInventory)internalInventory).ParentInventory;
            set => ((IInventory)internalInventory).ParentInventory = value;
        }

        public Inventory(
            int collectionCapacity = 4,
            IEqualityComparer<int>? idComparer = null,
            IEqualityComparer<TItemContainer?>? containerComparer = null,
            IEnumerable<TItemContainer>? initialContainers = null
            )
        {
            IEqualityComparer<IItemContainer?>? untypedContainerComparer = null;

            if (containerComparer.IsNotNull())
            {
                untypedContainerComparer = new AnonymousEqualityComparer<IItemContainer?>(
                    comparison: (left, right) =>
                    {
                        return containerComparer.Equals((TItemContainer?)left, (TItemContainer?)right);
                    },
                    hashCodeGenerator: (value) =>
                    {
                        return containerComparer.GetHashCode((TItemContainer?)value);
                    });
            }

            internalInventory = new Inventory(
                collectionCapacity: collectionCapacity,
                idComparer: idComparer,
                containerComparer: untypedContainerComparer,
                initialContainers: initialContainers.Cast<IItemContainer>()
                );

            containersView = new DictionaryView<int, IItemContainer, TItemContainer>(
                internalInventory.Containers,
                static (container) => (TItemContainer)container
                );
        }

        public Inventory(
            ICollection<TItemContainer> initialContainers,
            IEqualityComparer<int>? idComparer = null,
            IEqualityComparer<TItemContainer?>? containerComparer = null
            )
            :
            this(
                initialContainers.Count,
                idComparer: idComparer,
                containerComparer: containerComparer,
                initialContainers: initialContainers
                )
        {
        }

        public static Inventory<TItem, TItemContainer> CreateWith<TItemContainerClone>(
            int containerCount,
            IEqualityComparer<int>? idComparer = null,
            IEqualityComparer<TItemContainer?>? containerComparer = null
            )
            where TItemContainerClone : TItemContainer, new()
        {
            var inventory = new Inventory<TItem, TItemContainer>(
                collectionCapacity: containerCount,
                idComparer: idComparer,
                containerComparer: containerComparer
                );

            for (int i = 0; i < containerCount; i++)
                inventory.AddContainer(new TItemContainerClone());

            return inventory;
        }

        public void AddContainer(TItemContainer itemContainer)
        {
            internalInventory.AddContainer(itemContainer);
        }

        public void AddContainers(IEnumerable<TItemContainer> containers)
        {
            foreach (var container in containers)
                if (container.IsNotNull() && !container.IsReadOnlyContainer)
                    internalInventory.AddContainer(container);
        }

        public bool CanPut() => internalInventory.CanPut();
        public bool CanPut(IItem? item) => internalInventory.CanPut(item);
        public bool CanPut(IItem? item, int count) => internalInventory.CanPut(item, count);

        public void Clear() => internalInventory.Clear();

        public bool ContainsItem() => internalInventory.ContainsItem();
        public bool ContainsItem(IItem? item) => internalInventory.ContainsItem(item);
        public bool ContainsItem(IItem? item, int count) => internalInventory.ContainsItem(item, count);

        public void EnsureFreeSpace(
            int targetSpace,
            TItem? item = default,
            TItemContainer? cloneExample = default
            )
        {
            internalInventory.EnsureFreeSpace(targetSpace, item, cloneExample);
        }

        public Maybe<int> GetContainerID(TItemContainer cnt)
        {
            return internalInventory.GetContainerID(cnt);
        }

        public int GetFreeSpace(TItem? item) => internalInventory.GetFreeSpace(item);

        public int GetItemCount(TItem? item) => internalInventory.GetItemCount(item);

        public void InstantiateContainers(
            int count,
            TItemContainer? cloneExample = default
            )
        {
            internalInventory.InstantiateContainers(count, cloneExample);
        }

        public void InstantiateContainers(
            int count,
            out IList<TItemContainer> results,
            TItemContainer? cloneExample = default
            )
        {
            internalInventory.InstantiateContainers(
                count,
                out IList<IItemContainer> untpyedResults,
                cloneExample
                );

            results = untpyedResults
#if ZLINQ_PLUGIN
                .AsValueEnumerable()
#endif
                .Cast<TItemContainer>()
                .ToArray();
        }

        public ReadOnlyItemContainer<TItem> PutItem(TItem? item, int count = 1)
        {
            return internalInventory.PutItem(item, count).Convert<TItem>();
        }
        public ReadOnlyItemContainer<TItem> PutItem(IItemContainerInfo<TItem>? containerInfo)
        {
            return internalInventory.PutItem(containerInfo).Convert<TItem>();
        }
        public ReadOnlyItemContainer<TItem> PutItem<TItemContainerInfo>(TItemContainerInfo containerInfo)
            where TItemContainerInfo : struct, IItemContainerInfo<TItem>
        {
            return internalInventory.PutItem(containerInfo).Convert<TItem>();
        }

        public ReadOnlyItemContainer<TItem> PutItemFrom(
            IItemContainer<TItem>? container, 
            int count
            )
        {
            return internalInventory.PutItemFrom(container, count).Convert<TItem>();
        }
        public ReadOnlyItemContainer<TItem> PutItemFrom(IItemContainer<TItem>? container)
        {
            return internalInventory.PutItemFrom(container).Convert<TItem>();
        }

        public bool RemoveContainer(int id) => internalInventory.RemoveContainer(id);   

        public void RemoveCount(int count, out IList<TItemContainer> removed)
        {
            internalInventory.RemoveCount(count, out IList<IItemContainer> untypedRemoved);
            removed = untypedRemoved.Cast<TItemContainer>().ToArray();
        }
        public void RemoveCount(int count) => internalInventory.RemoveCount(count);

        public void ResetContainers() => internalInventory.ResetContainers();

        public void SetContainerCount(
            int count,
            TItemContainer? cloneExample = default
            )
        {
            internalInventory.SetContainerCount(count, cloneExample);
        }
        public void SetContainerCount(
            int count,
            out IList<TItemContainer> changed,
            TItemContainer? cloneExample = default
            )
        {
            internalInventory.SetContainerCount(
                count,
                out IList<IItemContainer> untypedChanged,
                cloneExample
                );

            changed = untypedChanged
#if ZLINQ_PLUGIN
                .AsValueEnumerable()
#endif
                .Cast<TItemContainer>()
                .ToArray();
        }

        public ReadOnlyItemContainer<TItem> TakeItem(TItem? item, int count)
        {
            return internalInventory.TakeItem(item, count).Convert<TItem>();
        }

        public bool TryGetContainer(int id, [NotNullWhen(true)] out TItemContainer? container)
        {
            if (!internalInventory.TryGetContainer(id, out IItemContainer? untypedContainer))
            {
                container = default;
                return false;
            }

            container = (TItemContainer)untypedContainer;
            return true;
        }

        public IList<ReadOnlyItemContainer<TItem>> GetCompactedContainers()
        {
            return internalInventory.GetCompactedContainersQuery()
#if ZLINQ_PLUGIN
                .AsValueEnumerable()
#endif
                .Select(container => container.Convert<TItem>())
                .ToArray();
        }

        public IList<TItemContainer> GetOccupiedContainers()
        {
            return internalInventory.GetCompactedContainersQuery()
#if ZLINQ_PLUGIN
                .AsValueEnumerable()
#endif
                .Cast<TItemContainer>()
                .ToArray();
        }

        public void CopyItemFrom(IItemContainerInfo<TItem> itemContainer)
        {
            internalInventory.CopyItemFrom(itemContainer);
        }

        public Observable<Unit> ObserveClear() => internalInventory.ObserveClear();

        public Observable<InventoryContainerAddEvent<TItemContainer>> ObserveContainerAdd()
        {
            return internalInventory.ObserveContainerAdd()
                .Select(ev => ev.Convert<TItemContainer>());
        }

        public Observable<InventoryContainerRemoveEvent<TItemContainer>> ObserveContainerRemove()
        {
            return internalInventory.ObserveContainerRemove()
                .Select(ev => ev.Convert<TItemContainer>());
        }

        public Observable<InventoryContainerReplaceEvent<TItemContainer>> ObserveContainerReplace()
        {
            return internalInventory.ObserveContainerReplace()
                .Select(ev => ev.Convert<TItemContainer>());
        }

        public Observable<int> ObserveItemCount() => internalInventory.ObserveItemCount();

        public void Dispose() => internalInventory.Dispose();

        ReadOnlyItemContainer<TItem> IItemAccessor<TItem>.TakeItem() => ReadOnlyItemContainer<TItem>.Empty;
        ReadOnlyItemContainer<TItem> IItemAccessor<TItem>.TakeItem(int count) => ReadOnlyItemContainer<TItem>.Empty;

        Maybe<int> IItemContainerInfoItemless.GetContainerID() => Maybe<int>.None;
    }
}
