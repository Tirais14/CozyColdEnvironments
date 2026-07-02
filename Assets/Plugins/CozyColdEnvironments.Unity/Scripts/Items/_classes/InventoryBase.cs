using CCEnvs.Collections;
using CCEnvs.Diagnostics;
using CCEnvs.FuncLanguage;
using CCEnvs.Linq;
using CCEnvs.Pools;
using CCEnvs.Rx;
using CCEnvs.Threading;
using CCEnvs.TypeMatching;
using ObservableCollections;
using R3;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;

#nullable enable
namespace CCEnvs.UnityX.Items
{
    public abstract class InventoryBase<TItem, TItemContainer, TInputItemContainer, TInputItemContainerInfo, TReadOnlyItemContainer, TLargeReadOnlyItemContainer, TContainerAddEvent, TContainerRemoveEvent, TContainerReplaceEvent>
        
        where TItem : class, IItem
        where TItemContainer : class, IItemContainer
        where TInputItemContainer : IItemContainer
        where TInputItemContainerInfo : IItemContainerInfo
        where TReadOnlyItemContainer : struct, IItemContainerInfo
        where TLargeReadOnlyItemContainer : struct, IItemContainerInfo
        where TContainerAddEvent : struct
        where TContainerRemoveEvent : struct
        where TContainerReplaceEvent : struct
    {
        private readonly ObservableDictionary<int, TItemContainer> containers;

        private readonly Dictionary<TItem, List<TItemContainer>> occupiedContainers = new();
        private readonly Dictionary<TItemContainer, CompositeDisposable> containerDisposables;

        private readonly ReactiveProperty<int> itemCount = new();

        private readonly CancellationTokenSource disposeCancellationTokenSource = new();

        private IDisposable? containerAddBinding;
        private IDisposable? containerRemoveBinding;
        private IDisposable? containerReplaceBinding;
        private IDisposable? containersClearBinding;

        private bool autoSize;

        public TItemContainer this[int id] => containers[id];

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

        public IEnumerable<KeyValuePair<int, TItemContainer>> Containers => containers;

        public TItemContainer? ContainerSample { get; set; }

        public IEqualityComparer<TItemContainer?> ContainerComaprer => containerDisposables.Comparer!;

        protected CancellationToken DisposeCancellationToken => disposeCancellationTokenSource.Token;

        protected InventoryBase(
            int collectionCapacity = 4,
            IEqualityComparer<TItemContainer?>? containerComparer = null,
            IEnumerable<TItemContainer>? initialContainers = null
            )
        {
            containerComparer ??= ReferenceEqualityComparer<TItemContainer>.Default!;

            containers = new ObservableDictionary<int, TItemContainer>(collectionCapacity, null);
            containerDisposables = new Dictionary<TItemContainer, CompositeDisposable>(collectionCapacity, containerComparer);

            BindContainerAdd();
            BindContainerRemove();
            BindContainerReplace();
            BindContainersClear();

            if (initialContainers.IsNotNull())
                AddContainers(initialContainers);
        }

        protected InventoryBase(
            ICollection<TItemContainer> initialContainers,
            IEqualityComparer<TItemContainer?>? containerComparer = null
            )
            :
            this(
                initialContainers.Count,
                containerComparer,
                initialContainers
                )
        {
        }

        ~InventoryBase() => Dispose();

        public bool ContainsItem() => ItemCount >= 1;
        public bool ContainsItem(TItem? item)
        {
            if (item.IsNull())
                return ContainsItem();

            return occupiedContainers.ContainsKey(item);
        }
        public bool ContainsItem(TItem? item, int count)
        {
            if (ItemCount <= 0)
                return false;

            return GetItemCount(item) >= count;
        }

        public bool TryGetContainer(int id, [NotNullWhen(true)] out TItemContainer? container)
        {
            return containers.TryGetValue(id, out container);
        }

        public void ResetContainers()
        {
            foreach (var (_, cnt) in containers)
                cnt.Clear();
        }

        public TLargeReadOnlyItemContainer PutItem(TItem? item, long count = 1)
        {
            if (count <= 0 || item.IsNull())
                return CreateLargeReadOnlyItemContainer();

            if (AutoSize)
            {
                long targetItemCount = GetItemCount(item) + count;
                EnsureFreeSpace(targetItemCount, item);
            }

            static bool putItem(TItem item, ref long count, TItemContainer container)
            {
                count = container.PutItem(item, count.ToInt()).ItemCount;
                return count >= 1;
            }

            using (var sameItemContainers = GetContainersWithItemPooled(item, ignoreFull: true))
                foreach (var container in sameItemContainers)
                    if (!putItem(item, ref count, container))
                        return CreateLargeReadOnlyItemContainer();

            using (var emptyContainers = GetEmptyContainersPooled())
                foreach (var container in emptyContainers)
                    if (!putItem(item, ref count, container))
                        return CreateLargeReadOnlyItemContainer();

            return CreateLargeReadOnlyItemContainer(item, count);
        }
        public TReadOnlyItemContainer PutItem(TInputItemContainerInfo? containerInfo)
        {
            if (containerInfo.IsNull())
                return CreateReadOnlyItemContainer();

            return ConvertLargeToNormalReadOnlyContainer(PutItem(containerInfo.Item.CastTo<TItem>(), containerInfo.ItemCount));
        }
        public TReadOnlyItemContainer PutItem(TReadOnlyItemContainer readOnlyContainer)
        {
            if (readOnlyContainer.IsEmpty)
                return CreateReadOnlyItemContainer();

            return ConvertLargeToNormalReadOnlyContainer(PutItem(readOnlyContainer.Item.CastTo<TItem>()!, readOnlyContainer.ItemCount));
        }
        public TLargeReadOnlyItemContainer PutItem(TLargeReadOnlyItemContainer readOnlyContainer)
        {
            return PutItem(
                GetItemFromReadOnlyContainer(readOnlyContainer),
                GetItemCountFromReadOnlytContainer(readOnlyContainer)
                );
        }

        public TReadOnlyItemContainer PutItemFrom(TInputItemContainer? container, int count)
        {
            CC.Guard.IsNotNull(container, nameof(container));

            if (count <= 0 || container.IsEmpty)
                return CreateReadOnlyItemContainer();

            ReadOnlyItemContainer containerItems = container.TakeItem(count);
            TLargeReadOnlyItemContainer notFitItems = PutItem(containerItems.Item.CastTo<TItem>(), containerItems.ItemCount);
            ReadOnlyItemContainer restItems = container.PutItem(notFitItems);

            return CreateReadOnlyItemContainer(restItems.Item.CastTo<TItem>(), restItems.ItemCount);
        }
        public TReadOnlyItemContainer PutItemFrom(TInputItemContainer? container)
        {
            if (container.IsNull())
                return CreateReadOnlyItemContainer();

            return PutItemFrom(container, container.ItemCount);
        }

        public TLargeReadOnlyItemContainer TakeItem(TItem? item, long count)
        {
            if (item.IsNull()
                ||
                count <= 0
                ||
                !occupiedContainers.TryGetValue(item, out List<TItemContainer> containers))
            {
                return CreateLargeReadOnlyItemContainer();
            }

            long takenCount = 0;
            ReadOnlyItemContainer takenItems;

            for (int i = 0; i < containers.Count; i++)
            {
                takenItems = containers[i].TakeItem(count.ToInt());
                takenCount += takenItems.ItemCount;
                count -= takenCount;

                if (count <= 0)
                    break;
            }

            return CreateLargeReadOnlyItemContainer(item, takenCount);
        }
        public TLargeReadOnlyItemContainer TakeItem(TItem? item)
        {
            return TakeItem(item, long.MaxValue);
        }

        public void EnsureFreeSpace(
            long targetSpace,
            TItem? forItem = null,
            TItemContainer? cloneSample = null
            )
        {
            if (targetSpace <= 0)
                return;

            int containerCapacity = Math.Min(
                cloneSample.IfNull(ContainerSample).IfNotNull(container => container.Capacity),
                forItem.Maybe().Map(item => item.MaxItemCount).GetValue(int.MaxValue)
                );

            if (GetFreeSpace(forItem) >= targetSpace)
                return;

            double targetContainerCountRaw = targetSpace / (double)containerCapacity;
            int targetContainerCount = (int)Math.Ceiling(targetContainerCountRaw);
            InstantiateContainers(targetContainerCount, cloneSample);
        }

        public long GetFreeSpace(TItem? item)
        {
            if (item.IsNull())
                return FreeSpace;

            long freeSpace = 0;

            foreach (var container in FilterContainersWithItem(item, ignoreFull: true))
                freeSpace += container.FreeSpace;

            foreach (var container in FilterEmptyContainers())
            {
                if (!container.IgnoreMaxItemCount)
                    freeSpace += item.MaxItemCount;
                else
                    freeSpace += int.MaxValue;
            }

            return freeSpace;
        }

        public long GetItemCount(TItem? item)
        {
            if (item.IsNull()
                ||
                !occupiedContainers.TryGetValue(item, out var cnts))
            {
                return ItemCount;
            }

            long count = 0;

            foreach (var cnt in cnts)
                count += cnt.ItemCount;

            return count;
        }

        public IEnumerable<TItemContainer> FilterContainersWithItem(
            TItem item,
            bool ignoreFull = true
            )
        {
            CC.Guard.IsNotNull(item, nameof(item));

            if (item.IsNull()
                ||
                !occupiedContainers.TryGetValue(item, out var containers))
            {
                yield break;
            }

            for (int i = 0; i < containers.Count; i++)
            {
                TItemContainer container = containers[i];

                if (!EqualityComparer<TItem?>.Default.Equals(item, (TItem)container.Item!)
                    ||
                    (ignoreFull && container.IsFull))
                {
                    continue;
                }

                yield return container;
            }
        }

        public PooledList<TItemContainer> GetContainersWithItemPooled(
            TItem item,
            bool ignoreFull = true
            )
        {
            if (!occupiedContainers.TryGetValue(item, out List<TItemContainer> containersWithItem))
                return default;

            var results = new PooledList<TItemContainer>(containersWithItem.Count);

            for (int i = 0; i < containersWithItem.Count; i++)
            {
                TItemContainer container = containersWithItem[i];

                if (ignoreFull && container.IsFull)
                    continue;

                results.Add(container);
            }

            return results;
        }

        public PooledList<TItemContainer> GetEmptyContainersPooled()
        {
            if (containers.IsEmpty())
                return default;

            var results = new PooledList<TItemContainer>(containers.Count);

            foreach (var (_, container) in containers)
                if (container.IsEmpty)
                    results.Add(container);

            return results;
        }

        public IEnumerable<TItemContainer> FilterEmptyContainers()
        {
            foreach (var (_, cnt) in containers)
            {
                if (!cnt.IsEmpty)
                    continue;

                yield return cnt;
            }
        }

        public int AddContainer(TItemContainer container, int? id = null)
        {
            CC.Guard.IsNotNull(container, nameof(container));

            if (ContainsContainer(container))
                throw new ArgumentException($"Container already added. Container: {container}");

            if (container.ParentInventory is not null
                &&
                !EqualityComparer<IInventory?>.Default.Equals(container.ParentInventory, (IInventory)this))
            {
                throw new ArgumentException("Container cannot have another parent inventory");
            }

            if (container.IsReadOnlyContainer)
                throw new ArgumentException($"Container cannot be readonly. Container: {container}");

            if (!id.HasValue)
                id = ResolveID();

            containers.Add(id.Value, container);

            if (CCDebug<Inventory>.IsEnabled)
            {
                this.PrintLog(DebugMessageBuilder.CreatePooled()
                    .AddMessage("Container added")
                    .AddProperty("ID", id)
                    .AddProperty(nameof(container), container)
                    .ToStringAndDispose()
                    );
            }

            return id.Value;
        }

        public void AddContainers(IEnumerable<TItemContainer> containers)
        {
            CC.Guard.IsNotNull(containers, nameof(containers));

            foreach (var container in containers)
            {
                if (container.IsNull() || container.IsReadOnlyContainer)
                    continue;

                if (container.ParentInventory is not null)
                    container.SetParentInventory(null);

                AddContainer(container);
            }
        }
        public void AddContainers(IEnumerable<(TItemContainer Value, int? ID)> containers)
        {
            CC.Guard.IsNotNull(containers, nameof(containers));

            foreach (var container in containers)
            {
                if (container.IsNull() || container.Value.IsReadOnlyContainer)
                    continue;

                if (container.Value.ParentInventory is not null)
                    container.Value.SetParentInventory(null);

                AddContainer(container.Value, container.ID);
            }
        }
        public void AddContainers(
            IEnumerable<TItemContainer> containers,
            out IList<int> ids
            )
        {
            if (containers.TryGetNonEnumeratedCount(out int containerCount))
                ids = new List<int>(containerCount);
            else
                ids = new List<int>();

            foreach (var container in containers)
            {
                if (container.IsNull() || container.IsReadOnlyContainer)
                    continue;

                if (container.ParentInventory is not null)
                    container.SetParentInventory(null);

                int id = AddContainer(container);
                ids.Add(id);
            }
        }

        public void AddContainers(
            IEnumerable<(TItemContainer Value, int? ID)> containers,
            out IList<int> ids
            )
        {
            if (containers.TryGetNonEnumeratedCount(out int containerCount))
                ids = new List<int>(containerCount);
            else
                ids = new List<int>();

            foreach (var container in containers)
            {
                if (container.IsNull() || container.Value.IsReadOnlyContainer)
                    continue;

                if (container.Value.ParentInventory is not null)
                    container.Value.SetParentInventory(null);

                int id = AddContainer(container.Value, container.ID);
                ids.Add(id);
            }
        }

        public bool RemoveContainer(int id)
        {
            return containers.Remove(id);
        }
        public bool RemoveContainer(TItemContainer container)
        {
            if (!container.ID.HasValue)
                return false;

            return RemoveContainer(container.ID.Value);
        }

        public bool ContainsContainer(IItemContainer? container)
        {
            return container is TItemContainer typedContainer &&
                   containerDisposables.ContainsKey(typedContainer);
        }
        public bool ContainsContainer(int? id)
        {
            return id.HasValue &&
                   containers.ContainsKey(id.Value);
        }

        public void InstantiateContainers(
            int count,
            TItemContainer? cloneExmaple = null
            )
        {
            InstantiateContainersCore(count, results: null, cloneExmaple);
        }
        public void InstantiateContainers(
            int count,
            out IList<TItemContainer> results,
            TItemContainer? cloneExmaple = null
            )
        {
            results = new List<TItemContainer>(count);
            InstantiateContainersCore(count, results, cloneExmaple);
        }

        public void SetContainerCount(
            int count,
            TItemContainer? containerSample = null
            )
        {
            count = Math.Max(count, 0);
            int delta = count - ContainerCount;

            if (delta < 0)
                RemoveCount(delta);
            else if (delta > 0)
                InstantiateContainers(delta, containerSample);
        }

        public void SetContainerCount(
            int count,
            out IList<TItemContainer> changed,
            TItemContainer? containerSample = null
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
                    InstantiateContainers(delta, out changed, containerSample);
                    return;
                }
            }

            changed = Array.Empty<TItemContainer>();
        }

        public void RemoveCount(int removeCount)
        {
            RemoveCountCore(removeCount, null);
        }
        public void RemoveCount(int removeCount, out IList<TItemContainer> removed)
        {
            removed = new List<TItemContainer>(removeCount);
            RemoveCountCore(removeCount, removed);
        }

        public bool CanPut() => !IsFull;
        public bool CanPut(TItem? item)
        {
            if (item.IsNull() || FreeSpace <= 0)
                return false;

            return GetFreeSpace(item) > 0;
        }
        public bool CanPut(TItem? item, int count)
        {
            if (item.IsNull() || FreeSpace <= 0)
                return false;

            return GetFreeSpace(item) >= count;
        }

        public IEnumerable<TLargeReadOnlyItemContainer> GetCompactedContainersQuery()
        {
            foreach (var containers in occupiedContainers.Values)
            {
                IItem? item = null;
                long itemCount = 0;
                bool hasItem = false;

                for (int i = 0; i < containers.Count; i++)
                {
                    if (!hasItem)
                    {
                        item = containers[i].Item;
                        hasItem = true;
                    }

                    itemCount += containers[i].ItemCount;
                }

                if (item.IsNull() || itemCount <= 0)
                    continue;

                yield return CreateLargeReadOnlyItemContainer((TItem)item, itemCount);
            }
        }

        public IList<TLargeReadOnlyItemContainer> GetCompactedContainers()
        {
            return GetCompactedContainersQuery().ToArray();
        }

        public IEnumerable<TItemContainer> GetOccupiedContainersQuery()
        {
            foreach (var containers in occupiedContainers.Values)
                for (int i = 0; i < containers.Count; i++)
                    yield return containers[i];
        }

        public IList<TItemContainer> GetOccupiedContainers()
        {
            return GetOccupiedContainersQuery().ToArray();
        }

        public void CopyItemFrom(TInputItemContainer itemContainer)
        {
            PutItem((TItem)itemContainer.Item!, itemContainer.ItemCount);
        }

        public Observable<int> ObserveItemCount() => itemCount;

        public Observable<TContainerAddEvent> ObserveContainerAdd()
        {
            return containers.ObserveDictionaryAdd(DisposeCancellationToken)
                .Select(this, static (container, @this) => @this.CreateContainerAddEvent(container.Key, container.Value));
        }

        public Observable<TContainerRemoveEvent> ObserveContainerRemove()
        {
            return containers.ObserveDictionaryRemove(DisposeCancellationToken)
                .Select(this, static (container, @this) => @this.CreateContainerRemoveEvent(container.Key, container.Value));
        }

        public Observable<TContainerReplaceEvent> ObserveContainerReplace()
        {
            return containers.ObserveDictionaryReplace(DisposeCancellationToken)
                .Select(this, static (container, @this) => @this.CreateContainerReplaceEvent(container.Key, container.OldValue, container.NewValue));
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

                containerDisposables?.SelectValue()
                    .DisposeEach(bufferized: true);

                containers?.SelectValue()
                        .OfType<IDisposable>()
                        .DisposeEach(bufferized: true);

                itemCount?.Dispose();
                containerAddBinding?.Dispose();
                containerRemoveBinding?.Dispose();
                containerReplaceBinding?.Dispose();
                containersClearBinding?.Dispose();

                containers?.Clear();
                occupiedContainers?.Clear();
            }
        }

        protected abstract TReadOnlyItemContainer CreateReadOnlyItemContainer();
        protected abstract TReadOnlyItemContainer CreateReadOnlyItemContainer(
            TItem? item,
            int itemCount
            );

        protected abstract TLargeReadOnlyItemContainer CreateLargeReadOnlyItemContainer();
        protected abstract TLargeReadOnlyItemContainer CreateLargeReadOnlyItemContainer(
            TItem? item,
            long itemCount
            );

        protected abstract TReadOnlyItemContainer ConvertLargeToNormalReadOnlyContainer(
    TLargeReadOnlyItemContainer largeContainer
    );

        protected abstract TContainerAddEvent CreateContainerAddEvent(
            int id,
            TItemContainer container
            );

        protected abstract TContainerRemoveEvent CreateContainerRemoveEvent(
            int id,
            TItemContainer container
            );

        protected abstract TContainerReplaceEvent CreateContainerReplaceEvent(
            int id,
            TItemContainer oldContainer,
            TItemContainer newContainer
            );

        protected abstract TItem? GetItemFromReadOnlyContainer(TLargeReadOnlyItemContainer largeReadOnlyContainer);

        protected abstract long GetItemCountFromReadOnlytContainer(TLargeReadOnlyItemContainer largeReadOnlyContainer);

        protected virtual int ResolveID()
        {
            var r = new Random();

            int id;

            do
            {
                id = r.Next();
            }
            while (containers.ContainsKey(id));

            return id;
        }

        protected virtual void InstantiateContainersCore(
            int count,
            IList<TItemContainer>? results,
            TItemContainer? cloneSample = null
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
                var cloned = (TItemContainer)cloneSample.ShallowClone();
                cloned.Clear();

                AddContainer(cloned);

                if (collectResults)
                    results!.Add(cloned);
            }
        }

        protected virtual void RemoveCountCore(int removeCount, IList<TItemContainer>? removed)
        {
            if (removeCount <= 0)
                return;

            bool collectRemoved = removed is not null;

            bool removeContainer(TItemContainer container)
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

        protected virtual void OnContainerItemChanged((TItem? Previous, TItem? Current) items, TItemContainer cnt)
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

        protected virtual void OnContainerAdd(DictionaryAddEvent<int, TItemContainer> addEv)
        {
            TItemContainer? container = addEv.Value;

            BindContainerItemCount(container);
            BindContainerItem(container);
            ResolveOccupied(container);

            if (!EqualityComparer<IInventory?>.Default.Equals((IInventory)this, container.ParentInventory))
            {
                container.ID = addEv.Key;
                container.SetParentInventory((IInventory)this);
            }

            FreeSpace += container.FreeSpace;
        }

        protected virtual void OnContainerRemove(DictionaryRemoveEvent<int, TItemContainer> removeEv)
        {
            TItemContainer container = removeEv.Value;

            if (containerDisposables.TryGetValue(container, out var disposables))
                disposables.Dispose();

            FreeSpace = Math.Clamp(FreeSpace - container.FreeSpace, 0, int.MaxValue);
            itemCount.Value = Math.Max(itemCount.Value - container.ItemCount, 0);

            if (!container.IsEmpty
                &&
                container.Item.Is<TItem>(out var item)
                &&
                occupiedContainers.TryGetValue(item, out var cnts))
            {
                cnts.Remove(container);
            }
        }

        protected virtual void OnContainerReplace(DictionaryReplaceEvent<int, TItemContainer> replaceEv)
        {
            var id = replaceEv.Key;
            var oldCnt = replaceEv.OldValue;
            var newCnt = replaceEv.NewValue;

            var removeEv = new DictionaryRemoveEvent<int, TItemContainer>(id, oldCnt);
            OnContainerRemove(removeEv);

            var addEv = new DictionaryAddEvent<int, TItemContainer>(id, newCnt);
            OnContainerAdd(addEv);
        }

        protected virtual void OnContainersClear(Unit _)
        {
            occupiedContainers.Clear();
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

        private void BindContainerItemCount(TItemContainer cnt)
        {
            var disposables = containerDisposables.GetOrCreateNew(cnt);

            cnt.ObserveItemCount()
                .Pairwise()
                .SelectDelta()
                .Subscribe(OnContainerItemCountChanged)
                .AddTo(disposables);
        }

        private void ResolveOccupied(TItemContainer cnt)
        {
            if (cnt.IsEmpty || !cnt.Item.Is<TItem>(out var item))
                return;

            occupiedContainers.GetOrCreateNew(item).Add(cnt);
        }

        private void OnContainerItemCountChanged(int itemCountDelta)
        {
            itemCount.Value = Math.Clamp(itemCount.Value + itemCountDelta, 0, int.MaxValue);
        }

        private void BindContainerItem(TItemContainer cnt)
        {
            var disposables = containerDisposables.GetOrCreateNew(cnt);

            cnt.ObserveItem()!
                .Cast<IItem, TItem>()
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
    }
}
