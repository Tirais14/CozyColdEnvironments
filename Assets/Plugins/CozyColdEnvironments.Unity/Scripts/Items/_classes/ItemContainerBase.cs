using CCEnvs.Diagnostics;
using CCEnvs.FuncLanguage;
using R3;
using System;
using System.Collections.Generic;
using System.Threading;

#nullable enable
namespace CCEnvs.UnityX.Items
{
    public abstract class ItemContainerBase<TItem, TInputItemContainer, TInputItemContainerInfo, TReadOnlyItemContainer> : IDisposable

        where TItem : class, IItem
        where TInputItemContainer : IItemContainer
        where TInputItemContainerInfo : IItemContainerInfo
        where TReadOnlyItemContainer : struct, IItemContainerInfo
    {
        private readonly ReactiveProperty<TItem?> item = new();
        private readonly ReactiveProperty<int> itemCount = new();
        private readonly ReactiveProperty<bool> isActive = new();

        private IInventory? parentInventory;

        private int capacity = 128;
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

        public TReadOnlyItemContainer PutItem(TItem? inputItem, int count = 1)
        {
            if (count <= 0 || inputItem.IsNull())
                return CreateReadOnlyItemContainer();

            if (IsFull || (Item.IsNotNull() && !EqualityComparer<IItem?>.Default.Equals(Item, inputItem)))
                return CreateReadOnlyItemContainer(inputItem, count);

            item.Value = inputItem;
            int toPutCount = Math.Clamp(count, 0, FreeSpace);
            itemCount.Value += toPutCount;

            int restCount = count - toPutCount;

            if (CCDebug<ItemContainer>.IsEnabled)
            {
                this.PrintLog(DebugMessageBuilder.CreatePooled()
                    .AddMessage("Item was put")
                    .AddPredicatedProperty(ID.HasValue, "ID", ID)
                    .AddPredicatedProperty(parentInventory is not null, "ParentInventory", parentInventory)
                    .AddProperty("Item", inputItem)
                    .AddProperty("ItemCount", toPutCount)
                    .AddPredicatedProperty(restCount != 0, "RestItemCount", restCount)
                    .ToStringAndDispose()
                    );
            }

            if (restCount <= 0)
                return CreateReadOnlyItemContainer();

            return CreateReadOnlyItemContainer(Item, restCount);
        }
        public TReadOnlyItemContainer PutItem(TInputItemContainerInfo? containerInfo)
        {
            if (containerInfo.IsNull())
                return CreateReadOnlyItemContainer();

            return PutItem(containerInfo.Item.CastTo<TItem>(), containerInfo.ItemCount);
        }
        public TReadOnlyItemContainer PutItem(TReadOnlyItemContainer readOnlyItemContainer)
        {
            return PutItem(readOnlyItemContainer.Item.CastTo<TItem>(), readOnlyItemContainer.ItemCount);
        }

        public TReadOnlyItemContainer PutItemFrom(TInputItemContainer? container, int count)
        {
            if (count <= 0 || container.IsNull() || Equals(container))
                return CreateReadOnlyItemContainer();

            ReadOnlyItemContainer containerItems = container.TakeItem(count);
            TReadOnlyItemContainer notFitItems = PutItem(containerItems.Item.CastTo<TItem>(), containerItems.ItemCount);
            ReadOnlyItemContainer restItems = container.PutItem(notFitItems);

            return CreateReadOnlyItemContainer(restItems);
        }
        public TReadOnlyItemContainer PutItemFrom(TInputItemContainer? container)
        {
            if (container.IsNull())
                return CreateReadOnlyItemContainer();

            return PutItemFrom(container, container.ItemCount);
        }

        public TReadOnlyItemContainer TakeItem(int count)
        {
            if (IsEmpty || count <= 0)
                return CreateReadOnlyItemContainer();

            int takenCount = Math.Min(count, ItemCount);
            itemCount.Value -= takenCount;

            return CreateReadOnlyItemContainer(Item, takenCount);
        }
        public TReadOnlyItemContainer TakeItem() => TakeItem(itemCount.Value);
        public TReadOnlyItemContainer TakeItem(TItem? item, int count)
        {
            if (!ContainsItem(item))
                return CreateReadOnlyItemContainer();

            return TakeItem(count);
        }

        public TReadOnlyItemContainer ToReadOnly() => CreateReadOnlyItemContainer(Item, ItemCount);

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

        public bool CanPut() => !IsFull;
        public bool CanPut(IItem? item) => !IsFull && ContainsItem(item);
        public bool CanPut(IItem? item, int count)
        {
            if (!CanPut(item))
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

        private int disposed;
        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        protected abstract TReadOnlyItemContainer CreateReadOnlyItemContainer();
        protected abstract TReadOnlyItemContainer CreateReadOnlyItemContainer(
            TItem? item, 
            int itemCount
            );
        protected  TReadOnlyItemContainer CreateReadOnlyItemContainer<TInputReadOnlyItemContainer>(
            TInputReadOnlyItemContainer container
            )
            where TInputReadOnlyItemContainer : struct, IItemContainerInfo
        {
            return CreateReadOnlyItemContainer(container.Item.CastTo<TItem>(), container.ItemCount);
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
}
