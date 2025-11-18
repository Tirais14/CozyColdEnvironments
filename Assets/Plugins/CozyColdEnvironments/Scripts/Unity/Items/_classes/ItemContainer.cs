using CCEnvs.Diagnostics;
using CCEnvs.FuncLanguage;
using System;
using UniRx;
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
        private readonly ReactiveProperty<bool> isActiveContainer = new();
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
        public bool IsEmpty => !ContainsItem();
        public bool IsFull => ItemCount >= Capacity;
        public bool IsActiveContainer => isActiveContainer.Value;
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
            Capacity = capacity;
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
            return Item.IsNotNull() && ItemCount > 0;
        }
        public bool ContainsItem(IItem? item)
        {
            if (!ContainsItem())
                return false;

            return Item.Contains(item);
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

        public Maybe<IItemContainer> PutItemFrom(IItemContainer itemContainer, int count)
        {
            CC.Guard.IsNotNull(itemContainer, nameof(itemContainer));

            if (itemContainer.Equals(this))
                return null!;

            return itemContainer.TakeItem(count)
                .Map(cnt => PutItem(
                    cnt.Item.GetValue(),
                    cnt.ItemCount).GetValue()
                    );
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

            int taked = Math.Clamp(count, 1, ItemCount);
            itemCount.Value -= taked;

            var result = new ItemContainer(Item.GetValue(), taked);

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
            return new ItemContainer(Item.GetValue(), ItemCount);
        }

        public void CopyFrom(IItemContainerInfo itemContainer)
        {
            item.Value = itemContainer.Item;
            itemCount.Value = itemContainer.ItemCount;
            capacity = itemContainer.Capacity;
        }

        public void Clear()
        {
            item.Value = null;
            itemCount.Value = 0;
        }

        public Maybe<int> GetContainerID()
        {
            return parentInventory.Map(x => x.GetContainerID(this)).Raw;
        }

        public override string ToString()
        {
            return $"{nameof(Item)}: {Item.Map(x => x.ToString()).GetValue("null")}; {nameof(ItemCount)}: {ItemCount}.";
        }

        public void ActivateContainer()
        {
            if (IsEmpty)
                return;

            parentInventory.IfSome(inv =>
            {
                inv.ActiveContainer.IfSome(cnt => cnt.DeactivateContainer());
                inv.ActiveContainer = this;
            });

            isActiveContainer.Value = true;

            this.PrintLog($"Activated. ID: {GetContainerID().Map(x => x.ToString()).GetValue("null")}");
        }

        public void DeactivateContainer()
        {
            isActiveContainer.Value = false;
            parentInventory.IfSome(inv => inv.ActiveContainer = null);

            this.PrintLog($"Deactivated. ID: {GetContainerID().Map(x => x.ToString()).GetValue("null")}");
        }

        public bool SwitchContainerActiveState()
        {
            if (isActiveContainer.Value)
                DeactivateContainer();
            else
                ActivateContainer();

            return isActiveContainer.Value;
        }

        public bool BindGameObject(GameObject gameObject)
        {
            return this.gameObject.BiMap(
                some: _ => false,
                none: () =>
                {
                    this.gameObject = gameObject;
                    return true;
                }).Raw;
        }

        public IObservable<Maybe<IItem>> ObserveItem() => item;

        public IObservable<bool> ObserveActiveState()
        {
            return isActiveContainer;
        }

        public IObservable<bool> ObserveDeactivateContainer()
        {
            return isActiveContainer.Where(x => !x);
        }

        public IObservable<bool> ObserveActivateContainer()
        {
            return isActiveContainer.Where(x => x);
        }

        public IObservable<Pair<int>> ObserveItemCount()
        {
            return itemCount.Pairwise();
        }

        public IObservable<Pair<int>> ObserveDecreasedItemCount()
        {
            return itemCount.Pairwise().Where(pair => pair.Current < pair.Previous);
        }

        public IObservable<Pair<int>> ObserveIncreaseItemCount()
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
                isActiveContainer.Dispose();
            }

            disposed = true;
        }
    }
}
