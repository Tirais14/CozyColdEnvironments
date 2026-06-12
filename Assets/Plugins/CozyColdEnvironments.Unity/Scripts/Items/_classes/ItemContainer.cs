using CCEnvs.Diagnostics;
using CCEnvs.FuncLanguage;
using R3;
using System;
using System.Threading;
using UnityEngine;

#nullable enable
#pragma warning disable S3236
#pragma warning disable S1144
namespace CCEnvs.UnityX.Items
{
    public class ItemContainer : IItemContainer, IDisposable
    {
        public static ItemContainer Empty { get; } = new(isReadOnlyContainer: true);

        private readonly ReactiveProperty<Maybe<IItem>> item = new();
        private readonly ReactiveProperty<int> itemCount = new();
        private readonly ReactiveProperty<bool> isActive = new();

        private Maybe<IInventory> parentInventory;

        private int capacity;

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
        public int FreeSpace => Math.Max(Capacity - ItemCount, 0);

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

        public ItemContainer(
            IItem? item = null,
            int count = 1,
            int capacity = 0,
            bool isReadOnlyContainer = false
            )
        {
            this.item.Value = item.Maybe()!;
            Capacity = capacity;
            IsReadOnlyContainer = isReadOnlyContainer;

            if (item.IsNull() && count > 0)
                count = 0;

            itemCount.Value = count;
        }

        ~ItemContainer() => Dispose();

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

        public Maybe<IItemContainerInfo> PutItem(IItem? inputItem, int count = 1)
        {
            if (inputItem.IsNull() || count <= 0)
                return Maybe<IItemContainerInfo>.None;

            if (IsFull
                ||
                (this.item.Value.TryGetValue(out var item) && item.ID != inputItem.ID))
            {
                return new ItemContainer(
                    inputItem,
                    count: count,
                    capacity: int.MaxValue,
                    isReadOnlyContainer: true
                    );
            }

            var restCount = FreeSpace - count;

            ItemContainer? restItems = null!;

            if (restCount < 0)
            {
                restItems = new ItemContainer(
                    inputItem,
                    count: Math.Abs(restCount),
                    capacity: int.MaxValue,
                    isReadOnlyContainer: true
                    );
            }

            itemCount.Value += count;

            if (Item.IsNone)
                this.item.Value = inputItem.Maybe();

            return restItems;
        }

        public Maybe<IItemContainerInfo> PutItemFrom(IItemContainer cnt, int count)
        {
            CC.Guard.IsNotNull(cnt, nameof(cnt));

            if (cnt.Equals(this)
                ||
                !cnt.TakeItem(count).TryGetValue(out var takedItems))
            {
                return Maybe<IItemContainerInfo>.None;
            }

            return PutItem(takedItems.Item.GetValue(), takedItems.ItemCount);
        }

        public Maybe<IItemContainerInfo> PutItemFrom(IItemContainer itemContainer)
        {
            CC.Guard.IsNotNull(itemContainer, nameof(itemContainer));

            return PutItemFrom(itemContainer, itemContainer.ItemCount);
        }

        public Maybe<IItemContainerInfo> TakeItem(int count)
        {
            if (Item.IsNone || count <= 0)
                return Maybe<IItemContainerInfo>.None;

            if (IsReadOnlyContainer)
                throw new InvalidOperationException($"Cannot take item from readonly container. Container: {this}");

            int taked = Math.Clamp(count, 1, ItemCount);

            itemCount.Value -= taked;

            var result = new ItemContainer(Item.GetValue(), taked);

            if (itemCount.Value <= 0)
                Clear();

            return result;
        }

        public Maybe<IItemContainerInfo> TakeItem() => TakeItem(itemCount.Value);

        public Maybe<IItemContainerInfo> TakeItem(IItem item, int count)
        {
            if (!ContainsItem(item))
                return Maybe<IItemContainerInfo>.None;

            return TakeItem(count);
        }

        public IItemContainer ShallowClone()
        {
            return new ItemContainer(Item.GetValue(), ItemCount, Capacity, IsReadOnlyContainer);
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
            return parentInventory.Map(inv => GetContainerID()).GetValue();
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

        public Observable<Maybe<IItem>> ObserveItem() => item;

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

    public class ItemContainer<TItem, TItemContainer, TItemContainerInfo>
        :
        IItemContainer<TItem, TItemContainerInfo>,
        IDisposable

        where TItem : IItem
        where TItemContainer : IItemContainer, TItemContainerInfo, new()
        where TItemContainerInfo : IItemContainerInfo<TItem>
    {
        private readonly ItemContainer internalContainer;

        public bool IsReadOnlyContainer => internalContainer.IsReadOnlyContainer;
        public bool UnlockCapacity {
            get => internalContainer.UnlockCapacity;
            set => internalContainer.UnlockCapacity = value;
        }
        public bool IsEmpty => internalContainer.IsEmpty;
        public bool IsFull => internalContainer.IsFull;

        public Maybe<TItem> Item => internalContainer.Item.Cast<TItem>();

        public int ItemCount => internalContainer.ItemCount;
        public int Capacity {
            get => internalContainer.Capacity;
            set => internalContainer.Capacity = value;
        }
        public int FreeSpace => internalContainer.FreeSpace;

        public Maybe<IInventory> ParentInventory {
            get => internalContainer.ParentInventory;
            set => internalContainer.ParentInventory = value;
        }

        public ItemContainer(
            TItem? item = default,
            int count = 1,
            int capacity = 0,
            bool isReadOnlyContainer = false
            )
        {
            internalContainer = new ItemContainer(item, count, capacity, isReadOnlyContainer);
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

        public Maybe<TItemContainerInfo> PutItem(TItem? item, int count = 1)
        {
            if (!internalContainer.PutItem(item, count).TryGetValue(out IItemContainerInfo? untypedRestItems))
                return Maybe<TItemContainerInfo>.None;

            var restItems = new TItemContainer
            {
                Capacity = untypedRestItems.Capacity
            };

            restItems.PutItem(
                untypedRestItems.Item.GetValue().As<TItem>(),
                untypedRestItems.ItemCount
                );

            return restItems;
        }

        public Maybe<TItemContainerInfo> PutItemFrom(IItemContainer<TItem> itemContainer)
        {
            if (!internalContainer.PutItemFrom(itemContainer).TryGetValue(out IItemContainerInfo? untypedRestItems))
                return Maybe<TItemContainerInfo>.None;

            var restItems = new TItemContainer
            {
                Capacity = untypedRestItems.Capacity
            };

            restItems.PutItem(
                untypedRestItems.Item.GetValue().As<TItem>(),
                untypedRestItems.ItemCount
                );

            return restItems;
        }
        public Maybe<TItemContainerInfo> PutItemFrom(
            IItemContainer<TItem> itemContainer,
            int count
            )
        {
            if (!internalContainer.PutItemFrom(itemContainer, count).TryGetValue(out IItemContainerInfo? untypedRestItems))
                return Maybe<TItemContainerInfo>.None;

            var restItems = new TItemContainer
            {
                Capacity = untypedRestItems.Capacity
            };

            restItems.PutItem(
                untypedRestItems.Item.GetValue().As<TItem>(),
                untypedRestItems.ItemCount
                );

            return restItems;
        }

        public IItemContainer ShallowClone()
        {
            return new ItemContainer<TItem, TItemContainer, TItemContainerInfo>(
                item: Item.GetValue(),
                count: ItemCount,
                capacity: Capacity,
                isReadOnlyContainer: IsReadOnlyContainer
                )
            {
                Capacity = Capacity,
                ParentInventory = ParentInventory,
                UnlockCapacity = UnlockCapacity
            };
        }

        public Maybe<TItemContainerInfo> TakeItem()
        {
            if (!internalContainer.TakeItem().TryGetValue(out IItemContainerInfo? untypedTakedItems))
                return Maybe<TItemContainerInfo>.None;

            var restItems = new TItemContainer
            {
                Capacity = untypedTakedItems.Capacity
            };

            restItems.PutItem(
                untypedTakedItems.Item.GetValue().As<TItem>(), 
                untypedTakedItems.ItemCount
                );

            return restItems;
        }
        public Maybe<TItemContainerInfo> TakeItem(int count)
        {
            if (!internalContainer.TakeItem(count).TryGetValue(out IItemContainerInfo? untypedTakedItems))
                return Maybe<TItemContainerInfo>.None;

            var restItems = new TItemContainer
            {
                Capacity = untypedTakedItems.Capacity
            };

            restItems.PutItem(
                untypedTakedItems.Item.GetValue().As<TItem>(),
                untypedTakedItems.ItemCount
                );

            return restItems;
        }
        public Maybe<TItemContainerInfo> TakeItem(TItem item, int count)
        {
            if (!internalContainer.TakeItem(item, count).TryGetValue(out IItemContainerInfo? untypedTakedItems))
                return Maybe<TItemContainerInfo>.None;

            var restItems = new TItemContainer
            {
                Capacity = untypedTakedItems.Capacity
            };

            restItems.PutItem(
                untypedTakedItems.Item.GetValue().As<TItem>(),
                untypedTakedItems.ItemCount
                );

            return restItems;
        }

        public void Dispose() => internalContainer.Dispose();

        public Observable<Maybe<TItem>> ObserveItem()
        {
            return internalContainer.ObserveItem().Select(item => item.Cast<TItem>());
        }

        public Observable<int> ObserveItemCount() => internalContainer.ObserveItemCount();
    }
}
