//#nullable enable
//using CCEnvs.Collections;
//using CCEnvs.FuncLanguage;
//using CCEnvs.Linq;
//using CCEnvs.Pools;
//using CCEnvs.Rx;
//using CCEnvs.TypeMatching;
//using ObservableCollections;
//using R3;
//using System;
//using System.Collections;
//using System.Collections.Generic;
//using System.Diagnostics.CodeAnalysis;
//using System.Linq;
//using System.Threading;
//using UnityEngine.Experimental.GlobalIllumination;
//using static UnityEditor.Progress;

//namespace CCEnvs.UnityX.Items
//{
//    public static class InventoryMethods
//    {
//        public static void SetupInventory()
//        {
//            BindContainerAdd();
//            BindContainerRemove();
//            BindContainerReplace();
//            BindContainersClear();
//        }

//        public static bool ContainsItem<TItem, TItemContainer>(IReadOnlyDictionary<TItem, List<TItemContainer>> occupiedContainers)
//            where TItem : IItem
//            where TItemContainer : IItemContainer
//        {
//            return occupiedContainers.Count != 0;
//        }
//        public static bool ContainsItem<TItem, TItemContainer>(
//            IReadOnlyDictionary<TItem, List<TItemContainer>> occupiedContainers,
//            TItem? item
//            )
//            where TItem : IItem
//            where TItemContainer : IItemContainer
//        {
//            if (item.IsNull())
//                return ContainsItem(occupiedContainers);

//            return occupiedContainers.ContainsKey(item);
//        }
//        public static bool ContainsItem<TItem, TItemContainer>(
//            int allItemCount,
//            IReadOnlyDictionary<TItem, List<TItemContainer>> occupiedContainers,
//            TItem? item,
//            int count
//            )
//            where TItem : IItem
//            where TItemContainer : IItemContainer
//        {
//            if (occupiedContainers.Count == 0)
//                return false;

//            return GetItemCount(occupiedContainers, item) >= count;
//        }

//        public static ReadOnlyItemContainer PutItem<TItem, TItemContainer>(
//            IInventory self,
//            IDictionary<int, TItemContainer> containers,
//            IReadOnlyDictionary<TItem, List<TItemContainer>> occupiedContainers,
//            bool autoSize,
//            TItemContainer? containerSample,
//            TItem? item,
//            int count
//            )

//            where TItem : IItem
//            where TItemContainer : IItemContainer
//        {
//            if (count <= 0 || item.IsNull())
//                return ReadOnlyItemContainer.Empty;

//            if (autoSize)
//            {
//                int targetItemCount = GetItemCount(occupiedContainers, item) + count;

//                EnsureFreeSpace(
//                    self,
//                    containers,
//                    occupiedContainers,
//                    int.MinValue,
//                    targetItemCount,
//                    item,
//                    containerSample
//                    );
//            }

//            static bool putItem(TItem item, ref int count, IItemContainer container)
//            {
//                count = container.PutItem(item, count).ItemCount;
//                return count >= 1;
//            }

//            using (var sameItemContainers = GetContainersWithItemPooled(occupiedContainers, item, ignoreFull: true))
//                foreach (var container in sameItemContainers)
//                    if (!putItem(item, ref count, container))
//                        return ReadOnlyItemContainer.Empty;

//            using (var emptyContainers = GetEmptyContainersPooled(containers))
//                foreach (var container in emptyContainers)
//                    if (!putItem(item, ref count, container))
//                        return ReadOnlyItemContainer.Empty;

//            return new ReadOnlyItemContainer(item, count);
//        }
//        public static ReadOnlyItemContainer PutItem<TItem, TItemContainer>(
//            IInventory self,
//            IDictionary<int, TItemContainer> containers,
//            IReadOnlyDictionary<TItem, List<TItemContainer>> occupiedContainers,
//            bool autoSize,
//            TItemContainer? containerSample,
//            IItemContainerInfo? containerInfo
//            )

//            where TItem : IItem
//            where TItemContainer : IItemContainer
//        {
//            if (containerInfo.IsNull())
//                return ReadOnlyItemContainer.Empty;

//            return PutItem(
//                self,
//                containers,
//                occupiedContainers,
//                autoSize,
//                containerSample,
//                (TItem)containerInfo.Item!,
//                containerInfo.ItemCount
//                );
//        }
//        public static ReadOnlyItemContainer PutItem<TItem, TItemContainer, TItemContainerInfo>(
//            IInventory self,
//            IDictionary<int, TItemContainer> containers,
//            IReadOnlyDictionary<TItem, List<TItemContainer>> occupiedContainers,
//            bool autoSize,
//            TItemContainer? containerSample,
//            TItemContainerInfo containerInfo
//            )

//            where TItem : IItem
//            where TItemContainer : IItemContainer
//            where TItemContainerInfo : struct, IItemContainerInfo
//        {
//            if (containerInfo.IsEmpty)
//                return ReadOnlyItemContainer.Empty;

//            return PutItem(
//                self,
//                containers,
//                occupiedContainers,
//                autoSize,
//                containerSample,
//                (TItem)containerInfo.Item!,
//                containerInfo.ItemCount
//                );
//        }

//        public static ReadOnlyItemContainer PutItemFrom<TItem, TItemContainer>(
//            IInventory self,
//            IDictionary<int, TItemContainer> containers,
//            IReadOnlyDictionary<TItem, List<TItemContainer>> occupiedContainers,
//            bool autoSize,
//            TItemContainer? containerSample,
//            TItemContainer? container,
//            int count
//            )

//            where TItem : IItem
//            where TItemContainer : IItemContainer
//        {
//            CC.Guard.IsNotNull(container, nameof(container));

//            if (count <= 0 || container.IsEmpty)
//                return ReadOnlyItemContainer.Empty;

//            return container.PutItem(
//                PutItem(
//                    self,
//                    containers,
//                    occupiedContainers,
//                    autoSize,
//                    containerSample,
//                    container.TakeItem(count)
//                    )
//                );
//        }
//        public static ReadOnlyItemContainer PutItemFrom<TItem, TItemContainer>(
//            IInventory self,
//            IDictionary<int, TItemContainer> containers,
//            IReadOnlyDictionary<TItem, List<TItemContainer>> occupiedContainers,
//            bool autoSize,
//            TItemContainer? containerSample,
//            TItemContainer? container
//            )

//            where TItem : IItem
//            where TItemContainer : IItemContainer
//        {
//            if (container.IsNull())
//                return ReadOnlyItemContainer.Empty;

//            return PutItemFrom(
//                self,
//                containers,
//                occupiedContainers,
//                autoSize,
//                containerSample,
//                container,
//                container.ItemCount
//                );
//        }

//        public static ReadOnlyItemContainer TakeItem<TItem, TItemContainer>(
//            IReadOnlyDictionary<TItem, List<TItemContainer>> occupiedContainers,
//            TItem? item,
//            int count
//            )
//            where TItemContainer : IItemContainer
//        {
//            if (item.IsNull()
//                ||
//                count <= 0
//                ||
//                !occupiedContainers.TryGetValue(item, out List<TItemContainer> tOccupiedContainers))
//            {
//                return ReadOnlyItemContainer.Empty;
//            }

//            var takedItems = ReadOnlyItemContainer.Empty;

//            for (int i = 0; i < tOccupiedContainers.Count; i++)
//            {
//                takedItems = tOccupiedContainers[i].TakeItem(count);
//                count -= takedItems.ItemCount;

//                if (count <= 0)
//                    break;
//            }

//            return takedItems;
//        }

//        public static ReadOnlyItemContainer TakeItem<TItem, TItemContainer>(
//            IReadOnlyDictionary<TItem, List<TItemContainer>> occupiedContainers,
//            int count
//            )

//            where TItem : IItem
//            where TItemContainer : IItemContainer
//        {
//            if (occupiedContainers.Values.FirstOrDefault(containers => containers.Any(container => !container.IsEmpty)).IsNot<IItemContainer>(out var container))
//                return ReadOnlyItemContainer.Empty;

//            return TakeItem(occupiedContainers, (TItem)container.Item!, count);
//        }
//        public static ReadOnlyItemContainer TakeItem<TItem, TItemContainer>(
//            IReadOnlyDictionary<TItem, List<TItemContainer>> occupiedContainers
//            )

//            where TItem : IItem
//            where TItemContainer : IItemContainer
//        {
//            if (occupiedContainers.Values.FirstOrDefault(containers => containers.Any(container => !container.IsEmpty)).IsNot<IItemContainer>(out var container))
//                return ReadOnlyItemContainer.Empty;

//            return TakeItem(occupiedContainers, (TItem)container.Item!, GetItemCount(occupiedContainers, (TItem)container.Item!));
//        }

//        public static void EnsureFreeSpace<TItem, TItemContainer>(
//            IInventory self,
//            IDictionary<int, TItemContainer> containers,
//            IReadOnlyDictionary<TItem, List<TItemContainer>> occupiedContainers,
//            int allFreeSpace,
//            int targetSpace,
//            TItem? forItem,
//            TItemContainer? containerSample
//            )

//            where TItem : IItem
//            where TItemContainer : IItemContainer
//        {
//            if (targetSpace <= 0)
//                return;

//            int containerCapacity = Math.Min(

//                containerSample.Maybe()
//                    .BiMap(
//                    state: containers,
//                    some: static (containerSample, _) => containerSample.Capacity,
//                    none: static containers => containers.FirstOrDefault().Value.IfNotNull(static container => container.Capacity)
//                    )
//                    .GetValue(0),

//                forItem.Maybe()
//                    .Map(item => item.MaxItemCount).GetValue(int.MaxValue)
//                );

//            if (containerCapacity == 0 
//                ||
//                GetFreeSpace(containers, occupiedContainers, allFreeSpace, forItem) >= targetSpace)
//            {
//                return;
//            }

//            double targetContainerCountRaw = targetSpace / (double)containerCapacity;
//            int targetContainerCount = (int)Math.Ceiling(targetContainerCountRaw);

//            InstantiateContainers(
//                self,
//                containers,
//                targetContainerCount,
//                containerSample
//                );
//        }

//        public static int GetFreeSpace<TItem, TItemContainer>(
//            IDictionary<int, TItemContainer> containers,
//            IReadOnlyDictionary<TItem, List<TItemContainer>> occupiedContainers,
//            int allFreeSpace,
//            TItem? item
//            )
//            where TItem : IItem
//            where TItemContainer : IItemContainer
//        {
//            if (item.IsNull())
//                return allFreeSpace;

//            int freeSpace = 0;

//            foreach (var container in FilterContainersWithItem(occupiedContainers, item, ignoreFull: true))
//                freeSpace += container.FreeSpace;

//            foreach (var container in FilterEmptyContainers(containers))
//            {
//                if (!container.IgnoreMaxItemCount)
//                    freeSpace += item.MaxItemCount;
//                else
//                    freeSpace += int.MaxValue;
//            }

//            return freeSpace;
//        }

//        public static int GetItemCount<TItem, TItemContainer>(
//            IReadOnlyDictionary<TItem, List<TItemContainer>> occupiedContainers,
//            TItem? item
//            )
//            where TItem : IItem
//            where TItemContainer : IItemContainer
//        {
//            if (item.IsNull())
//            {
//                int allItemCount = 0;

//                foreach (var containerList in occupiedContainers.Values)
//                    for (int i = 0; i < containerList.Count; i++)
//                        allItemCount += containerList[i].ItemCount;

//                return allItemCount;
//            }

//            if (!occupiedContainers.TryGetValue(item, out List<TItemContainer> containers))
//                return 0;

//            int count = 0;

//            foreach (var container in containers)
//                count += container.ItemCount;

//            return count;
//        }

//        public static IEnumerable<IItemContainer> FilterContainersWithItem<TItem, TItemContainer>(
//            IReadOnlyDictionary<TItem, List<TItemContainer>> occupiedContainers,
//            TItem item,
//            bool ignoreFull = true
//            )
//            where TItem : IItem
//            where TItemContainer : IItemContainer
//        {
//            CC.Guard.IsNotNull(item, nameof(item));

//            if (item.IsNull()
//                ||
//                !occupiedContainers.TryGetValue(item, out var containers))
//            {
//                yield break;
//            }

//            for (int i = 0; i < containers.Count; i++)
//            {
//                IItemContainer container = containers[i];

//                if (!EqualityComparer<IItem?>.Default.Equals(item, container.Item)
//                    ||
//                    (ignoreFull && container.IsFull))
//                {
//                    continue;
//                }

//                yield return container;
//            }
//        }

//        public static PooledList<TItemContainer> GetContainersWithItemPooled<TItem, TItemContainer>(
//            IReadOnlyDictionary<TItem, List<TItemContainer>> occupiedContainers,
//            TItem item,
//            bool ignoreFull = true
//            )
//            where TItem : IItem
//            where TItemContainer : IItemContainer
//        {
//            if (!occupiedContainers.TryGetValue(item, out List<TItemContainer> containersWithItem))
//                return default;

//            var results = new PooledList<TItemContainer>(containersWithItem.Count);

//            for (int i = 0; i < containersWithItem.Count; i++)
//            {
//                TItemContainer container = containersWithItem[i];

//                if (ignoreFull && container.IsFull)
//                    continue;

//                results.Add(container);
//            }

//            return results;
//        }

//        public static PooledList<TItemContainer> GetEmptyContainersPooled<TItemContainer>(
//            IDictionary<int, TItemContainer> containers
//            )
//            where TItemContainer : IItemContainer
//        {
//            if (containers.Count == 0)
//                return default;

//            var results = new PooledList<TItemContainer>(containers.Count);

//            foreach (var (_, container) in containers)
//                if (container.IsEmpty)
//                    results.Add(container);

//            return results;
//        }

//        public static IEnumerable<TItemContainer> FilterEmptyContainers<TItemContainer>(
//            IDictionary<int, TItemContainer> containers
//            )
//            where TItemContainer : IItemContainer
//        {
//            foreach (var (_, cnt) in containers)
//            {
//                if (!cnt.IsEmpty)
//                    continue;

//                yield return cnt;
//            }
//        }

//        public static int AddContainer<TItemContainer>(
//            IInventory self,
//            IDictionary<int, TItemContainer> containers,
//            TItemContainer container
//            )

//            where TItemContainer : IItemContainer
//        {
//            CC.Guard.IsNotNull(container, nameof(container));

//            if (container.ParentInventory is not null
//                &&
//                !EqualityComparer<IInventory?>.Default.Equals(container.ParentInventory, self))
//            {
//                throw new ArgumentException("Container cannot have other parent inventory");
//            }

//            if (container.IsReadOnlyContainer)
//                throw new ArgumentException($"Container cannot be readonly. Container: {container}");

//            var id = ResolveID(containers);

//            containers.Add(id, container);
//            return id;
//        }

//        public static void AddContainers<TItem, TItemContainer>(
//            IInventory self,
//            IDictionary<int, TItemContainer> containers,
//            IEnumerable<TItemContainer> newContainers
//            )

//            where TItem : IItem
//            where TItemContainer : IItemContainer
//        {
//            CC.Guard.IsNotNull(newContainers, nameof(newContainers));

//            foreach (var container in newContainers)
//            {
//                if (container.IsNull() || container.IsReadOnlyContainer)
//                    continue;

//                if (container.ParentInventory is not null)
//                    container.SetParentInventory(null);

//                AddContainer(self, containers, container);
//            }
//        }
//        public static void AddContainers<TItem, TItemContainer>(
//            IInventory self,
//            IDictionary<int, TItemContainer> containers,
//            IEnumerable<TItemContainer> newContainers,
//            out IList<int> ids
//            )

//            where TItem : IItem
//            where TItemContainer : IItemContainer
//        {
//            if (newContainers.TryGetNonEnumeratedCount(out int containerCount))
//                ids = new List<int>(containerCount);
//            else
//                ids = new List<int>();

//            foreach (var container in newContainers)
//            {
//                if (container.IsNull() || container.IsReadOnlyContainer)
//                    continue;

//                if (container.ParentInventory is not null)
//                    container.SetParentInventory(null);

//                int id = AddContainer(self, containers, container);
//                ids.Add(id);
//            }
//        }

//        public static bool RemoveContainer<TItemContainer>(
//            IDictionary<int, TItemContainer> containers,
//            int id
//            )

//            where TItemContainer : IItemContainer
//        {
//            return containers.Remove(id);
//        }
//        public static bool RemoveContainer<TItemContainer>(
//            IDictionary<int, TItemContainer> containers,
//            IItemContainer container
//            )

//            where TItemContainer : IItemContainer
//        {
//            if (!container.ID.HasValue)
//                return false;

//            return RemoveContainer(containers, container.ID.Value);
//        }

//        public static void InstantiateContainers<TItemContainer>(
//            IInventory self,
//            IDictionary<int, TItemContainer> containers,
//            int count,
//            TItemContainer? containerSample
//            )

//            where TItemContainer : IItemContainer
//        {
//            InstantiateContainersCore(self, containers, count, null, containerSample);
//        }
//        public static void InstantiateContainers<TItemContainer>(
//            IInventory self,
//            IDictionary<int, TItemContainer> containers,
//            int count,
//            out IList<TItemContainer> results,
//            TItemContainer? cloneExmaple
//            )

//            where TItemContainer : IItemContainer
//        {
//            results = new List<TItemContainer>(count);
//            InstantiateContainersCore(self, containers, count, results, cloneExmaple);
//        }

//        public static void SetContainerCount<TItemContainer>(
//            IInventory self,
//            IDictionary<int, TItemContainer> containers,
//            int count,
//            TItemContainer? containerSample
//            )

//            where TItemContainer : IItemContainer
//        {
//            count = Math.Max(count, 0);
//            int delta = count - ((ICollection)containers).Count;

//            if (delta < 0)
//                RemoveCount(containers, delta);
//            else if (delta > 0)
//                InstantiateContainers(self, containers, delta, containerSample);
//        }

//        public static void SetContainerCount<TItemContainer>(
//            IInventory self,
//            IDictionary<int, TItemContainer> containers,
//            int count,
//            out IList<TItemContainer> changed,
//            TItemContainer? containerSample
//            )

//            where TItemContainer : IItemContainer
//        {
//            if (count >= 1)
//            {
//                int delta = count - ((ICollection)containers).Count;

//                if (delta < 0)
//                {
//                    RemoveCount(containers, delta, out changed);
//                    return;
//                }
//                else if (delta > 0)
//                {
//                    InstantiateContainers(
//                        self,
//                        containers,
//                        delta,
//                        out changed,
//                        containerSample
//                        );
//                    return;
//                }
//            }

//            changed = Array.Empty<TItemContainer>();
//        }

//        public static void RemoveCount<TItemContainer>(
//            IDictionary<int, TItemContainer> containers,
//            int removeCount
//            )

//            where TItemContainer: IItemContainer
//        {
//            RemoveCountCore(containers, removeCount, null);
//        }
//        public static void RemoveCount<TItemContainer>(
//            IDictionary<int, TItemContainer> containers,
//            int removeCount,
//            out IList<TItemContainer> removed
//            )

//            where TItemContainer : IItemContainer
//        {
//            removed = new List<TItemContainer>(removeCount);
//            RemoveCountCore(containers, removeCount, (List<TItemContainer>)removed);
//        }

//        public static bool CanPut<TItem, TItemContainer>(
//            IDictionary<int, TItemContainer> containers,
//            IReadOnlyDictionary<TItem, List<TItemContainer>> occupiedContainers,
//            int allFreeSpace,
//            TItem? item
//            )
//            where TItem : IItem
//            where TItemContainer : IItemContainer
//        {
//            if (item.IsNull() || allFreeSpace <= 0)
//                return false;

//            return GetFreeSpace(
//                        containers,
//                        occupiedContainers,
//                        allFreeSpace,
//                        item) > 0;
//        }
//        public static bool CanPut<TItem, TItemContainer>(
//            IDictionary<int, TItemContainer> containers,
//            IReadOnlyDictionary<TItem, List<TItemContainer>> occupiedContainers,
//            int allFreeSpace,
//            TItem? item,
//            int count
//            )

//            where TItem : IItem
//            where TItemContainer : IItemContainer
//        {
//            if (item.IsNull() || allFreeSpace <= 0)
//                return false;

//            return GetFreeSpace(containers, occupiedContainers, allFreeSpace, item) >= count;
//        }

//        public static IEnumerable<ReadOnlyItemContainer> GetCompactedContainersQuery<TItem, TItemContainer>(
//            IReadOnlyDictionary<TItem, List<TItemContainer>> occupiedContainers
//            )
//            where TItem : IItem
//            where TItemContainer : IItemContainer
//        {
//            foreach (var containers in occupiedContainers.Values)
//            {
//                IItem? item = null;
//                int itemCount = 0;
//                bool hasItem = false;

//                for (int i = 0; i < containers.Count; i++)
//                {
//                    if (!hasItem)
//                    {
//                        item = containers[i].Item;
//                        hasItem = true;
//                    }

//                    itemCount += containers[i].ItemCount;
//                }

//                if (item.IsNull() || itemCount <= 0)
//                    continue;

//                yield return new ReadOnlyItemContainer(item, itemCount);
//            }
//        }

//        public static IEnumerable<IItemContainer> GetOccupiedContainersQuery<TItem, TItemContainer>(
//            IReadOnlyDictionary<TItem, List<TItemContainer>> occupiedContainers
//            )

//            where TItem : IItem
//            where TItemContainer : IItemContainer
//        {
//            foreach (var containers in occupiedContainers.Values)
//                for (int i = 0; i < containers.Count; i++)
//                    yield return containers[i];
//        }

//        public static void CopyItemFrom<TItem, TItemContainer, TContainers>(
//            IInventory self,
//            TContainers containers,
//            IReadOnlyDictionary<TItem, List<TItemContainer>> occupiedContainers,
//            bool autoSize,
//            TItemContainer? containerSample,
//            IItemContainerInfo itemContainer
//            )

//            where TContainers : IDictionary<int, TItemContainer>, IReadOnlyDictionary<int, TItemContainer>
//            where TItem : IItem
//            where TItemContainer : IItemContainer
//        {
//            PutItem(
//                self,
//                containers,
//                occupiedContainers,
//                autoSize,
//                containerSample,
//                itemContainer
//                );
//        }

//        public static int ResolveID<TItemContainer>(
//            IDictionary<int, TItemContainer> containers
//            )
//            where TItemContainer : IItemContainer
//        {
//            var ids = new List<int>(containers.Count);

//            foreach (var (cntID, _) in containers)
//                ids.Add(cntID);

//            if (Do.TryFindHoleInRange(start: 0, containers.Count, ids, out int hole))
//                return hole;

//            return containers.Count;
//        }

//        public static Observable<InventoryContainerAddEvent> ObserveContainerAdd<TItemContainer>(
//            ObservableDictionary<int, TItemContainer> containers,
//            CancellationToken cancellationToken = default
//            )

//            where TItemContainer : IItemContainer
//        {
//            return containers.ObserveDictionaryAdd(cancellationToken)
//                .Select(container => new InventoryContainerAddEvent 
//                {
//                    ID = container.Key, 
//                    Container = container.Value 
//                });
//        }

//        public static Observable<InventoryContainerRemoveEvent> ObserveContainerRemove<TItemContainer>(
//             ObservableDictionary<int, TItemContainer> containers,
//             CancellationToken cancellationToken = default
//            )

//            where TItemContainer : IItemContainer
//        {
//            return containers.ObserveDictionaryRemove(cancellationToken)
//                .Select(container => new InventoryContainerRemoveEvent 
//                {
//                    ID = container.Key, 
//                    Container = container.Value 
//                });
//        }

//        public static Observable<InventoryContainerReplaceEvent> ObserveContainerReplace<TItemContainer>(
//            ObservableDictionary<int, TItemContainer> containers,
//            CancellationToken cancellationToken = default
//            )

//            where TItemContainer : IItemContainer
//        {
//            return containers.ObserveDictionaryReplace(cancellationToken)
//                .Select(container => new InventoryContainerReplaceEvent
//                {
//                    ID = container.Key,
//                    OldContainer = container.NewValue,
//                    NewContainer = container.NewValue 
//                });
//        }

//        private static void InstantiateContainersCore<TItemContainer>(
//            IInventory self,
//            IDictionary<int, TItemContainer> containers,
//            int count,
//            IList<TItemContainer>? results,
//            TItemContainer? containerSample
//            )

//            where TItemContainer : IItemContainer
//        {
//            if (containerSample.IsNull())
//                containerSample = containers.FirstOrDefault().Value;

//            if (containerSample.IsNull())
//            {
//                self.PrintWarning($"Cannot instantiate containers without container sample");
//                return;
//            }

//            if (count <= 0)
//                return;

//            if (containerSample.IsReadOnlyContainer)
//                throw new ArgumentException($"Item container cannot be readonly. Container: {containerSample}");

//            bool collectResults = results is not null;

//            for (int i = 0; i < count; i++)
//            {
//                var cloned = (TItemContainer)containerSample.ShallowClone();
//                cloned.Clear();

//                AddContainer(self, containers, cloned);

//                if (collectResults)
//                    results!.Add(cloned);
//            }
//        }

//        private static void RemoveCountCore<TItemContainer>(
//            IDictionary<int, TItemContainer> containers,
//            int removeCount,
//            List<TItemContainer>? removed
//            )

//            where TItemContainer : IItemContainer
//        {
//            if (removeCount <= 0)
//                return;

//            bool collectRemoved = removed is not null;

//            bool removeContainer(TItemContainer container)
//            {
//                if (RemoveContainer(containers, container))
//                    removeCount--;

//                if (collectRemoved)
//                    removed!.Add(container);

//                return removeCount >= 1;
//            }

//            foreach (var emptyContainer in FilterEmptyContainers(containers))
//                if (!removeContainer(emptyContainer))
//                    return;

//            foreach (var container in containers.SelectValue().Reverse())
//                if (!removeContainer(container))
//                    return;
//        }

//        private static void OnContainerAdd(DictionaryAddEvent<int, IItemContainer> addEv)
//        {
//            int id = addEv.Key;
//            IItemContainer? cnt = addEv.Value;

//            BindContainerItemCount(cnt);
//            BindContainerItem(cnt);
//            ResolveOccupied(cnt);

//            if (!EqualityComparer<IInventory?>.Default.Equals(this, cnt.ParentInventory))
//            {
//                cnt.ID = addEv.Key;
//                cnt.SetParentInventory(this);
//            }

//            FreeSpace += cnt.FreeSpace;
//        }

//        protected virtual void OnContainerItemChanged((IItem? Previous, IItem? Current) items, IItemContainer cnt)
//        {
//            var (previous, current) = items;

//            if (previous.IsNotNull()
//                &&
//                occupiedContainers.TryGetValue(previous, out var occupiedCnts))
//            {
//                occupiedCnts.Remove(cnt);
//            }

//            if (current.IsNotNull())
//                occupiedContainers.GetOrCreateNew(current).Add(cnt);
//        }

//        public static void OnContainerRemove<TItem, TItemContainer>(
//            IDictionary<TItemContainer, CompositeDisposable> containerDisposables,
//            IDictionary<TItem, List<TItemContainer>> occupiedContainers,
//            ref int freeSpace,
//            ReactiveProperty<int> itemCount,
//            DictionaryRemoveEvent<int, TItemContainer> removeEv
//            )

//            where TItem : IItem
//            where TItemContainer : IItemContainer
//        {
//            var cnt = removeEv.Value;

//            if (containerDisposables.TryGetValue(cnt, out var disposables))
//                disposables.Dispose();

//            freeSpace = Math.Max(freeSpace - cnt.FreeSpace, 0);
//            itemCount.Value = Math.Max(itemCount.Value - cnt.ItemCount, 0);

//            if (!cnt.IsEmpty
//                &&
//                cnt.Item.Is<TItem>(out var item)
//                &&
//                occupiedContainers.TryGetValue(item, out var cnts))
//            {
//                cnts.Remove(cnt);
//            }
//        }

//        public static void OnContainerReplace(DictionaryReplaceEvent<int, IItemContainer> replaceEv)
//        {
//            var id = replaceEv.Key;
//            var oldCnt = replaceEv.OldValue;
//            var newCnt = replaceEv.NewValue;

//            var removeEv = new DictionaryRemoveEvent<int, IItemContainer>(id, oldCnt);
//            OnContainerRemove(removeEv);

//            var addEv = new DictionaryAddEvent<int, IItemContainer>(id, newCnt);
//            OnContainerAdd(addEv);
//        }

//        protected virtual void OnContainersClear(Unit _)
//        {
//            occupiedContainers.Clear();
//            containerDisposables.SelectValue().DisposeEach(bufferized: true);
//            containerDisposables.Clear();
//            FreeSpace = 0;
//            itemCount.Value = 0;
//        }

//        private void BindContainerAdd()
//        {
//            containerAddBinding = containers.ObserveDictionaryAdd(DisposeCancellationToken)
//                .Subscribe(OnContainerAdd);
//        }

//        public static void BindContainerItemCount<TItemContainer>(
//            IDictionary<TItemContainer, CompositeDisposable> containerDisposables,
//            TItemContainer container
//            )

//            where TItemContainer : IItemContainer
//        {
//            var disposables = containerDisposables.GetOrCreateNew(container);

//            container.ObserveItemCount()
//                .Pairwise()
//                .SelectDelta()
//                .Subscribe(OnContainerItemCountChanged)
//                .AddTo(disposables);
//        }

//        private void ResolveOccupied(IItemContainer cnt)
//        {
//            if (cnt.IsEmpty || !cnt.Item.Is<IItem>(out var item))
//                return;

//            occupiedContainers.GetOrCreateNew(item).Add(cnt);
//        }

//        private static void OnContainerItemCountChanged(int itemCountDelta)
//        {
//            itemCount.Value = Math.Clamp(itemCount.Value + itemCountDelta, 0, int.MaxValue);
//        }

//        private void BindContainerItem(IItemContainer cnt)
//        {
//            var disposables = containerDisposables.GetOrCreateNew(cnt);

//            cnt.ObserveItem()
//                .Pairwise()
//                .Subscribe(cnt, OnContainerItemChanged)
//                .AddTo(disposables);
//        }

//        private void BindContainerRemove()
//        {
//            containerRemoveBinding = containers.ObserveDictionaryRemove(DisposeCancellationToken)
//                 .Subscribe(OnContainerRemove);
//        }

//        private void BindContainerReplace()
//        {
//            containerReplaceBinding = containers.ObserveDictionaryReplace(DisposeCancellationToken)
//                .Subscribe(OnContainerReplace);
//        }

//        private void BindContainersClear()
//        {
//            containersClearBinding = containers.ObserveClear(DisposeCancellationToken)
//                .Subscribe(OnContainersClear);
//        }
//    }
//}
