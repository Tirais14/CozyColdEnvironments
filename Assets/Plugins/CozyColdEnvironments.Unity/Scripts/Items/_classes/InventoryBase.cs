using CCEnvs.Collections;
using CCEnvs.FuncLanguage;
using CCEnvs.Linq;
using CCEnvs.Rx;
using CCEnvs.Threading;
using ObservableCollections;
using R3;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;

#nullable enable
namespace CCEnvs.UnityX.Items
{
    public abstract class InventoryBase<TItem, TItemContainer, TItemContainerInfo, TDefaultItemContainer> 
        :
        IInventoryBase<TItem, TItemContainer, TItemContainerInfo>,
        IItemContainerInfoItemless<TItem>,
        IDisposable

        where TItem : IItem
        where TItemContainer : IItemContainer<TItem, TItemContainerInfo>
        where TItemContainerInfo : IItemContainerInfo<TItem>
        where TDefaultItemContainer : TItemContainer, TItemContainerInfo, new()
    {
        private readonly ObservableDictionary<int, TItemContainer> containers = new();

        private readonly Dictionary<IItem, List<TItemContainer>> occupiedContainers = new();
        private readonly Dictionary<TItemContainer, CompositeDisposable> containerDisposables = new();
        private readonly Dictionary<TItemContainer, int> containerIDs = new();

        private readonly ReactiveProperty<int> itemCount = new();

        private readonly CancellationTokenSource disposeCancellationTokenSource = new();

        private IDisposable? containerAddBinding;
        private IDisposable? containerRemoveBinding;
        private IDisposable? containerReplaceBinding;
        private IDisposable? containersClearBinding;

        public TItemContainer this[int id] => containers[id];

        public bool IsEmpty => ItemCount <= 0;
        public bool IsFull => FreeSpace <= 0;
        public bool AutoSize { get; set; }

        public int FreeSpace { get; private set; }
        public int ContainerCount => containers.Count;
        public int ItemCount => itemCount.Value;

        public IReadOnlyObservableDictionary<int, TItemContainer> Containers => containers;

        protected CancellationToken DisposeCancellationToken => disposeCancellationTokenSource.Token;

        Maybe<IInventory> IItemContainerInfoItemless.ParentInventory { get => null!; set => _ = value; }

        int IItemContainerInfoItemless.Capacity {
            get => ContainerCount;
            set => SetContainerCount(value);
        }

        public InventoryBase()
        {
            BindContainerAdd();
            BindContainerRemove();
            BindContainerReplace();
            BindContainersClear();
        }

        public InventoryBase(IEqualityComparer<int> comparer)
            :
            this()
        {
            containers = new ObservableDictionary<int, TItemContainer>(comparer);
        }

        public InventoryBase(Dictionary<int, TItemContainer> innerDictionary)
            :
            this()
        {
            containers = new ObservableDictionary<int, TItemContainer>(innerDictionary);
        }

        public InventoryBase(IEnumerable<KeyValuePair<int, TItemContainer>> values)
            :
            this(new Dictionary<int, TItemContainer>(values))
        {
        }

        public InventoryBase(int containerCount, TItemContainer? cloneExample = default)
            :
            this(new Dictionary<int, TItemContainer>())
        {
            SetContainerCount(containerCount, cloneExample);
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

        public Maybe<TItemContainerInfo> PutItem(TItem? item, int count = 1)
        {
            if (item.IsNull() || count <= 0)
                return Maybe<TItemContainerInfo>.None;

            var restItemsMaybe = Maybe<TItemContainerInfo>.None;
            TItemContainerInfo? restItems;

            var loopFuse = LoopFuse.Create();

            while (restItemsMaybe.IsSome)
            {
                if (!loopFuse.MoveNext())
                    return restItemsMaybe;

                foreach (var cnt in FilterContainersWithItem(item, ignoreFull: true).Concat(FilterEmptyContainers()))
                {
                    restItemsMaybe = cnt.PutItem(item, count);

                    if (!restItemsMaybe.TryGetValue(out restItems) || restItems.IsEmpty)
                        return Maybe<TItemContainerInfo>.None;

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

        public Maybe<TItemContainerInfo> PutItemFrom(IItemContainer<TItem> itemContainer, int count)
        {
            CC.Guard.IsNotNull(itemContainer, nameof(itemContainer));

            if (count <= 0 || itemContainer.IsEmpty)
                return Maybe<TItemContainerInfo>.None;

            return PutItem(itemContainer.Item.GetValue(), count);
        }
        public Maybe<TItemContainerInfo> PutItemFrom(IItemContainer<TItem> itemContainer)
        {
            CC.Guard.IsNotNull(itemContainer, nameof(itemContainer));

            if (itemContainer.IsEmpty)
                return Maybe<TItemContainerInfo>.None;

            return PutItem(itemContainer.Item.GetValue(), itemContainer.ItemCount);
        }

        public Maybe<TItemContainerInfo> TakeItem(TItem item, int count)
        {
            CC.Guard.IsNotNull(item, nameof(item));

            count = Math.Max(count, 0);

            if (count <= 0
                ||
                !occupiedContainers.TryGetValue(item, out var cnts)
                ||
                GetItemCount(item) < count)
            {
                return Maybe<TItemContainerInfo>.None;
            }

            var items = new TDefaultItemContainer
            {
                Capacity = int.MaxValue
            };

            items.PutItem(item, count);

            foreach (var cnt in cnts)
            {
                if (items.ItemCount <= count)
                    break;

                items.PutItemFrom(cnt);
            }

            if (items.IsEmpty)
                return Maybe<TItemContainerInfo>.None;

            return items;
        }

        public void EnsureFreeSpace(
            int tragetSpace,
            TItem? forItem = default,
            TItemContainer? cloneExample = default
            )
        {
            var loopFuse = LoopFuse.Create(15000);

            while (GetFreeSpace(forItem) < tragetSpace && loopFuse.MoveNext())
                InstantiateContainers(1, cloneExample);
        }

        public int GetFreeSpace(TItem? item)
        {
            if (item.IsNull())
                return FreeSpace;

            int freeSpace = 0;

            foreach (var cnt in FilterContainersWithItem(item, ignoreFull: true))
                freeSpace += cnt.FreeSpace;

            return freeSpace;
        }

        public int GetItemCount(TItem? item)
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

        public IEnumerable<TItemContainer> FilterContainersWithItem(
            TItem? item,
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

        public IEnumerable<TItemContainer> FilterEmptyContainers()
        {
            foreach (var (_, cnt) in containers)
            {
                if (!cnt.IsEmpty)
                    continue;

                yield return cnt;
            }
        }

        public void AddContainer(TItemContainer cnt)
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
        public bool RemoveContainer(IItemContainerInfo container)
        {
            if (!container.GetContainerID().TryGetValue(out int cntID))
                return false;

            return RemoveContainer(cntID);
        }

        public void InstantiateContainers(
            int count,
            TItemContainer? cloneExmaple = default
            )
        {
            InstantiateContainersCore(count, results: null, cloneExmaple);
        }

        public void InstantiateContainers(
            int count,
            out IList<TItemContainer> results,
            TItemContainer? cloneExmaple = default
            )
        {
            results = new List<TItemContainer>(count);
            InstantiateContainersCore(count, (List<TItemContainer>)results, cloneExmaple);
        }

        public void SetContainerCount(
            int count,
            TItemContainer? cloneExample = default
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
            out IList<TItemContainer> changed,
            TItemContainer? cloneExample = default
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

            changed = Array.Empty<TItemContainer>();
        }

        public void RemoveCount(int removeCount)
        {
            RemoveCountCore(removeCount, null);
        }
        public void RemoveCount(int removeCount, out IList<TItemContainer> removed)
        {
            removed = new List<TItemContainer>(removeCount);
            RemoveCountCore(removeCount, (List<TItemContainer>)removed);
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

        public Maybe<int> GetContainerID(TItemContainer cnt)
        {
            CC.Guard.IsNotNull(cnt, nameof(cnt));

            if (!containerIDs.TryGetValue(cnt, out var id))
                return Maybe<int>.None;

            return id;
        }

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

        public Observable<int> ObserveItemCount() => itemCount;

        public IEnumerator<TItemContainer> GetEnumerator()
        {
            return containers.SelectValue().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        protected virtual int ResolveID(TItemContainer itemContainer)
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
            List<TItemContainer>? results,
            TItemContainer? cloneExmaple = default
            )
        {
            if (count <= 0)
                return;

            count = Math.Max(count, 0);

            cloneExmaple ??= containers.FirstOrDefault().Value ?? new TDefaultItemContainer();

            if (cloneExmaple.IsReadOnlyContainer)
                throw new ArgumentException($"Item container cannot be readonly. Container: {cloneExmaple}");

            TItemContainer cloned;
            bool collectResults = results is not null;

            for (int i = 0; i < count; i++)
            {
                cloned = cloneExmaple.ShallowClone().CastTo<TItemContainer>();
                cloned.Clear();

                AddContainer(cloned);

                if (collectResults)
                    results!.Add(cloned);
            }
        }

        protected virtual void RemoveCountCore(int removeCount, List<TItemContainer>? removed)
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

        protected virtual void OnContainerAdd(DictionaryAddEvent<int, TItemContainer> addEv)
        {
            var id = addEv.Key;
            var cnt = addEv.Value;

            BindContainerItemCount(cnt);
            ResolveOccupied(cnt);

            FreeSpace += cnt.FreeSpace;
            containerIDs[cnt] = id;
        }

        protected virtual void OnContainerItemChanged(
            (TItem? Previous, TItem? Current) items,
            TItemContainer cnt
            )
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

        protected virtual void OnContainerRemove(DictionaryRemoveEvent<int, TItemContainer> removeEv)
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
            if (cnt.IsEmpty || !cnt.Item.TryGetValue(out var item))
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

        Maybe<TItemContainerInfo> IItemAccessor<TItem, TItemContainerInfo>.TakeItem(int count) => Maybe<TItemContainerInfo>.None;

        Maybe<TItemContainerInfo> IItemAccessor<TItem, TItemContainerInfo>.TakeItem() => Maybe<TItemContainerInfo>.None;

        Maybe<int> IItemContainerInfoItemless.GetContainerID() => Maybe<int>.None;

        void IItemAccessor<TItem, TItemContainerInfo>.CopyItemFrom(IItemContainerInfo<TItem> itemContainer)
        {
            throw new NotImplementedException();
        }
    }
}
