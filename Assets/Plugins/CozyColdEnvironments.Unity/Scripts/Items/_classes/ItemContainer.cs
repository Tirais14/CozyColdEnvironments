using CCEnvs.Diagnostics;
using CCEnvs.FuncLanguage;
using CCEnvs.TypeMatching;
using R3;
using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

#nullable enable
namespace CCEnvs.UnityX.Items
{
    public class ItemContainer : IItemContainer, IDisposable
    {
        private readonly ReactiveProperty<IItem?> item = new();
        private readonly ReactiveProperty<int> itemCount = new();
        private readonly ReactiveProperty<bool> isActive = new();

        private IInventory? parentInventory;

        private int capacity;

        public IItem? Item => item.Value;
        public int ItemCount => itemCount.Value;
        public int Capacity {
            get
            {
                if (IgnoreMaxItemCount)
                    return int.MaxValue;

                return Math.Min(item.Value.Maybe().Map(item => item.MaxItemCount).GetValue(int.MaxValue), capacity);
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
        public int FreeSpace => Math.Max(Capacity - ItemCount, 0);

        public bool IsEmpty => !ContainsItem();
        public bool IsFull => ItemCount >= Capacity;
        public bool IsActive => isActive.Value;
        /// <summary>
        /// If true ignores <see cref="IItem.MaxItemCount"/>
        /// </summary>
        public bool IgnoreMaxItemCount { get; set; }

        //TODO: Remove and Add to new item container parent
        public IInventory? ParentInventory {
            get => parentInventory;
            set => parentInventory = value;
        }

        bool IItemContainer.IsReadOnlyContainer { get; }

        public ItemContainer()
            :
            this(capacity: int.MaxValue)
        {
        }

        public ItemContainer(
            IItem? item = null,
            int count = 1,
            int capacity = 0
            )
        {
            this.item.Value = item;
            Capacity = capacity;

            if (item.IsNull() && count > 0)
                count = 0;

            itemCount.Value = count;
        }

        public static explicit operator ReadOnlyItemContainer(ItemContainer instance)
        {
            return instance.ToReadOnly();
        }

        ~ItemContainer() => Dispose();

        public bool ContainsItem()
        {
            return ItemCount >= 1 && Item.IsNotNull();
        }
        public bool ContainsItem(IItem? item)
        {
            if (!ContainsItem())
                return false;

            return EqualityComparer<IItem?>.Default.Equals(Item, item);
        }
        public bool ContainsItem(IItem? item, int count)
        {
            if (!ContainsItem(item))
                return false;

            return ItemCount >= count;
        }

        public ReadOnlyItemContainer PutItem(IItem? inputItem, int count = 1)
        {
            if (count <= 0 || inputItem.IsNull())
                return ReadOnlyItemContainer.Empty;

            if (IsFull || (Item.IsNotNull() && !EqualityComparer<IItem?>.Default.Equals(Item, inputItem)))
                return new ReadOnlyItemContainer(inputItem, count);

            item.Value = inputItem;
            int toPutCount = Math.Clamp(count, 0, FreeSpace);
            itemCount.Value += toPutCount;

            int restCount = count - toPutCount;

            if (restCount <= 0)
                return ReadOnlyItemContainer.Empty;

            return new ReadOnlyItemContainer(Item, restCount);
        }
        public ReadOnlyItemContainer PutItem(IItemContainerInfo? containerInfo)
        {
            if (containerInfo.IsNull())
                return ReadOnlyItemContainer.Empty;

            return PutItem(containerInfo.Item, containerInfo.ItemCount);
        }
        public ReadOnlyItemContainer PutItem<TItemContainerInfo>(TItemContainerInfo containerInfo)
            where TItemContainerInfo : struct, IItemContainerInfo
        {
            return PutItem(containerInfo.Item, containerInfo.ItemCount);
        }

        public ReadOnlyItemContainer PutItemFrom(IItemContainer? container, int count)
        {
            if (count <= 0 || container.IsNull() || this == container)
                return ReadOnlyItemContainer.Empty;

            return container.PutItem(PutItem(container.TakeItem(count)));
        }

        public ReadOnlyItemContainer PutItemFrom(IItemContainer? container)
        {
            if (container.IsNull())
                return ReadOnlyItemContainer.Empty;

            return PutItemFrom(container, container.ItemCount);
        }

        public ReadOnlyItemContainer TakeItem(int count)
        {
            if (IsEmpty || count <= 0)
                return ReadOnlyItemContainer.Empty;

            int takenCount = Math.Min(count, ItemCount);
            itemCount.Value -= takenCount;

            return new ReadOnlyItemContainer(Item, takenCount);
        }

        public ReadOnlyItemContainer TakeItem() => TakeItem(itemCount.Value);

        public ReadOnlyItemContainer TakeItem(IItem? item, int count)
        {
            if (!ContainsItem(item))
                return ReadOnlyItemContainer.Empty;

            return TakeItem(count);
        }

        public ReadOnlyItemContainer ToReadOnly() => new(Item, ItemCount);

        public IItemContainer ShallowClone()
        {
            return new ItemContainer(Item, ItemCount, Capacity);
        }

        public void CopyItemFrom(IItemContainerInfo itemContainer)
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
            return parentInventory.Maybe().Map(inv => GetContainerID()).GetValue();
        }

        public override string ToString()
        {
            return ToStringBuilder.CreatePooled()
                .AddProperty(nameof(Item), Item)
                .AddProperty(nameof(ItemCount), ItemCount)
                .ToStringAndDispose();
        }

        public void Activate()
        {
            if (IsEmpty)
                return;

            isActive.Value = true;

#if CC_DEBUG_ENABLED
            if (CCDebug.Instance.IsEnabled)
                this.PrintLog($"Activated. ID: {GetContainerID().Map(x => x.ToString()).GetValue("null")}");
#endif
        }

        public void Deactivate()
        {
            isActive.Value = false;

#if CC_DEBUG_ENABLED
            if (CCDebug.Instance.IsEnabled)
                this.PrintLog($"Deactivated. ID: {GetContainerID().Map(x => x.ToString()).GetValue("null")}");
#endif
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

        public Observable<IItem?> ObserveItem() => item;

        public Observable<bool> ObserveIsActive() => isActive;

        public Observable<int> ObserveItemCount() => itemCount;

        private int disposed;
        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
        protected virtual void Dispose(bool disposing)
        {
            if (Interlocked.Exchange(ref disposed, 1) != 0)
                return;

            if (disposing)
            {
                item.Dispose();
                itemCount.Dispose();
                isActive.Dispose();
            }
        }
    }

    public class ItemContainer<TItem>
        :
        IItemContainer<TItem>,
        IDisposable

        where TItem : IItem
    {
        private readonly ItemContainer internalContainer;

        public bool IgnoreMaxItemCount {
            get => internalContainer.IgnoreMaxItemCount;
            set => internalContainer.IgnoreMaxItemCount = value;
        }
        public bool IsEmpty => internalContainer.IsEmpty;
        public bool IsFull => internalContainer.IsFull;

        public TItem? Item => (TItem?)internalContainer.Item;

        public int ItemCount => internalContainer.ItemCount;
        public int Capacity {
            get => internalContainer.Capacity;
            set => internalContainer.Capacity = value;
        }
        public int FreeSpace => internalContainer.FreeSpace;

        public IInventory? ParentInventory {
            get => internalContainer.ParentInventory;
            set => internalContainer.ParentInventory = value;
        }

        bool IItemContainer.IsReadOnlyContainer => false;

        public ItemContainer(
            TItem? item = default,
            int count = 1,
            int capacity = 0
            )
        {
            internalContainer = new ItemContainer(item, count, capacity);
        }

        public ItemContainer()
            :
            this(capacity: int.MaxValue)
        {
        }

        ~ItemContainer() => Dispose();

        public bool CanPut() => internalContainer.CanPut();
        public bool CanPut(IItem? item) => internalContainer.CanPut(item);
        public bool CanPut(IItem? item, int count) => internalContainer.CanPut(item, count);

        public void Clear() => internalContainer.Clear();

        public bool ContainsItem() => internalContainer.ContainsItem();
        public bool ContainsItem(IItem? item) => internalContainer.ContainsItem(item);
        public bool ContainsItem(IItem? item, int count) => internalContainer.ContainsItem(item, count);

        public void CopyItemFrom(IItemContainerInfo<TItem> itemContainer)
        {
            internalContainer.CopyItemFrom(itemContainer);
        }

        public Maybe<int> GetContainerID() => internalContainer.GetContainerID();

        public ReadOnlyItemContainer<TItem> PutItem(TItem? item, int count = 1)
        {
            return internalContainer.PutItem(item, count).Convert<TItem>();
        }
        public ReadOnlyItemContainer<TItem> PutItem(IItemContainerInfo<TItem>? containerInfo)
        {
            return internalContainer.PutItem(containerInfo).Convert<TItem>();
        }
        public ReadOnlyItemContainer<TItem> PutItem<TItemContainerInfo>(TItemContainerInfo containerInfo) where TItemContainerInfo : struct, IItemContainerInfo<TItem>
        {
            return internalContainer.PutItem(containerInfo).Convert<TItem>();
        }

        public ReadOnlyItemContainer<TItem> PutItemFrom(IItemContainer<TItem>? container)
        {
            return internalContainer.PutItemFrom(container).Convert<TItem>();
        }
        public ReadOnlyItemContainer<TItem> PutItemFrom(
            IItemContainer<TItem>? container,
            int count
            )
        {
            return internalContainer.PutItemFrom(container).Convert<TItem>();
        }

        public ReadOnlyItemContainer<TItem> ToReadOnly() => new(Item, ItemCount);

        public IItemContainer ShallowClone()
        {
            return new ItemContainer<TItem>(
                item: Item,
                count: ItemCount,
                capacity: Capacity
                )
            {
                Capacity = Capacity,
                ParentInventory = ParentInventory,
                IgnoreMaxItemCount = IgnoreMaxItemCount
            };
        }

        public ReadOnlyItemContainer<TItem> TakeItem()
        {
            return internalContainer.TakeItem().Convert<TItem>();
        }
        public ReadOnlyItemContainer<TItem> TakeItem(int count)
        {
            return internalContainer.TakeItem(count).Convert<TItem>();
        }
        public ReadOnlyItemContainer<TItem> TakeItem(TItem? item, int count)
        {
            return internalContainer.TakeItem(item, count).Convert<TItem>();
        }

        public void Dispose() => internalContainer.Dispose();

        public Observable<TItem?> ObserveItem()
        {
            return internalContainer.ObserveItem().Select(item => (TItem?)item);
        }

        public Observable<int> ObserveItemCount() => internalContainer.ObserveItemCount();
    }
}
