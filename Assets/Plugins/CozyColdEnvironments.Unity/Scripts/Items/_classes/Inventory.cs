using CCEnvs.Collections;
using CCEnvs.Diagnostics;
using CCEnvs.FuncLanguage;
using CCEnvs.Linq;
using Cysharp.Threading.Tasks;
using ObservableCollections;
using R3;
using SuperLinq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using ZLinq;

#pragma warning disable S3236
#nullable enable
namespace CCEnvs.Unity.Items
{
    public class Inventory : IInventory, IDisposable
    {
        private readonly ObservableDictionary<int, IItemContainer> containers = new();

        private readonly Dictionary<IItem, List<IItemContainer>> occupiedContainers = new();
        private readonly Dictionary<IItemContainer, CompositeDisposable> containerDisposables = new();

        private readonly ReactiveProperty<int> itemCount = new();

        private IDisposable? containerAddBinding;
        private IDisposable? containerRemoveBinding;
        private IDisposable? containerReplaceBinding;

        public IItemContainer this[int id] => containers[id];

        public bool IsEmpty => ItemCount <= 0;
        public bool IsFull => FreeSpace <= 0;
        public bool AutoSize { get; set; }

        public int FreeSpace { get; private set; }
        public int ContainerCount => containers.Count;
        public int ItemCount => itemCount.Value;

        public IEnumerable<int> IDs => containers.SelectKey();

        public IEnumerable<IItemContainer> Containers => containers.SelectValue();

        public IReadOnlyObservableDictionary<int, IItemContainer> ContainersRX => containers;

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



            int countSum = 0;

            foreach (var (_, cnt) in containers)
            {
                if (cnt.ContainsItem(item))
                    countSum += cnt.ItemCount;

                if (countSum >= count)
                    return true;
            }

            return false;
        }

        public bool TryGetContainer(int id, [NotNullWhen(true)] out IItemContainer? container)
        {
            return containers.TryGetValue(id, out container);
        }

        public void ResetContainers()
        {
            foreach (var (_, cnt) in containers)
                cnt.Reset();
        }

        public Maybe<IItemContainer> PutItem(IItem? item, int count = 1)
        {
            if (item.IsNull() || count <= 0)
            {
                if (CCDebug.Instance.IsEnabled)
                    this.PrintLog($"Item: {item.Maybe().Map(x => x!.ToString()).GetValue("null")}, count: {count}. Is not added.");

                return Maybe<IItemContainer>.None;
            }

            Maybe<IItemContainer> restItemsMaybe = Maybe<IItemContainer>.None;
            IItemContainer? restItems;

            var loopFuse = LoopFuse.Create();

            while (restItemsMaybe.IsSome)
            {
                if (!loopFuse.MoveNext())
                    return restItemsMaybe;

                foreach (var cnt in FilterContainersWithItem(item, ignoreFull: true).Concat(FilterEmptyContainers()))
                {
                    restItemsMaybe = cnt.PutItem(item, count);

                    if (!restItemsMaybe.TryGetValue(out restItems))
                        return Maybe<IItemContainer>.None;

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

        public Maybe<IItemContainer> PutItemFrom(IItemContainer itemContainer, int count)
        {
            CC.Guard.IsNotNull(itemContainer, nameof(itemContainer));

            if (count <= 0 || itemContainer.IsEmpty)
                return Maybe<IItemContainer>.None;

            return PutItem(itemContainer.Item.GetValue(), count);
        }
        public Maybe<IItemContainer> PutItemFrom(IItemContainer itemContainer)
        {
            CC.Guard.IsNotNull(itemContainer, nameof(itemContainer));

            if (itemContainer.IsEmpty)
                return Maybe<IItemContainer>.None;

            return PutItem(itemContainer.Item.GetValue(), itemContainer.ItemCount);
        }

        public Maybe<IItemContainer> TakeItem(IItem item, int count)
        {
            CC.Guard.IsNotNull(item, nameof(item));
            if (count <= 0)
                return null!;

            Maybe<IItemContainer> taked;
            foreach (var cnt in containers
#if ZLINQ_PLUGIN
                .AsValueEnumerable()
#endif
                .Select(x => x.Value)
                .Where(x => x.ContainsItem(item)))
            {
                taked = cnt.TakeItem(count);
                count -= taked.GetValueUnsafe().ItemCount;

                if (count <= 0)
                    break;
            }

            return default!;
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

            var id = ResolveID(cnt);
            AddContainer(ResolveID(cnt), cnt);
        }

        internal void AddContainer(int id, IItemContainer cnt)
        {
            containers[id] = cnt;
        }

        public bool RemoveContainer(int id)
        {
            return containers.Remove(id);
        }

        public IList<IItemContainer> InstantiateContainers(
            int count,
            IItemContainer? cloneExmaple = null
            ) 
        {
            count = Math.Max(count, 0);

            cloneExmaple ??= containers.FirstOrDefault().Value ?? new ItemContainer();

            IItemContainer cloned;

            var results = new IItemContainer[count];

            for (int i = 0; i < count; i++)
            {
                cloned = cloneExmaple.ShallowClone();
                cloned.Reset();

                AddContainer(cloned);

                results[i] = cloned;
            }

            return results;
        }

        public IList<IItemContainer> SetContainerCount(
            int count, 
            IItemContainer? cloneExample = null
            )
        {
            count = Math.Max(count, 0);
            int delta = count - ContainerCount;

            if (delta < 0)
                return RemoveCount(delta);
            else if (delta > 0)
                return InstantiateContainers(delta, cloneExample);
            else
                return Array.Empty<IItemContainer>();
        }

        public IList<IItemContainer> RemoveCount(int removeCount)
        {
            var removed = new List<IItemContainer>(removeCount);

            IItemContainer? cnt;

            var emptyContainers = FilterEmptyContainers().ToList();

            for (int i = 0; i < removeCount; i++)
            {
                if (emptyContainers.IsEmpty())
                    cnt = emptyContainers.FirstOrDefault();
                else
                {
                    cnt = emptyContainers[^1];
                    emptyContainers.RemoveAt(emptyContainers.Count - 1);
                }

                if (cnt.IsNull() || !cnt.GetContainerID().TryGetValue(out var cntID))
                    return removed;

                if (RemoveContainer(cntID))
                    removeCount--;

                if (removeCount <= 0)
                    return removed;
            }

            return removed;
        }

        public bool CanPut() => !IsFull;

        public bool CanPut(IItem? item)
        {
            if (item.IsNull())
                return false;

            foreach (var (_, cnt) in containers)
            {
                if (cnt.IsEmpty || (cnt.CanPut(item) && !cnt.IsFull))
                    return true;
            }

            return false;
        }
        public bool CanPut(IItem? item, int count)
        {
            if (!CanPut(item) || (FreeSpace <= 0 && AutoSize))
                return false;

            if (count <= 0)
                return CanPut(item);

            int freeSpace = 0;
            foreach (var (_, cnt) in containers)
            {
                if (cnt.IsEmpty || cnt.ContainsItem(item))
                    freeSpace += cnt.FreeSpace;

                if (freeSpace >= count)
                    return true;
            }

            return false;
        }

        public void Reset() => containers.Clear();

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

                containers?.Clear();
                occupiedContainers?.Clear();
            }
            catch (Exception ex)
            {
                this.PrintException(ex);
            }
        }

        public Observable<int> ObserveItemCount() => itemCount;

        public IEnumerator<KeyValuePair<int, IItemContainer>> GetEnumerator()
        {
            return containers.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        protected virtual int ResolveID(IItemContainer itemContainer)
        {
            var ids = new List<int>(containers.Count);

            foreach (var (cntID, _) in containers)
                ids.Add(cntID);

            if (Do.TryFindHoleInRange(start: 0, ContainerCount, ids, out int hole))
                return hole;

            return ContainerCount;
        }

        private void BindContainerAdd()
        {
            containerAddBinding = containers.ObserveDictionaryAdd()
                .Subscribe(OnContainerAdd);
        }

        private void OnContainerAdd(DictionaryAddEvent<int, IItemContainer> addEv)
        {
            var cnt = addEv.Value;

            BindContainerItemCount(cnt);
            AddFreeSpace(cnt);
            ResolveOccupied(cnt);
        }

        private void BindContainerItemCount(IItemContainer cnt)
        {
            var disposables = containerDisposables.GetOrCreateNew(cnt);

            cnt.ObserveItemCount()
                .Pairwise()
                .Select(pair => pair.Current - pair.Previous)
                .Subscribe(OnContainerItemCount)
                .AddTo(disposables);
        }

        private void AddFreeSpace(IItemContainer cnt)
        {
            FreeSpace += cnt.FreeSpace;
        }

        private void ResolveOccupied(IItemContainer cnt)
        {
            if (cnt.IsEmpty || !cnt.Item.TryGetValue(out var item))
                return;

            occupiedContainers.GetOrCreateNew(item).Add(cnt);
        }

        private void OnContainerItemCount(int itemCountDelta)
        {
            itemCount.Value = Math.Clamp(itemCount.Value + itemCountDelta, 0, int.MaxValue);
        }

        private void BindContainerItem(IItemContainer cnt)
        {
            var disposables = containerDisposables.GetOrCreateNew(cnt);

            cnt.ObserveItem()
                .Select(x => x.GetValue())
                .Pairwise()
                .Subscribe(cnt, OnContainerItem)
                .AddTo(disposables);
        }

        private void OnContainerItem((IItem? Previous, IItem? Current) items, IItemContainer cnt)
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

        private void BindContainerRemove()
        {
           containerRemoveBinding = containers.ObserveDictionaryRemove()
                .Subscribe(OnContainerRemove);
        }

        private void OnContainerRemove(DictionaryRemoveEvent<int, IItemContainer> removeEv)
        {
            var cnt = removeEv.Value;

            if (containerDisposables.TryGetValue(cnt, out var disposables))
                disposables.Dispose();

            if (!cnt.IsEmpty
                &&
                cnt.Item.TryGetValue(out var item)
                &&
                occupiedContainers.TryGetValue(item, out var cnts))
            {
                cnts.Remove(cnt);
            }
        }

        private void BindContainerReplace()
        {
            containerReplaceBinding = containers.ObserveDictionaryReplace()
                .Subscribe(OnContainerReplace);
        }

        private void OnContainerReplace(DictionaryReplaceEvent<int, IItemContainer> replaceEv)
        {
            var id = replaceEv.Key;
            var oldCnt = replaceEv.OldValue;
            var newCnt = replaceEv.NewValue;

            var removeEv = new DictionaryRemoveEvent<int, IItemContainer>(id, oldCnt);
            OnContainerRemove(removeEv);

            var addEv = new DictionaryAddEvent<int, IItemContainer>(id, newCnt);
            OnContainerAdd(addEv);
        }

        Maybe<IItemContainer> IItemAccessor.TakeItem(int count) => Maybe<IItemContainer>.None;

        Maybe<IItemContainer> IItemAccessor.TakeItem() => Maybe<IItemContainer>.None;

        Maybe<int> IItemContainerInfoItemless.GetContainerID() => Maybe<int>.None;

        void IItemAccessor.CopyFrom(IItemContainerInfo itemContainer)
        {
            throw new NotImplementedException();
        }
    }
}
