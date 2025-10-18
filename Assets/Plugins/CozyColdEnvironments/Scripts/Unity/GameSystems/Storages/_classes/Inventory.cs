using CCEnvs.Diagnostics;
using CCEnvs.Linq;
using System.Collections;
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

        public IItemContainer this[int key] => inner[key];

        public IEnumerable<int> Keys => inner.Keys;
        public IEnumerable<IItemContainer> Values => inner.Values;
        public int Capacity => inner.Count;
        public bool IsEmpty => inner.Values.Any(x => x.Contains());
        public bool IsFull => inner.Values.All(x => x.Contains());

        int IReadOnlyCollection<KeyValuePair<int, IItemContainer>>.Count => inner.Count;
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

        public Inventory(IEnumerable<(int id, IItemContainer value)> containers)
        {
            CC.Guard.NullArgument(containers, nameof(containers));

            inner = new Dictionary<int, IItemContainer>(containers.AsKeyValuePairs());
        }

        public void Add(int id, IItemContainer itemContainer)
        {
            CC.Guard.NullArgument(itemContainer, nameof(itemContainer));

            inner.Add(id, itemContainer);
        }

        public void Clear() => inner.Clear();

        public bool ContainsKey(int key) => inner.ContainsKey(key);

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
            return inner.ContainsValue(itemContainer);
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

        public void Remove(int id) => inner.Remove(id);

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

        public bool TryGetValue(int key, out IItemContainer value)
        {
            return inner.TryGetValue(key, out value);
        }

        public IEnumerator<KeyValuePair<int, IItemContainer>> GetEnumerator()
        {
            return inner.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        IItemContainer IItemAccessor.Take(int count) => ItemContainer.Empty;

        void IItemAccessor.CopyFrom(IItemContainerInfo itemContainer)
        {
            this.PrintWarning($"{nameof(IItemAccessor.CopyFrom)} not supported and was be mocked.");
        }
    }
}
