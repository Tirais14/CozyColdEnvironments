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
using System.Diagnostics;
using System.Linq;
using ZLinq;

#pragma warning disable S3236
#nullable enable
namespace CCEnvs.Unity.Items
{
    public class Inventory : IInventory
    {
        private readonly ObservableDictionary<int, IItemContainer> collection = new();

        protected IDictionary<int, IItemContainer> collectionBase => collection;

        public Result<IItemContainer> this[int id] {
            get
            {
                if (!collection.TryGetValue(id, out var cnt))
                    return (null, new InvalidOperationException($"Not found id: {id}"));

                return (cnt, null);
            }
        }

        public bool IsEmpty => Containers.Any(static cnt => !cnt.IsEmpty);
        public bool IsFull => collectionBase.Values.All(static cnt => cnt.IsFull);
        public bool AutoSize { get; set; }
        public int FreeSpace => collectionBase.Values.Count(static x => x.IsEmpty);
        public int ContainerCount => collection.Count;

        public IEnumerable<int> IDs => collectionBase.Keys;
        public IEnumerable<IItemContainer> Containers => collectionBase.Values;

        int IItemContainerInfoItemless.ItemCount => throw new NotImplementedException();
        Maybe<IInventory> IItemContainerInfoItemless.ParentInventory { get => null!; set => _ = value; }
        int IItemContainerInfoItemless.Capacity {
            get => ContainerCount;
            set => SetCount<ItemContainer>(value);
        }

        public Inventory()
        {
        }

        public Inventory(IEqualityComparer<int> comparer)
        {
            collection = new ObservableDictionary<int, IItemContainer>(comparer);
        }

        public Inventory(Dictionary<int, IItemContainer> innerDictionary)
        {
            collection = new ObservableDictionary<int, IItemContainer>(innerDictionary);
        }

        public Inventory(IEnumerable<KeyValuePair<int, IItemContainer>> values)
            :
            this(new Dictionary<int, IItemContainer>(values))
        {
        }

        public Inventory(int containerCount)
            :
            this(new Dictionary<int, IItemContainer>())
        {
            SetCount<ItemContainer>(containerCount);
        }

        public static Inventory Create<T>(int containerCount)
            where T : IItemContainer, new()
        {
            var inv = new Inventory();
            inv.SetCount<T>(containerCount);

            return inv;
        }

        public bool ContainsItem()
        {
            return collectionBase.Values.AsValueEnumerable().Any(x => x.ContainsItem());
        }

        public bool ContainsItem(IItem? item)
        {
            return collectionBase.Values.AsValueEnumerable().Any(x => x.ContainsItem(item));
        }

        public bool ContainsItem(IItem? item, int count)
        {
            int containedCount = collectionBase.Values.AsValueEnumerable()
                .Where(x => x.ContainsItem(item))
                .Sum(x => x.ItemCount);

            return containedCount >= count;
        }

        public void ResetContainers()
        {
            foreach (var cnt in collectionBase.Values)
                cnt.Reset();
        }

        public Maybe<IItemContainer> PutItem(IItem? item, int count = 1)
        {
            if (item.IsNull() || count <= 0)
            {
                this.PrintLog($"Item: {item.Maybe().Map(x => x!.ToString()).GetValue("null")}, count: {count}. Is not added.");
                return null!;
            }

            if (AutoSize)
            {
                while (!CanPut(item, count))
                    AddContainer(new ItemContainer(capacity: int.MaxValue));
            }
            else
            {
                if (!CanPut(item))
                    return new ItemContainer(item: item, count: count);
            }

            int rest = count;
            Maybe<IItemContainer> restItems;
            foreach (var cnt in from cnt in collection.AsValueEnumerable() //Searching for the container with same item or first empty container
                                where cnt.Value.IsEmpty || (cnt.Value.ContainsItem(item) && !cnt.Value.IsFull)
                                select (cnt, priority: cnt.Value.ContainsItem(item) ? cnt.Key - 1 : cnt.Key) into pair
                                orderby pair.priority
                                select pair.cnt)
            {
                restItems = cnt.Value.PutItem(item, rest);

                this.PrintLog($"Item: {item}, count: {count}, item container id: {cnt.Key}. Is added.");

                rest = restItems.Map(x => x.ItemCount).GetValue(0);

                if (rest <= 0)
                    break;
            }

            if (rest > 0)
            {
                return new ItemContainer(item, rest)
                {
                    UnlockCapacity = true
                };
            }

            return null!;
        }

        public Maybe<IItemContainer> PutItemFrom(IItemContainer itemContainer, int count)
        {
            return (from cnt in itemContainer.TakeItem(count)
                    where cnt.Item.IsSome
                    let item = cnt.Item.GetValueUnsafe()
                    select (cnt, rest: PutItem(item, count)))
                    .IfRight(x => x.rest.IfSome(
                        rest => itemContainer.PutItemFrom(rest)).Raw)
                    .RightTarget
                    .Maybe()!;
        }

        [DebuggerStepThrough]
        public Maybe<IItemContainer> PutItemFrom(IItemContainer itemContainer)
        {
            return PutItem(itemContainer.Item.GetValue(), itemContainer.ItemCount);
        }

        public Maybe<IItemContainer> TakeItem(IItem item, int count)
        {
            CC.Guard.IsNotNull(item, nameof(item));
            if (count <= 0)
                return null!;

            Maybe<IItemContainer> taked;
            foreach (var cnt in collectionBase.Values.AsValueEnumerable().Where(x => x.ContainsItem(item)))
            {
                taked = cnt.TakeItem(count);
                count -= taked.GetValueUnsafe().ItemCount;

                if (count <= 0)
                    break;
            }

            return default!;
        }

        public void AddContainer(int id, IItemContainer itemContainer)
        {
            CC.Guard.IsNotNull(itemContainer, nameof(itemContainer));
            collection.Add(id, itemContainer);
        }

        public void AddContainer(IItemContainer itemContainer)
        {
            CC.Guard.IsNotNull(itemContainer, nameof(itemContainer));
            AddContainer(ResolveID(itemContainer), itemContainer);
        }

        public bool RemoveContainer(int id)
        {
            return collection.Remove(id);
        }

        public T[] AddCount<T>(int count) where T : IItemContainer, new()
        {
            count = Math.Abs(count);
            var results = new List<T>(count);
            T cnt;
            foreach (var _ in Enumerable.Range(0, count))
            {
                cnt = new T();
                AddContainer(cnt);
                results.Add(cnt);
            }

            return results.ToArray();
        }

        public IItemContainer[] SetCount<T>(int count) where T : IItemContainer, new()
        {
            count = Math.Abs(count);
            int delta = count - ContainerCount;

            if (delta < 0)
                return RemoveCount(delta);
            else if (delta > 0)
                return AddCount<T>(delta).Cast<IItemContainer>().ToArray();
            else
                return Array.Empty<IItemContainer>();
        }

        public IItemContainer[] RemoveCount(int count)
        {
            var removed = new List<IItemContainer>(count);
            IItemContainer cnt;
            int id;
            foreach (var _ in Enumerable.Range(0, count))
            {
                if (ContainerCount == 0)
                    break;

                id = collectionBase.Keys.Last();
                cnt = this[id].Strict();
                RemoveContainer(id);
            }

            return removed.ToArray();
        }

        public bool CanPut() => !IsFull;

        public bool CanPut(IItem? item)
        {
            if (item.IsNull())
                return false;

            foreach (var cnt in collectionBase.Values)
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
            foreach (var cnt in collectionBase.Values)
            {
                if (cnt.IsEmpty || cnt.ContainsItem(item))
                    freeSpace += cnt.FreeSpace;

                if (freeSpace >= count)
                    return true;
            }

            return false;
        }

        public void Reset() => collection.Clear();

        public IEnumerator<KeyValuePair<int, IItemContainer>> GetEnumerator()
        {
            return collection.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public Observable<DictionaryAddEvent<int, IItemContainer>> ObserveAddContainer()
        {
            return collection.ObserveDictionaryAdd();
        }

        public Observable<DictionaryRemoveEvent<int, IItemContainer>> ObserveRemoveContainer()
        {
            return collection.ObserveDictionaryRemove();
        }

        public Observable<DictionaryReplaceEvent<int, IItemContainer>> ObserveReplaceContainer()
        {
            return collection.ObserveDictionaryReplace();
        }

        public Observable<CollectionResetEvent<KeyValuePair<int, IItemContainer>>> ObserveReset()
        {
            return collection.ObserveReset();
        }

        protected virtual int ResolveID(IItemContainer itemContainer)
        {
            IEnumerable<int> ids = collectionBase.Values.AsValueEnumerable()
                .Select(x => x.GetContainerID())
                .Where(x => x.IsSome)
                .Select(x => x.Raw)
                .AsEnumerable();

            if (Do.TryFindHoleInRange(start: 0, ContainerCount, ids, out int hole))
                return hole;

            return ContainerCount;
        }

        Maybe<IItemContainer> IItemAccessor.TakeItem(int count) => null!;

        Maybe<IItemContainer> IItemAccessor.TakeItem() => null!;

        Maybe<int> IItemContainerInfoItemless.GetContainerID() => Maybe<int>.None;

        void IItemAccessor.CopyFrom(IItemContainerInfo itemContainer)
        {
            throw new NotImplementedException();
        }

        Observable<(int Previous, int Current)> IItemContainerInfoItemless.ObserveDecreasedItemCount()
        {
            throw new NotImplementedException();
        }

        Observable<(int Previous, int Current)> IItemContainerInfoItemless.ObserveIncreaseItemCount()
        {
            throw new NotImplementedException();
        }

        Observable<int> IItemContainerInfoItemless.ObserveItemCount()
        {
            throw new NotImplementedException();
        }
    }
}
