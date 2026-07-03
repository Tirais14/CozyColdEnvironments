using CCEnvs.Diagnostics;
using CCEnvs.FuncLanguage;
using R3;
using System;
using System.Collections.Generic;
using System.Threading;

#nullable enable
namespace CCEnvs.UnityX.Items
{
    public abstract class ItemContainerBase
        <
        TItem,
        TInputItemContainer,
        TInputItemContainerInfo,
        TReadOnlyContainer,
        TLargeReadOnlyContainer,
        TPutItemEvent,
        TTakeItemEvent
        >
        :
        IDisposable

        where TItem : class, IItem
        where TInputItemContainer : IItemContainer
        where TInputItemContainerInfo : IItemContainerInfo
        where TReadOnlyContainer : struct, IItemContainerInfo
        where TLargeReadOnlyContainer : struct
    {
        private readonly ReactiveProperty<TItem?> item = new();

        private readonly ReactiveProperty<int> itemCount = new();

        private readonly ReactiveProperty<bool> isActive = new();

        private ReactiveCommand<TPutItemEvent>? onPutItem;

        private ReactiveCommand<TTakeItemEvent>? onTakeItem;

        private IInventory? parentInventory;

        private int capacity;
        private int? id;

        public TItem? Item => item.Value;
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
        public int? ID {
            get => id;
            set
            {
                if (parentInventory is null)
                    id = value;
            }
        }

        public bool IsEmpty => !ContainsItem();
        public bool IsFull => ItemCount >= Capacity;
        public bool IsActive => isActive.Value;
        /// <summary>
        /// If true ignores <see cref="IItem.MaxItemCount"/>
        /// </summary>
        public bool IgnoreMaxItemCount { get; set; }

        public IInventory? ParentInventory => parentInventory;

        public ItemContainerBase()
            :
            this(capacity: int.MaxValue)
        {
        }

        public ItemContainerBase(
            TItem? item = null,
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

        ~ItemContainerBase() => Dispose();

        public bool ContainsItem()
        {
            return ItemCount >= 1 && Item.IsNotNull();
        }
        public bool ContainsItem(IItem? item)
        {
            return ContainsItem() && item is TItem typedItem && EqualityComparer<TItem?>.Default.Equals(Item, typedItem);
        }
        public bool ContainsItem(IItem? item, int count)
        {
            if (!ContainsItem(item))
                return false;

            return ItemCount >= count;
        }

        public TReadOnlyContainer PutItem(TItem? inputItem, int count = 1)
        {
            if (count <= 0 || inputItem.IsNull())
                return CreateReadOnlyContainer();

            if (IsFull || (Item.IsNotNull() && !EqualityComparer<IItem?>.Default.Equals(Item, inputItem)))
                return CreateReadOnlyContainer(inputItem, count);

            this.item.Value = inputItem;
            int toPutCount = Math.Clamp(count, 0, FreeSpace);
            itemCount.Value += toPutCount;

            if (onPutItem is not null && toPutCount >= 1)
                onPutItem.Execute(CreatePutItemEvent(inputItem, toPutCount));

            int restCount = count - toPutCount;

            if (CCDebug<ItemContainer>.IsEnabled)
            {
                this.PrintLog(DebugMessageBuilder.CreatePooled(indented: true)
                    .AddMessage("Item was put")
                    .AddProperty("Item", inputItem)
                    .AddProperty("ItemCount", toPutCount)
                    .AddPredicatedProperty(restCount != 0, "RestItemCount", restCount)
                    .AddPredicatedProperty(ID.HasValue, "ID", ID)
                    .AddPredicatedProperty(parentInventory is not null, "ParentInventory", parentInventory)
                    .ToStringAndDispose()
                    );
            }

            if (restCount <= 0)
                return CreateReadOnlyContainer();

            return CreateReadOnlyContainer(Item, restCount);
        }
        public TReadOnlyContainer PutItem(TInputItemContainerInfo? containerInfo)
        {
            if (containerInfo.IsNull())
                return CreateReadOnlyContainer();

            return PutItem(containerInfo.Item.CastTo<TItem>(), containerInfo.ItemCount);
        }
        public TReadOnlyContainer PutItem(TReadOnlyContainer readOnlyContainer)
        {
            return PutItem(readOnlyContainer.Item.CastTo<TItem>(), readOnlyContainer.ItemCount);
        }
        public TLargeReadOnlyContainer PutItem(TItem? item, long count)
        {
            if (count <= int.MaxValue)
                return ConvertReadOnlyContainerToLarge(PutItem(item, (int)count));

            if (!CanPutItem(item) || count <= 0)
                return CreateLargeReadOnlyContainer();

            TItem? previousItem = Item;
            this.item.Value = item;
            int putItemCount = Capacity - ItemCount;

            if (putItemCount <= 0)
            {
                this.item.Value = previousItem;
                return CreateLargeReadOnlyContainer();
            }

            long restItemCount = count - putItemCount + PutItem(item, putItemCount).ItemCount;
            return CreateLargeReadOnlyContainer(item, restItemCount);
        }
        public TLargeReadOnlyContainer PutItem(TLargeReadOnlyContainer largeReadOnlyContainer)
        {
            return PutItem(
                GetLargeReadOnlyContainerItem(largeReadOnlyContainer),
                GetLargeReadOnlyContainerItemCount(largeReadOnlyContainer)
                );
        }

        public TReadOnlyContainer PutItemFrom(TInputItemContainer? container, int count)
        {
            if (count <= 0 || container.IsNull() || Equals(container))
                return CreateReadOnlyContainer();

            ReadOnlyItemContainer containerItems = container.TakeItem(count);
            TReadOnlyContainer notFitItems = PutItem(containerItems.Item.CastTo<TItem>(), containerItems.ItemCount);
            ReadOnlyItemContainer restItems = container.PutItem(notFitItems);

            return CreateReadOnlyContainer(restItems.Item.CastTo<TItem>(), restItems.ItemCount);
        }
        public TReadOnlyContainer PutItemFrom(TInputItemContainer? container)
        {
            if (container.IsNull())
                return CreateReadOnlyContainer();

            return PutItemFrom(container, container.ItemCount);
        }

        public TReadOnlyContainer TakeItem(int count)
        {
            if (IsEmpty || count <= 0)
                return CreateReadOnlyContainer();

            int takenCount = Math.Min(count, ItemCount);
            itemCount.Value -= takenCount;

            if (onTakeItem is not null && takenCount >= 1)
                onTakeItem.Execute(CreateTakeItemEvent(Item!, takenCount));

            return CreateReadOnlyContainer(Item, takenCount);
        }
        public TReadOnlyContainer TakeItem() => TakeItem(itemCount.Value);
        public TReadOnlyContainer TakeItem(TItem? item, int count)
        {
            if (!ContainsItem(item))
                return CreateReadOnlyContainer();

            return TakeItem(count);
        }

        public TReadOnlyContainer ToReadOnly() => CreateReadOnlyContainer(Item, ItemCount);

        public abstract IItemContainer ShallowClone();

        public void CopyItemFrom(TInputItemContainer itemContainer)
        {
            item.Value = itemContainer.Item.CastTo<TItem>();
            itemCount.Value = itemContainer.ItemCount;
            capacity = itemContainer.Capacity;
        }

        public void Clear()
        {
            item.Value = null;
            itemCount.Value = 0;
        }

        public void SetParentInventory(IInventory? inventory)
        {
            if (EqualityComparer<IInventory?>.Default.Equals(parentInventory, inventory))
                return;

            if (parentInventory is not null)
            {
                if (ID.HasValue)
                    parentInventory.RemoveContainer(ID.Value);

                parentInventory = null;
                id = null;
            }

            if (inventory.IsNotNull())
            {
                parentInventory = inventory;

                IItemContainer untypedItemContainer = this.CastTo<IItemContainer>();

                if (!inventory.ContainsContainer(untypedItemContainer))
                    id = inventory.AddContainer(untypedItemContainer);
            }
        }

        public void Activate()
        {
            if (IsEmpty)
                return;

            isActive.Value = true;

#if CC_DEBUG_ENABLED
            if (CCDebug.Instance.IsEnabled)
                this.PrintLog($"Activated. ID: {ID}");
#endif
        }

        public void Deactivate()
        {
            isActive.Value = false;

#if CC_DEBUG_ENABLED
            if (CCDebug.Instance.IsEnabled)
                this.PrintLog($"Deactivated. ID: {ID}");
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

        public bool CanPutItem() => !IsFull;
        public bool CanPutItem(IItem? item) => !IsFull && ContainsItem(item);
        public bool CanPutItem(IItem? item, int count)
        {
            if (!CanPutItem(item))
                return false;

            count = Math.Clamp(count, min: 0, max: int.MaxValue);

            if (FreeSpace < count)
                return false;

            return true;
        }

        public override string ToString()
        {
            return ToStringBuilder.CreatePooled()
                .AddPredicatedProperty(ID.HasValue, nameof(ID), ID)
                .AddPredicatedProperty(ParentInventory is not null, nameof(ParentInventory), ParentInventory)
                .AddProperty(nameof(Item), Item)
                .AddProperty(nameof(ItemCount), ItemCount)
                .ToStringAndDispose();
        }

        public Observable<TItem?> ObserveItem() => item;

        public Observable<bool> ObserveIsActive() => isActive;

        public Observable<int> ObserveItemCount() => itemCount;

        public Observable<TPutItemEvent> ObservePutItem()
        {
            onPutItem ??= new ReactiveCommand<TPutItemEvent>();
            return onPutItem;
        }

        public Observable<TTakeItemEvent> ObserveTakeItem()
        {
            onTakeItem ??= new ReactiveCommand<TTakeItemEvent>();
            return onTakeItem;
        }

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
                onPutItem?.Dispose();
                onTakeItem?.Dispose();
            }
        }

        protected abstract TReadOnlyContainer CreateReadOnlyContainer();
        protected abstract TReadOnlyContainer CreateReadOnlyContainer(
            TItem? item, 
            int itemCount
            );

        protected abstract TLargeReadOnlyContainer CreateLargeReadOnlyContainer();
        protected abstract TLargeReadOnlyContainer CreateLargeReadOnlyContainer(
            TItem? item,
            long itemCount
            );

        protected abstract TLargeReadOnlyContainer ConvertReadOnlyContainerToLarge(TReadOnlyContainer readOnlyContainer);

        protected abstract TPutItemEvent CreatePutItemEvent(TItem item, int itemCount);

        protected abstract TTakeItemEvent CreateTakeItemEvent(TItem item, int itemCount);

        protected abstract TItem? GetLargeReadOnlyContainerItem(TLargeReadOnlyContainer largeReadOnlyContainer);

        protected abstract long GetLargeReadOnlyContainerItemCount(TLargeReadOnlyContainer largeReadOnlyContainer);
    }
}
