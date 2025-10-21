using CCEnvs.Diagnostics;
using CCEnvs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using ZLinq;

#nullable enable
namespace CCEnvs.Unity.GameSystems.Storages
{
    public class Inventory : IInventory
    {
        private readonly Dictionary<int, IItemContainer> inner;

        private Subject<(int id, IItemContainer value)>? addSubj;
        private Subject<(int id, IItemContainer value)>? removeSubj;

        public event Action<(int id, IItemContainer value)>? OnAdd;
        public event Action<(int id, IItemContainer value)>? OnRemove;

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
            int nextID = IDs.Max() + 1;

            try
            {
                inner.Add(nextID, itemContainer);

                addSubj?.OnNext((nextID, itemContainer));
                OnAdd?.Invoke((nextID, itemContainer));
            }
            catch (Exception ex)
            {
                this.PrintException(ex);
            }
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

        public void Clear()
        {
            foreach (var id in inner.Keys.ToArray())
                inner.Remove(id);
        }

        public IItemContainer Put(IItem? item, int count)
        {
            int before;
            foreach (var con in inner.Values)
            {
                if (count <= 0)
                    return ItemContainer.Empty;

                if (!con.IsEmpty && !con.Contains(item))
                    continue;

                before = con.ItemCount.Value;
                con.Put(item, count);

                count -= con.ItemCount.Value - before;
            }

            return new ItemContainer(item, count);
        }

        public IItemContainer Put(IItemContainer itemContainer, int count)
        {
            return Put(itemContainer.Item.Value, count);
        }

        public IItemContainer Put(IItemContainer itemContainer)
        {
            return Put(itemContainer.Item.Value, itemContainer.ItemCount.Value);
        }

        public IItemContainer Take(IItem item, int count)
        {
            var result = new ItemContainer
            {
                Unlocked = true
            };

            int beforeCount;
            foreach (var con in inner.Values.ZL().Where(x => x.Contains(item)))
            {
                beforeCount = con.ItemCount.Value;
                result.Put(con.Take(count));

                count -= beforeCount - con.ItemCount.Value;
            }

            return result;
        }

        IItemContainer IItemAccessor.Take(int count) => ItemContainer.Empty;

        void IItemAccessor.CopyFrom(IItemContainerInfo itemContainer)
        {
            this.PrintWarning($"{nameof(IItemAccessor.CopyFrom)} not supported and was be mocked.");
        }

        IObservable<(int id, IItemContainer value)> IInventory.ObserveAdd()
        {
            addSubj ??= new Subject<(int id, IItemContainer value)>();

            return addSubj;
        }

        IObservable<(int id, IItemContainer value)> IInventory.ObserveRemove()
        {
            removeSubj ??= new Subject<(int id, IItemContainer value)>();

            return removeSubj;
        }
    }
}
