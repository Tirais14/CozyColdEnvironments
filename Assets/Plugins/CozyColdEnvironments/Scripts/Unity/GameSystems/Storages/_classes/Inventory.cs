using CCEnvs.Diagnostics;
using CCEnvs.Language;
using CCEnvs.Linq;
using CCEnvs.Reflection;
using CCEnvs.UI.MVVM;
using System;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;
using ZLinq;

#nullable enable
namespace CCEnvs.Unity.GameSystems.Storages
{
    public class Inventory : IInventory
    {
        private readonly Dictionary<int, IItemContainer> inner;

        private Subject<(int id, IItemContainer value)>? addSubj;
        private Subject<(int id, IItemContainer value)>? removeSubj;
        private int nextSlotID;

        public IItemContainer this[int id] => inner[id];

        public event Action<(int id, IItemContainer value)>? OnAdd;
        public event Action<(int id, IItemContainer value)>? OnRemove;


        public int Count => inner.Count;
        public int Capacity {
            get => inner.Count;
            set => inner.EnsureCapacity(value);
        }
        public IEnumerable<int> IDs => inner.Keys;
        public IEnumerable<IItemContainer> Containers => inner.Values;
        public bool IsEmpty => inner.Values.Any(x => x.Contains());
        public bool IsFull => inner.Values.All(x => x.Contains());

        IReadOnlyReactiveProperty<int> IItemContainerInfoItemless.ItemCount { get; } = new ReactiveProperty<int>(int.MinValue);

        public Inventory(int capacity)
        {
            inner = new Dictionary<int, IItemContainer>(capacity);
        }

        public Inventory()
            :
            this(capacity: 4)
        {
        }

        public Inventory(IEnumerable<KeyValuePair<int, IItemContainer>> containers)
        {
            CC.Guard.NullArgument(containers, nameof(containers));

            inner = new Dictionary<int, IItemContainer>(containers);
        }

        public bool Contains()
        {
            return inner.Values.ZL().Any(x => x.Contains());
        }

        public bool Contains(IItem? item)
        {
            return inner.Values.ZL().Any(x => x.Contains(item));
        }

        public bool Contains(IItem? item, int count)
        {
            int containedCount = inner.Values.ZL()
                                             .Where(x => x.Contains(item))
                                             .Sum(x => x.ItemCount.Value);

            return containedCount >= count;
        }

        public bool Contains(IItemContainer itemContainer)
        {
            return inner.Values.Contains(itemContainer);
        }

        public bool Contains(int id) => inner.ContainsKey(id);

        public void Add(IItemContainer itemContainer)
        {
            try
            {
                inner.Add(nextSlotID, itemContainer);

                Do.While(() => inner.ContainsKey(nextSlotID), () => nextSlotID++);

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
            if (inner.Remove(id, out IItemContainer value))
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

            KeyValuePair<int, IItemContainer> found = inner.FirstOrDefault(x => x.Value.Equals(itemContainer));
            if (found.IsDefault())
                return false;

            return Remove(found.Key);
        }

        public void RemoveCount(int count)
        {
            for (int i = 0; i < count; i++)
                RemoveLast();
        }

        public bool RemoveLast() => Remove(inner.Keys.Max());

        public void Clear()
        {
            foreach (var id in inner.Keys.ToArray())
                inner.Remove(id);
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
                this.PrintLog($"Item: {item.Maybe().Map(x => x!.ToString()).Access("null")}, count: {count}. is not added.");
                return null!;
            }

            int rest = count;
            Maybe<IItemContainer> restItems;
            foreach (var cnt in from it in inner.ZL() //Searching for container with item or first empty container
                                where it.Value.IsEmpty || (it.Value.Contains(item) && !it.Value.IsFull)
                                select (it, priority: it.Value.Contains(item) ? it.Key - 1 : it.Key) into pair
                                orderby pair.priority
                                select pair.it)
            {
                restItems = cnt.Value.Put(item, rest);

                this.PrintLog($"Item: {item}, count: {count}, item container id: {cnt.Key}. Is added.");

                rest -= restItems.AccessUnsafe().ItemCount.Value;

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
            foreach (var cnt in inner.Values.ZL().Where(x => x.Contains(item)))
            {
                taked = cnt.Take(count);
                count -= taked.AccessUnsafe().ItemCount.Value;

                if (count <= 0)
                    break;
            }

            return default!;
        }

        Maybe<IItemContainer> IItemAccessor.Take(int count) => null!;

        Maybe<IItemContainer> IItemAccessor.Take() => null!;

        void IItemAccessor.CopyFrom(IItemContainerInfo itemContainer)
        {
            this.PrintWarning($"{nameof(IItemAccessor.CopyFrom)} not supported and was be mocked.");
        }
    }
}
