using UniRx;
using UTIRLib.GameSystems;
using UTIRLib.GameSystems.ItemStorageSystem;
using UTIRLib.UI.ItemStorageSystem;

#nullable enable
namespace UTIRLib.UI
{
    public class ItemSlotReactive 
        :
        IItemSlot,
        IItemContainerReactive
    {
        private readonly ItemSlot slot;
        private readonly ReactiveProperty<IStorageItem> itemReactive = new();
        private readonly ReactiveProperty<int> itemCountReactive = new();

        public ItemSlotReactive(IItemStack itemStack)
        {
            slot = new ItemSlot(itemStack);
        }

        public int CapacityLimit { 
            get => slot.CapacityLimit;
            set => slot.CapacityLimit = value; 
        }
        public bool HasCapacityLimit => slot.HasCapacityLimit;
        public IStorageItem Item => slot.Item;
        public int ItemCount => slot.ItemCount;
        public int MaxItemCount => slot.MaxItemCount;
        public bool HasItem => slot.HasItem;
        public bool IsContainerFull => slot.IsContainerFull;
        public IReadOnlyReactiveProperty<IStorageItem> ItemReactive => itemReactive;
        public IReadOnlyReactiveProperty<int> ItemCountReactive => itemCountReactive;

        public IItemContainer AddItem(IStorageItem item, int count)
        {
            IItemContainer temp = slot.AddItem(item, count);
            Update();

            return temp;
        }

        public void AddItemFrom(IItemContainer itemContainer, int count)
        {
            slot.AddItemFrom(itemContainer, count);
            Update();
        }

        public void AddItemFrom(IItemContainer itemContainer)
        {
            slot.AddItemFrom(itemContainer);
            Update();
        }

        public bool CanHold(IStorageItem item)
        {
            return slot.CanHold(item);
        }

        public void Clear()
        {
            slot.Clear();
            Update();
        }

        public bool Contains(IItemStack itemStack)
        {
            return slot.Contains(itemStack);
        }

        public bool Contains(IStorageItem item)
        {
            return slot.Contains(item);
        }

        public void CopyItemFrom(IItemContainer itemContainer)
        {
            slot.CopyItemFrom(itemContainer);
            Update();
        }

        public bool IsSameItem(IStorageItem item)
        {
            return slot.IsSameItem(item);
        }

        public IItemContainer TakeItem(int count)
        {
            IItemContainer temp = slot.TakeItem(count);
            Update();

            return temp;
        }

        public IItemContainer TakeItemAll()
        {
            IItemContainer temp = slot.TakeItemAll();
            Update();

            return temp;
        }

        private void Update()
        {
            itemReactive.Value = slot.Item;
            itemCountReactive.Value = slot.ItemCount;
        }
    }
    public class ItemSlotReactive<T> : IItemSlot<T>, IItemContainerReactive<T>
        where T : IItemStack
    {
        private readonly ItemSlot<T> slot;
        private readonly ReactiveProperty<IStorageItem> itemReactive = new();
        private readonly ReactiveProperty<int> itemCountReactive = new();

        public ItemSlotReactive(T itemStack)
        {
            slot = new ItemSlot<T>(itemStack);
        }

        public int CapacityLimit {
            get => slot.CapacityLimit;
            set => slot.CapacityLimit = value;
        }
        public bool HasCapacityLimit => slot.HasCapacityLimit;
        public IStorageItem Item => slot.Item;
        public int ItemCount => slot.ItemCount;
        public int MaxItemCount => slot.MaxItemCount;
        public bool HasItem => slot.HasItem;
        public bool IsContainerFull => slot.IsContainerFull;
        public IReadOnlyReactiveProperty<IStorageItem> ItemReactive => itemReactive;
        public IReadOnlyReactiveProperty<int> ItemCountReactive => itemCountReactive;

        public T AddItem(IStorageItem item, int count)
        {
            T temp = slot.AddItem(item, count);
            Update();

            return temp;
        }

        public void AddItemFrom(T itemContainer, int count)
        {
            slot.AddItemFrom(itemContainer, count);
            Update();
        }

        public void AddItemFrom(T itemContainer)
        {
            slot.AddItemFrom(itemContainer);
            Update();
        }

        public bool CanHold(IStorageItem item)
        {
            return slot.CanHold(item);
        }

        public void Clear()
        {
            slot.Clear();
            Update();
        }

        public bool Contains(T itemStack)
        {
            return slot.Contains(itemStack);
        }

        public bool Contains(IStorageItem item)
        {
            return slot.Contains(item);
        }

        public void CopyItemFrom(T itemContainer)
        {
            slot.CopyItemFrom(itemContainer);
            Update();
        }

        public bool IsSameItem(IStorageItem item)
        {
            return slot.IsSameItem(item);
        }

        public T TakeItem(int count)
        {
            T temp = slot.TakeItem(count);
            Update();

            return temp;
        }

        public T TakeItemAll()
        {
            T temp = slot.TakeItemAll();
            Update();

            return temp;
        }

        private void Update()
        {
            itemReactive.Value = slot.Item;
            itemCountReactive.Value = slot.ItemCount;
        }
    }
}
