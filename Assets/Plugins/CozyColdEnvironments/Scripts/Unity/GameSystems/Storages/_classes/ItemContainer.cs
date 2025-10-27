using CCEnvs.Diagnostics;
using CCEnvs.Language;
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
        private readonly ReactiveProperty<Maybe<IItem>> item = new();
        private readonly ReactiveProperty<int> itemCount = new();
        private int capacity;

        public static ItemContainer Empty => new();

        public IReadOnlyReactiveProperty<Maybe<IItem>> Item => item;
        public IReadOnlyReactiveProperty<int> ItemCount => itemCount;
        public int Capacity {
            get => Mathf.Min(item.Value.Map(item => item.MaxItemCount).Access(int.MaxValue), capacity);
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
        public bool UnlockCapacity { get; set; }
        public Maybe<GameObject?> gameObject { get; private set; }

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
            this.item.Value = item.Maybe()!;

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

        public Maybe<IItemContainer> Put(IItem? item, int count)
        {
            if (item.IsNull() || count <= 0)
                return null!;
            if (IsFull
                ||
                (!IsEmpty  && !Contains(item))
                )
                return new ItemContainer(item, count);

            int mergedItemCount = ItemCount.Value + count;
            if (mergedItemCount > Capacity)
            {
                itemCount.Value = Capacity;

                return new ItemContainer(item, Capacity - mergedItemCount);
            }

            itemCount.Value = mergedItemCount;
            return null!;
        }

        public Maybe<IItemContainer> Put(IItemContainer itemContainer, int count)
        {
            CC.Guard.NullArgument(itemContainer, nameof(itemContainer));

            return Put(itemContainer.Item.Value.Access(), count);
        }

        public Maybe<IItemContainer> Put(IItemContainer itemContainer)
        {
            CC.Guard.NullArgument(itemContainer, nameof(itemContainer));

            return Put(itemContainer.Item.Value.Access(), itemContainer.ItemCount.Value);
        }

        public Maybe<IItemContainer> Take(int count)
        {
            if (Item.Value.IsNone || count <= 0)
                return null!;

            int taked = Math.Clamp(count, 0, ItemCount.Value);
            itemCount.Value -= taked;

            var result = new ItemContainer(Item.Value.Access(), taked);

            if (itemCount.Value <= 0)
                Clear();

            return result;
        }

        public Maybe<IItemContainer> Take() => Take(itemCount.Value);

        public Maybe<IItemContainer> Take(IItem item, int count)
        {
            if (!Contains(item))
                return Empty;

            return Take(count);
        }

        public IItemContainer ShallowClone()
        {
            return new ItemContainer(Item.Value.Access(), ItemCount.Value);
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

        public override string ToString()
        {
            return $"{nameof(Item)}: {Item.Value.Map(x => x.ToString()).Access("null")}; {nameof(ItemCount)}: {ItemCount.Value}.";
        }
    }
}
