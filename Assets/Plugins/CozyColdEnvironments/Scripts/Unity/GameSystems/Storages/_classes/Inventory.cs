using CCEnvs.Diagnostics;
using CCEnvs.Language;
using CCEnvs.Linq;
using CCEnvs.UI.MVVM;
using CommunityToolkit.Diagnostics;
using System;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;
using ZLinq;

#nullable enable
namespace CCEnvs.Unity.GameSystems.Storages
{
    public class Inventory : IInventory
    {
        private readonly Dictionary<int, IItemContainer> collection;

        private Subject<(int id, IItemContainer value)>? addSubj;
        private Subject<(int id, IItemContainer value)>? removeSubj;
        private int nextSlotID;

        public IItemContainer this[int id] => collection[id];

        public event Action<(int id, IItemContainer value)>? OnAdd;
        public event Action<(int id, IItemContainer value)>? OnRemove;

        public int Count => collection.Count;
        public int Capacity {
            get => collection.Count;
            set => collection.EnsureCapacity(value);
        }
        public IEnumerable<int> IDs => collection.Keys;
        public IEnumerable<IItemContainer> Containers => collection.Values;
        public bool IsEmpty => collection.Values.Any(x => x.Contains());
        public bool IsFull => collection.Values.All(x => x.Contains());

        IReadOnlyReactiveProperty<int> IItemContainerInfoItemless.ItemCount { get; } = new ReadOnlyReactiveProperty<int>(Observable.Empty<int>());
        Maybe<IItemContainer> IItemContainerInfoItemless.ParentContainer => null!;

        public Inventory(int capacity)
        {
            collection = new Dictionary<int, IItemContainer>(capacity);
        }

        public Inventory()
            :
            this(capacity: 4)
        {
        }

        public Inventory(IEnumerable<KeyValuePair<int, IItemContainer>> containers)
        {
            CC.Guard.NullArgument(containers, nameof(containers));

            collection = new Dictionary<int, IItemContainer>(containers);
        }

        public bool Contains()
        {
            return collection.Values.ZL().Any(x => x.Contains());
        }

        public bool Contains(IItem? item)
        {
            return collection.Values.ZL().Any(x => x.Contains(item));
        }

        public bool Contains(IItem? item, int count)
        {
            int containedCount = collection.Values.ZL()
                                             .Where(x => x.Contains(item))
                                             .Sum(x => x.ItemCount.Value);

            return containedCount >= count;
        }

        public bool Contains(IItemContainer itemContainer)
        {
            return collection.Values.Contains(itemContainer);
        }

        public bool Contains(int id) => collection.ContainsKey(id);

        public void Add(IItemContainer itemContainer)
        {
            try
            {
                collection.Add(nextSlotID, itemContainer);

                Do.While(() => collection.ContainsKey(nextSlotID), () => nextSlotID++);

                addSubj?.OnNext((nextSlotID, itemContainer));
                OnAdd?.Invoke((nextSlotID, itemContainer));
            }
            catch (Exception ex)
            {
                this.PrintException(ex);
            }
        }
        public void Add(GameObject toInstantiate)
        {
            UnityEngine.Object.Instantiate(toInstantiate)
                              .GetAssignedModel<IItemContainer>()
                              .Maybe()
                              .IfSome(Add!);
        }

        public void AddCount(int count, UnityEngine.GameObject toInstantiate)
        {
            for (int i = 0; i < count; i++)
                Add(toInstantiate);
        }

        public void AddCount<T>(int count)
            where T : IItemContainer, new()
        {
            for (int i = 0; i < count; i++)
                Add(new T());
        }

        public bool Remove(int id)
        {
            if (collection.Remove(id, out IItemContainer value))
            {
                try
                {
                    removeSubj?.OnNext((id, value));
                    OnRemove?.Invoke((id, value));
                }
                catch (Exception ex)
                {
                    this.PrintException(ex);
                }

                return true;
            }

            return false;
        }
        public bool Remove(IItemContainer itemContainer)
        {
            CC.Guard.NullArgument(itemContainer, nameof(itemContainer));

            KeyValuePair<int, IItemContainer> found = collection.FirstOrDefault(x => x.Value.Equals(itemContainer));
            if (found.IsDefault())
                return false;

            return Remove(found.Key);
        }

        public void RemoveCount(int count)
        {
            for (int i = 0; i < count; i++)
                RemoveLast();
        }

        public bool RemoveLast() => Remove(collection.Keys.Max());

        public void Clear()
        {
            foreach (var id in collection.Keys.ToArray())
                collection.Remove(id);
        }

        public void SetCount<T>(int count)
            where T : IItemContainer, new()
        {
            if (count == Count)
                return;

            int delta = count - Count;
            if (delta < 0)
                RemoveCount(Math.Abs(delta));
            else
                AddCount<T>(delta);
        }
        public void SetCount(int count, GameObject toInstantiate)
        {
            if (count == Count)
                return;

            int delta = count - Count;
            if (delta < 0)
                RemoveCount(Math.Abs(delta));
            else
                AddCount(delta, toInstantiate);
        }

        public IObservable<(int id, IItemContainer value)> ObserveAdd()
        {
            addSubj ??= new Subject<(int id, IItemContainer value)>();

            return addSubj;
        }

        public IObservable<(int id, IItemContainer value)> ObserveRemove()
        {
            removeSubj ??= new Subject<(int id, IItemContainer value)>();

            return removeSubj;
        }

        public Maybe<IItemContainer> Put(IItem? item, int count)
        {
            if (item.IsNull() || count <= 0)
            {
                this.PrintLog($"Item: {item.Maybe().Map(x => x!.ToString()).Access("null")}, count: {count}. Is not added.");
                return null!;
            }

            int rest = count;
            Maybe<IItemContainer> restItems;
            foreach (var cnt in from it in collection.ZL() //Searching for container with item or first empty container
                                where it.Value.IsEmpty || (it.Value.Contains(item) && !it.Value.IsFull)
                                select (it, priority: it.Value.Contains(item) ? it.Key - 1 : it.Key) into pair
                                orderby pair.priority
                                select pair.it)
            {
                restItems = cnt.Value.Put(item, rest);

                this.PrintLog($"Item: {item}, count: {count}, item container id: {cnt.Key}. Is added.");

                rest = restItems.Map(x => x.ItemCount.Value).Access(0);

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

        public Maybe<IItemContainer> Put(IItemContainer itemContainer, int count)
        {
            return Put(itemContainer.Item.Value.Access(), count);
        }

        public Maybe<IItemContainer> Put(IItemContainer itemContainer)
        {
            return Put(itemContainer.Item.Value.Access(), itemContainer.ItemCount.Value);
        }

        public Maybe<IItemContainer> Take(IItem item, int count)
        {
            CC.Guard.NullArgument(item, nameof(item));
            if (count <= 0)
                return null!;

            Maybe<IItemContainer> taked;
            foreach (var cnt in collection.Values.ZL().Where(x => x.Contains(item)))
            {
                taked = cnt.Take(count);
                count -= taked.AccessUnsafe().ItemCount.Value;

                if (count <= 0)
                    break;
            }

            return default!;
        }

        public MaybeStruct<int> GetID(IItemContainer itemContainer)
        {
            Guard.IsNotNull(itemContainer, nameof(itemContainer));

            collection.FirstOrDefault(x => x.Value.Equals(itemContainer)).Maybe().Map(x => x.Key).Struct().IfSome(x => x)
        }

        Maybe<IItemContainer> IItemAccessor.Take(int count) => null!;

        Maybe<IItemContainer> IItemAccessor.Take() => null!;

        void IItemAccessor.CopyFrom(IItemContainerInfo itemContainer)
        {
            this.PrintWarning($"{nameof(IItemAccessor.CopyFrom)} not supported and was be mocked.");
        }
    }
}
