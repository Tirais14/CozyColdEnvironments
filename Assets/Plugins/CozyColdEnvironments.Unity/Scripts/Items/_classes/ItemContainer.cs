using System;
using CCEnvs.Collections;
using CCEnvs.FuncLanguage;
using R3;
using UnityEngine;

#nullable enable
#pragma warning disable S3236
#pragma warning disable S1144
namespace CCEnvs.Unity.Items
{
    public class ItemContainer : IItemContainer, IDisposable
    {
        private readonly ReactiveProperty<Maybe<IItem>> item = new();
        private readonly ReactiveProperty<int> itemCount = new();
        private readonly ReactiveProperty<bool> isActive = new();
        private Maybe<IInventory> parentInventory;
        private int capacity;

        public static ItemContainer Empty => new();

        public Maybe<IItem> Item => item.Value;
        public int ItemCount => itemCount.Value;
        public int Capacity {
            get
            {
                if (UnlockCapacity)
                    return int.MaxValue;

                return Mathf.Min(item.Value.Map(item => item.MaxItemCount).GetValue(int.MaxValue), capacity);
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
        public int FreeSpace => Math.Clamp(Capacity - ItemCount, min: 0, max: int.MaxValue);
        public bool IsEmpty => !ContainsItem();
        public bool IsFull => ItemCount >= Capacity;
        public bool IsActive => isActive.Value;
        /// <summary>
        /// If true ignores <see cref="IItem.MaxItemCount"/>
        /// </summary>
        public bool UnlockCapacity { get; set; }
        public bool IsReadOnlyContainer { get; }

#pragma warning disable S2292
        //TODO: Remove and Add to new item container parent
        public Maybe<IInventory> ParentInventory {
            get => parentInventory;
            set => parentInventory = value;
        }
#pragma warning restore S2292

        public ItemContainer()
            :
            this(capacity: int.MaxValue)
        {
        }

        public ItemContainer(IItem? item = null, int count = 1, int capacity = 0, bool isReadOnlyContainer = false)
        {
            this.item.Value = item.Maybe()!;
            Capacity = capacity;
            IsReadOnlyContainer = isReadOnlyContainer;

            if (item.IsNull() && count > 0)
                count = 0;

            itemCount.Value = count;
        }

        public bool ContainsItem()
        {
            return Item.IsNotNull() && ItemCount > 0;
        }
        public bool ContainsItem(IItem? item)
        {
            if (!ContainsItem())
                return false;

            return Item.Has(item);
        }
        public bool ContainsItem(IItem? item, int count)
        {
            if (!ContainsItem(item))
                return false;

            return ItemCount >= count;
        }

        public Maybe<IItemContainer> PutItem(IItem? item, int count = 1)
        {
            if (item.IsNull() || count <= 0)
                return Maybe<IItemContainer>.None;

            if (IsFull || (!IsEmpty && !ContainsItem(item)) || IsReadOnlyContainer)
                return new ItemContainer(item, count);

            int addedCount = Math.Clamp(count, 0, Capacity - itemCount.Value);
            this.item.Value.IfNone(() => this.item.Value = item.Maybe());

            ItemContainer? restItems = null;
            if (count - addedCount is int deltaCount && deltaCount > 0)
                restItems = new ItemContainer(item, deltaCount);

            itemCount.Value += addedCount;
            return restItems;
        }

        public Maybe<IItemContainer> PutItemFrom(IItemContainer itemContainer, int count)
        {
            CC.Guard.IsNotNull(itemContainer, nameof(itemContainer));

            if (itemContainer.Equals(this))
                return null!;

            if (!itemContainer.TakeItem(count).TryGetValue(out var taked))
                return Maybe<IItemContainer>.None;

            return PutItem(taked.Item.Raw, taked.ItemCount);
        }

        public Maybe<IItemContainer> PutItemFrom(IItemContainer itemContainer)
        {
            CC.Guard.IsNotNull(itemContainer, nameof(itemContainer));

            return PutItemFrom(itemContainer, itemContainer.ItemCount);
        }

        public Maybe<IItemContainer> TakeItem(int count)
        {
            if (Item.IsNone || count <= 0)
                return null!;

            if (IsReadOnlyContainer)
                return ShallowClone().Maybe();

            int taked = Math.Clamp(count, 1, ItemCount);
            itemCount.Value -= taked;

            var result = new ItemContainer(Item.GetValue(), taked);

            if (itemCount.Value <= 0)
                Reset();

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
            return new ItemContainer(Item.GetValue(), ItemCount, Capacity, IsReadOnlyContainer);
        }

        public void CopyFrom(IItemContainerInfo itemContainer)
        {
            item.Value = itemContainer.Item;
            itemCount.Value = itemContainer.ItemCount;
            capacity = itemContainer.Capacity;
        }

        public void Reset()
        {
            item.Value = null;
            itemCount.Value = 0;
        }

        public Maybe<int> GetContainerID()
        {
            return parentInventory.Map(inv => inv.GetKey(this)).Raw;
        }

        public override string ToString()
        {
            return $"{nameof(Item)}: {Item.Map(x => x.ToString()).GetValue("null")}; {nameof(ItemCount)}: {ItemCount}.";
        }

        public void Activate()
        {
            if (IsEmpty)
                return;

            isActive.Value = true;

            this.PrintLog($"Activated. ID: {GetContainerID().Map(x => x.ToString()).GetValue("null")}");
        }

        public void Deactivate()
        {
            isActive.Value = false;

            this.PrintLog($"Deactivated. ID: {GetContainerID().Map(x => x.ToString()).GetValue("null")}");
        }

        public bool SwitchActiveState()
        {
            if (isActive.Value)
                Deactivate();
            else
                Activate();

            return isActive.Value;
        }

        public bool CanPut() => !IsFull;

        public bool CanPut(IItem? item) => ContainsItem(item);

        public bool CanPut(IItem? item, int count)
        {
            if (!CanPut(item))
                return false;

            count = Math.Clamp(count, min: 0, max: int.MaxValue);

            if (FreeSpace < count)
                return false;

            return true;
        }

        public Observable<Maybe<IItem>> ObserveItem() => item;

        public Observable<bool> ObserveActiveState()
        {
            return isActive;
        }

        public Observable<bool> ObserveDeactivate()
        {
            return isActive.Where(x => !x);
        }

        public Observable<bool> ObserveActivate()
        {
            return isActive.Where(x => x);
        }

        public Observable<int> ObserveItemCount()
        {
            return itemCount;
        }

        public Observable<(int Previous, int Current)> ObserveDecreasedItemCount()
        {
            return itemCount.Pairwise().Where(pair => pair.Current < pair.Previous);
        }

        public Observable<(int Previous, int Current)> ObserveIncreaseItemCount()
        {
            return itemCount.Pairwise().Where(pair => pair.Current > pair.Previous);
        }

        public void Dispose() => Dispose(disposing: true);

        private bool disposed;
        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
                return;

            if (disposing)
            {
                item.Dispose();
                itemCount.Dispose();
                isActive.Dispose();
            }

            disposed = true;
        }

    }
}
