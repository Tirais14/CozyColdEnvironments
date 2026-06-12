using CCEnvs.Collections;
using CCEnvs.FuncLanguage;
using CCEnvs.Linq;
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
using UnityEditor.Localization.Plugins.XLIFF.V12;
using ZLinq;
using static UnityEditor.Progress;

#pragma warning disable S3236
#nullable enable
namespace CCEnvs.UnityX.Items
{
    public class Inventory : IInventory, IDisposable
    {
        private readonly ObservableDictionary<int, IItemContainer> containers = new();

        private readonly Dictionary<IItem, List<IItemContainer>> occupiedContainers = new();
        private readonly Dictionary<IItemContainer, CompositeDisposable> containerDisposables = new();
        private readonly Dictionary<IItemContainer, int> containerIDs = new();

        private readonly ReactiveProperty<int> itemCount = new();

        private readonly CancellationTokenSource disposeCancellationTokenSource = new();

        private IDisposable? containerAddBinding;
        private IDisposable? containerRemoveBinding;
        private IDisposable? containerReplaceBinding;
        private IDisposable? containersClearBinding;

        public IItemContainer this[int id] => containers[id];

        public bool IsEmpty => ItemCount <= 0;
        public bool IsFull => FreeSpace <= 0;
        public bool AutoSize { get; set; }

        public int FreeSpace { get; private set; }
        public int ContainerCount => containers.Count;
        public int ItemCount => itemCount.Value;

        public IReadOnlyDictionary<int, IItemContainer> Containers => containers;

        protected CancellationToken DisposeCancellationToken => disposeCancellationTokenSource.Token;

        Maybe<IInventory> IItemContainerInfoItemless.ParentInventory { get => null!; set => _ = value; }

        int IItemContainerInfoItemless.Capacity {
            get => ContainerCount;
            set => SetContainerCount(value);
        }

        public Inventory()
        {
            BindContainerAdd();
            BindContainerRemove();
            BindContainerReplace();
            BindContainersClear();
        }

        public Inventory(IEqualityComparer<int> comparer)
            :
            this()
        {
            containers = new ObservableDictionary<int, IItemContainer>(comparer);
        }

        public Inventory(Dictionary<int, IItemContainer> innerDictionary)
            :
            this()
        {
            containers = new ObservableDictionary<int, IItemContainer>(innerDictionary);
        }

        public Inventory(IEnumerable<KeyValuePair<int, IItemContainer>> values)
            :
            this(new Dictionary<int, IItemContainer>(values))
        {
        }

        public Inventory(int containerCount, IItemContainer? cloneExample = null)
            :
            this(new Dictionary<int, IItemContainer>())
        {
            SetContainerCount(containerCount, cloneExample);
        }

        public static Inventory CreateWith<TItemContainer>(int containerCount)
            where TItemContainer : IItemContainer, new()
        {
            var inventory = new Inventory(containerCount);

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

        public Maybe<ReadOnlyItemContainer> PutItem(IItem? item, int count = 1)
        {
            if (item.IsNull() || count <= 0)
                return Maybe<ReadOnlyItemContainer>.None;

            var restItemsMaybe = Maybe<ReadOnlyItemContainer>.None;
            ReadOnlyItemContainer restItems;

#if CC_DEBUG_ENABLED
            var loopFuse = LoopFuse.Create();
#endif

            while (restItemsMaybe.IsSome)
            {
#if CC_DEBUG_ENABLED
                if (!loopFuse.MoveNext())
                    return restItemsMaybe;
#endif

                foreach (var cnt in FilterContainersWithItem(item, ignoreFull: true).Concat(FilterEmptyContainers()))
                {
                    restItemsMaybe = cnt.PutItem(item, count);

                    if (!restItemsMaybe.TryGetValue(out restItems) || restItems.IsEmpty)
                        return Maybe<ReadOnlyItemContainer>.None;

                    count = restItems.ItemCount;
                }

                if (!AutoSize
                    ||
                    !restItemsMaybe.TryGetValue(out restItems)
                    ||
                    restItems.IsEmpty)
                {
                    break;
                }
            }

            return restItemsMaybe;
        }

        public Maybe<ReadOnlyItemContainer> PutItemFrom(IItemContainer itemContainer, int count)
        {
            CC.Guard.IsNotNull(itemContainer, nameof(itemContainer));

            if (count <= 0 || itemContainer.IsEmpty
                ||
                !itemContainer.TakeItem(count).TryGetValue(out ReadOnlyItemContainer takedItems)
                ||
                !PutItem(takedItems.Item.GetValue(), takedItems.ItemCount).TryGetValue(out ReadOnlyItemContainer restItems))
            {
                return Maybe<ReadOnlyItemContainer>.None;
            }

            return restItems.Maybe();
        }
        public Maybe<ReadOnlyItemContainer> PutItemFrom(IItemContainer itemContainer)
        {
            CC.Guard.IsNotNull(itemContainer, nameof(itemContainer));

            if (itemContainer.IsEmpty
                ||
                !itemContainer.TakeItem(itemContainer.ItemCount).TryGetValue(out ReadOnlyItemContainer takedItems)
                ||
                !PutItem(takedItems.Item.GetValue(), takedItems.ItemCount).TryGetValue(out ReadOnlyItemContainer restItems))
            {
                return Maybe<ReadOnlyItemContainer>.None;
            }

            return restItems.Maybe();
        }

        public Maybe<ReadOnlyItemContainer> TakeItem(IItem item, int count)
        {
            CC.Guard.IsNotNull(item, nameof(item));

            if (count <= 0
                ||
                !occupiedContainers.TryGetValue(item, out var cnts)
                ||
                GetItemCount(item) < count)
            {
                return Maybe<ReadOnlyItemContainer>.None;
            }

            int takedCount = 0;

            foreach (var cnt in cnts)
            {
                int toTakeCount = Math.Clamp(count, 0, cnt.ItemCount);

                if (!cnt.TakeItem(toTakeCount).TryGetValue(out ReadOnlyItemContainer takedItems))
                    continue;

                takedCount += takedItems.ItemCount;
                count -= takedItems.ItemCount;

                if (count <= 0)
                    break;
            }

            if (takedCount <= 0)
                return Maybe<ReadOnlyItemContainer>.None;

            return new ReadOnlyItemContainer(item, takedCount);
        }

        public void EnsureFreeSpace(
            int tragetSpace,
            IItem? forItem = null,
            IItemContainer? cloneExample = null
            )
        {
            var loopFuse = LoopFuse.Create(15000);

            while (GetFreeSpace(forItem) < tragetSpace && loopFuse.MoveNext())
                InstantiateContainers(1, cloneExample);
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
                occupiedContainers.TryGetValue(item, out var cnts))
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
            if (item.IsNull())
            {
                foreach (var emptyCnt in FilterEmptyContainers())
                    yield return emptyCnt;

                yield break;
            }

            if (!occupiedContainers.TryGetValue(item, out var cnts))
                yield break;

            foreach (var cnt in occupiedContainers[item])
            {
                if (ignoreFull && cnt.IsFull)
                    continue;

                yield return cnt;
            }
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

        public void AddContainer(IItemContainer cnt)
        {
            CC.Guard.IsNotNull(cnt, nameof(cnt));

            if (cnt.IsReadOnlyContainer)
                throw new ArgumentException($"Container cannot be readonly. Container: {cnt}");

            var id = ResolveID(cnt);

            containers[id] = cnt;
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
            IItemContainer? cloneExmaple = null
            )
        {
            if (count <= 0)
                return;

            cloneExmaple ??= containers.FirstOrDefault().Value ?? new ItemContainer();

            if (cloneExmaple.IsReadOnlyContainer)
                throw new ArgumentException($"Item container cannot be readonly. Container: {cloneExmaple}");

            IItemContainer cloned;
            bool collectResults = results is not null;

            for (int i = 0; i < count; i++)
            {
                cloned = cloneExmaple.ShallowClone();
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
            var id = addEv.Key;
            var cnt = addEv.Value;

            BindContainerItemCount(cnt);
            ResolveOccupied(cnt);

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

        Maybe<ReadOnlyItemContainer> IItemAccessor.TakeItem(int count) => Maybe<ReadOnlyItemContainer>.None;
        Maybe<ReadOnlyItemContainer> IItemAccessor.TakeItem() => Maybe<ReadOnlyItemContainer>.None;

        Maybe<int> IItemContainerInfoItemless.GetContainerID() => Maybe<int>.None;
    }

    public class Inventory<TItem, TItemContainer, TItemContainerInfo>
        :
        IInventory<TItem, TItemContainer>,
        IDisposable

        where TItem : IItem
        where TItemContainer : IItemContainer, TItemContainerInfo, new()
        where TItemContainerInfo : IItemContainerInfo
    {
        private readonly Inventory internalInventory;

        private readonly DictionaryView<int, IItemContainer, TItemContainer> containersView;

        public Inventory()
        {
            internalInventory = new Inventory();

            containersView = new DictionaryView<int, IItemContainer, TItemContainer>(
                 internalInventory.Containers,
                 static (value) => (TItemContainer)value
                 );
        }

        public Inventory(IEqualityComparer<int> comparer)
        {
            internalInventory = new Inventory(comparer);

            containersView = new DictionaryView<int, IItemContainer, TItemContainer>(
                 internalInventory.Containers,
                 static (value) => (TItemContainer)value
                 );
        }

        public Inventory(Dictionary<int, IItemContainer> innerDictionary)
        {
            internalInventory = new Inventory(innerDictionary);

            containersView = new DictionaryView<int, IItemContainer, TItemContainer>(
                 internalInventory.Containers,
                 static (value) => (TItemContainer)value
                 );
        }

        public Inventory(IEnumerable<KeyValuePair<int, IItemContainer>> values)
            :
            this(new Dictionary<int, IItemContainer>(values))
        {
        }

        public Inventory(int containerCount, TItemContainer? cloneExample = default)
            :
            this(new Dictionary<int, IItemContainer>())
        {
            SetContainerCount(containerCount, cloneExample);
        }

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
        public int ItemCount => internalInventory.ItemCount;
        public int FreeSpace => internalInventory.FreeSpace;

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

        public void AddContainer(TItemContainer itemContainer)
        {
            internalInventory.AddContainer(itemContainer);
        }

        public bool CanPut() => internalInventory.CanPut();
        public bool CanPut(IItem? item) => CanPut(item);
        public bool CanPut(IItem? item, int count) => CanPut(item, count);

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

            results = untpyedResults.Cast<TItemContainer>().ToArray();
        }

        public Maybe<ReadOnlyItemContainer<TItem>> PutItem(TItem? item, int count = 1)
        {
            if (!internalInventory.PutItem(item, count).TryGetValue(out ReadOnlyItemContainer untypedRestItems))
                return default;

            return untypedRestItems.Convert<TItem>();
        }

        public Maybe<ReadOnlyItemContainer<TItem>> PutItemFrom(IItemContainer<TItem> itemContainer, int count)
        {
            if (!internalInventory.PutItemFrom(itemContainer, count).TryGetValue(out ReadOnlyItemContainer untypedRestItems))
                return default;

            return untypedRestItems.Convert<TItem>();
        }
        public Maybe<ReadOnlyItemContainer<TItem>> PutItemFrom(IItemContainer<TItem> itemContainer)
        {
            if (!internalInventory.PutItemFrom(itemContainer).TryGetValue(out ReadOnlyItemContainer untypedRestItems))
                return default;

            return untypedRestItems.Convert<TItem>();
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

            changed = untypedChanged.Cast<TItemContainer>().ToArray();
        }

        public Maybe<ReadOnlyItemContainer<TItem>> TakeItem(TItem item, int count)
        {
            if (!internalInventory.TakeItem(item, count).TryGetValue(out ReadOnlyItemContainer untypedTakedItems))
                return default;

            return untypedTakedItems.Convert<TItem>();
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

        Maybe<ReadOnlyItemContainer<TItem>> IItemAccessor<TItem>.TakeItem() => default;
        Maybe<ReadOnlyItemContainer<TItem>> IItemAccessor<TItem>.TakeItem(int count) => default;

        Maybe<int> IItemContainerInfoItemless.GetContainerID() => Maybe<int>.None;
    }
}
