using CCEnvs.Diagnostics;
using CCEnvs.Language;
using CCEnvs.Unity.UI.MVVM;
using CommunityToolkit.Diagnostics;
using System;
using UniRx;
using UnityEngine;

#nullable enable
#pragma warning disable S3236
#pragma warning disable S1144
namespace CCEnvs.Unity.GameSystems.Storages
{
    public class ItemContainer : IItemContainer
    {
        private readonly ReactiveProperty<IItem?> item = new();
        private readonly ReactiveProperty<int> itemCount = new();
        private int capacity;

        public static ItemContainer Empty => new();

        public IReadOnlyReactiveProperty<IItem?> Item => item;
        public IReadOnlyReactiveProperty<int> ItemCount => itemCount;
        public int Capacity {
            get => Mathf.Min(capacity, Item.Value?.MaxItemCount ?? int.MaxValue);
            set
            {
                if (value < 0)
                {
                    capacity = 0;
                    return;
                }

                capacity = value;
            }
        }
        public bool IsEmpty => !Contains();
        public bool IsFull => ItemCount.Value >= Capacity;
        /// <summary>
        /// If true ignores <see cref="IItem.MaxItemCount"/>
        /// </summary>
        public bool Unlocked { get; set; }
        public Ghost<GameObject?> gameObject { get; private set; }

        public ItemContainer(int capacity)
        {
            Guard.IsGreaterThan(capacity, -1, nameof(capacity));

            this.capacity = capacity;
        }

        public ItemContainer()
            :
            this(int.MaxValue)
        {
        }

        public ItemContainer(IItem? item, int count, int capcacity)
            :
            this(capcacity)
        {
            this.item.Value = item;

            if (item.IsNull() && count > 0)
            {
                count = 0;
                this.PrintWarning("Trying to add null item with non-zero count.");
            }

            itemCount.Value = count;
        }

        public ItemContainer(IItem? item, int count)
            :
            this(item, count, int.MaxValue)
        {

        }

        public bool Contains()
        {
            return Item.IsNotNull() && ItemCount.Value > 0;
        }
        public bool Contains(IItem? item)
        {
            if (!Contains())
                return false;

            return Item.Equals(item);
        }
        public bool Contains(IItem? item, int count)
        {
            if (!Contains(item))
                return false;

            return ItemCount.Value >= count;
        }

        public IItemContainer Put(IItem? item, int count)
        {
            if (item.IsNull() || count <= 0)
                return Empty;

            if (Capacity - ItemCount.Value <= 0)
                return new ItemContainer(item, count);

            int mergedItemCount = ItemCount.Value + count;

            if (mergedItemCount > Capacity)
            {
                itemCount.Value = Capacity;

                return new ItemContainer(item, Capacity - mergedItemCount);
            }

            itemCount.Value = mergedItemCount;
            return Empty;
        }

        public IItemContainer Put(IItemContainer itemContainer, int count)
        {
            CC.Guard.NullArgument(itemContainer, nameof(itemContainer));

            return Put(itemContainer.Item.Value, count);
        }

        public IItemContainer Put(IItemContainer itemContainer)
        {
            CC.Guard.NullArgument(itemContainer, nameof(itemContainer));

            return Put(itemContainer.Item.Value, itemContainer.ItemCount.Value);
        }

        public IItemContainer Take(int count)
        {
            if (count >= ItemCount.Value)
            {
                Clear();
                return new ItemContainer(Item.Value, ItemCount.Value);
            }

            itemCount.Value -= count;

            if (itemCount.Value <= 0)
                Clear();

            return new ItemContainer(Item.Value, count);
        }

        public IItemContainer Take(IItem item, int count)
        {
            if (!Contains(item))
                return Empty;

            return Take(count);
        }

        public IItemContainer ShallowClone()
        {
            return new ItemContainer(Item.Value, ItemCount.Value);
        }

        public void CopyFrom(IItemContainerInfo itemContainer)
        {
            item.Value = itemContainer.Item.Value;
            itemCount.Value = itemContainer.ItemCount.Value;
            capacity = itemContainer.Capacity;
        }

        public void Clear()
        {
            item.Value = null;
            itemCount.Value = 0;
        }
    }
}
