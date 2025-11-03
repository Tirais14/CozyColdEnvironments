using CCEnvs.Diagnostics;
using CCEnvs.FuncLanguage;
using CommunityToolkit.Diagnostics;
using System;
using UniRx;
using UnityEngine;

#nullable enable
#pragma warning disable S3236
#pragma warning disable S1144
namespace CCEnvs.Unity.Storages
{
    public class ItemContainer : IItemContainer
    {
        private readonly ReactiveProperty<Maybe<IItem>> item = new();
        private readonly ReactiveProperty<int> itemCount = new();
        private readonly ReactiveProperty<bool> isActiveContainer = new();
        private Maybe<IInventory> parentInventory;
        private int capacity;

        public static ItemContainer Empty => new();

        public IReadOnlyReactiveProperty<Maybe<IItem>> Item => item;
        public IReadOnlyReactiveProperty<int> ItemCount => itemCount;
        public int Capacity {
            get
            {
                if (UnlockCapacity)
                    return int.MaxValue;

                return Mathf.Min(item.Value.Map(item => item.MaxItemCount).Access(int.MaxValue), capacity);
            }
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
        public bool IsEmpty => !ContainsItem();
        public bool IsFull => ItemCount.Value >= Capacity;
        public IReadOnlyReactiveProperty<bool> IsActiveContainer => isActiveContainer;
        /// <summary>
        /// If true ignores <see cref="IItem.MaxItemCount"/>
        /// </summary>
        public bool UnlockCapacity { get; set; }
        public Maybe<GameObject> gameObject { get; private set; }
#pragma warning disable S2292
        //TODO: Remove and Add to new item container parent
        public Maybe<IInventory> ParentInventory {
            get => parentInventory;
            set => parentInventory = value;
        }
#pragma warning restore S2292

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

        public ItemContainer(IItem? item, int capcacity, int count = 1)
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

        public ItemContainer(IItem? item, int count = 1)
            :
            this(item, int.MaxValue, count: count)
        {

        }

        public bool ContainsItem()
        {
            return Item.IsNotNull() && ItemCount.Value > 0;
        }
        public bool ContainsItem(IItem? item)
        {
            if (!ContainsItem())
                return false;

            return Item.Value.ItIs(item);
        }
        public bool ContainsItem(IItem? item, int count)
        {
            if (!ContainsItem(item))
                return false;

            return ItemCount.Value >= count;
        }

        public Maybe<IItemContainer> PutItem(IItem? item, int count = 1)
        {
            if (item.IsNull() || count <= 0)
                return Maybe<IItemContainer>.None;
            if (IsFull
                ||
                (!IsEmpty  && !ContainsItem(item))
                )
                return new ItemContainer(item, count);

            int addedCount = Math.Clamp(count, 0, Capacity - itemCount.Value);
            this.item.Value.IfNone(() => this.item.Value = item.Maybe());

            ItemContainer? restItems = null;
            if (count - addedCount is int deltaCount && deltaCount > 0)
                restItems = new ItemContainer(item, deltaCount);

            itemCount.Value += addedCount;
            return restItems;
        }

        public Maybe<IItemContainer> PutItem(IItemContainer itemContainer, int count)
        {
            CC.Guard.IsNotNull(itemContainer, nameof(itemContainer));

            if (itemContainer.Equals(this))
                return null!;

            return itemContainer.TakeItem(count)
                                .Map(cnt => PutItem(
                cnt.Item.Value.Access(),
                cnt.ItemCount.Value).Access()
                );
        }

        public Maybe<IItemContainer> PutItem(IItemContainer itemContainer)
        {
            CC.Guard.IsNotNull(itemContainer, nameof(itemContainer));

            return PutItem(itemContainer, itemContainer.ItemCount.Value);
        }

        public Maybe<IItemContainer> TakeItem(int count)
        {
            if (Item.Value.IsNone || count <= 0)
                return null!;

            int taked = Math.Clamp(count, 1, ItemCount.Value);
            itemCount.Value -= taked;

            var result = new ItemContainer(Item.Value.Access(), taked);

            if (itemCount.Value <= 0)
                Clear();

            return result;
        }

        public Maybe<IItemContainer> TakeItem() => TakeItem(itemCount.Value);

        public Maybe<IItemContainer> TakeItem(IItem item, int count)
        {
            if (!ContainsItem(item))
                return Empty;

            return TakeItem(count);
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

        public MaybeStruct<int> GetContainerID()
        {
            return parentInventory.Map(x => x.GetContainerID(this)).Target;
        }

        public override string ToString()
        {
            return $"{nameof(Item)}: {Item.Value.Map(x => x.ToString()).Access("null")}; {nameof(ItemCount)}: {ItemCount.Value}.";
        }

        public void ActivateContainer()
        {
            isActiveContainer.Value = true;

            this.PrintLog($"Activated. ID: {GetContainerID().Map(x => x.ToString()).Access("null")}");
        }

        public void DeactivateContainer()
        {
            isActiveContainer.Value = false;

            this.PrintLog($"Deactivated. ID: {GetContainerID().Map(x => x.ToString()).Access("null")}");
        }

        public bool SwitchContainerActiveState()
        {
            if (isActiveContainer.Value)
                DeactivateContainer();
            else
                ActivateContainer();

            return isActiveContainer.Value;
        }
    }
}
