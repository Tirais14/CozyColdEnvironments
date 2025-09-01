using CCEnvs.Unity.GameSystems.Storages;
using UniRx;

#nullable enable
namespace CCEnvs.Unity.UI.Storages
{
    public class ItemStackReactive
        :
        IItemStack,
        IItemContainerReactive
    {
        private readonly ItemStack stack;
        private readonly ReactiveProperty<IStorageItem> itemReactive = new(new StorageItem());
        private readonly ReactiveProperty<int> itemCountReactive = new();

        public IStorageItem Item => stack.Item;
        public int ItemCount => stack.ItemCount;
        public int MaxItemCount => stack.MaxItemCount;
        public bool HasItem => stack.HasItem;
        public bool IsContainerFull => stack.IsContainerFull;

        public IReadOnlyReactiveProperty<IStorageItem> ItemReactive => itemReactive;
        public IReadOnlyReactiveProperty<int> ItemCountReactive => itemCountReactive;

        public ItemStackReactive(int maxItemCount = int.MaxValue)
        {
            stack = new ItemStack(maxItemCount);
        }

        public ItemStackReactive(IStorageItem item,
                                 int itemCount = 1,
                                 int maxItemCount = int.MaxValue)
        {
            stack = new ItemStack(item, itemCount, maxItemCount);
        }

        public ItemStackReactive(ItemStackReactive stack)
            :
            this(stack.Item, stack.ItemCount, stack.MaxItemCount)
        {
        }

        public IItemContainer AddItem(IStorageItem item, int count)
        {
            IItemContainer temp = stack.AddItem(item, count);
            UpdateInfo();

            return temp;
        }

        public void AddItemFrom(IItemContainer itemContainer, int count)
        {
            stack.AddItemFrom(itemContainer, count);
            UpdateInfo();
        }
        public void AddItemFrom(IItemContainer itemContainer)
        {
            stack.AddItemFrom(itemContainer);
            UpdateInfo();
        }

        public void CopyItemFrom(IItemContainer itemContainer)
        {
            stack.CopyItemFrom(itemContainer);
            UpdateInfo();
        }

        public IItemContainer TakeItem(int count)
        {
            IItemContainer temp = stack.TakeItem(count);
            UpdateInfo();

            return temp;
        }

        public IItemContainer TakeItemAll()
        {
            IItemContainer temp = stack.TakeItemAll();
            UpdateInfo();

            return temp;
        }

        public bool CanHold(IStorageItem item) => stack.CanHold(item);

        public bool Contains(IStorageItem item) => stack.Contains(item);

        public bool IsSameItem(IStorageItem item) => stack.IsSameItem(item);

        public void Clear()
        {
            stack.Clear();
            UpdateInfo();
        }

        private void UpdateInfo()
        {
            itemReactive.Value = stack.Item;
            itemCountReactive.Value = stack.ItemCount;
        }
    }
    public class ItemStackReactive<T>
        :
        IItemStack<T>,
        IItemContainerReactive<IItemStack<T>, T>

        where T : IStorageItem, new()
    {
        private readonly ItemStack<T> stack;
        private readonly ReactiveProperty<IStorageItem> itemReactive = new(new T());
        private readonly ReactiveProperty<int> itemCountReactive = new();

        public T Item => stack.Item;
        public int ItemCount => stack.ItemCount;
        public int MaxItemCount => stack.MaxItemCount;
        public bool HasItem => stack.HasItem;
        public bool IsContainerFull => stack.IsContainerFull;

        public IReadOnlyReactiveProperty<IStorageItem> ItemReactive => itemReactive;
        public IReadOnlyReactiveProperty<int> ItemCountReactive => itemCountReactive;

        public ItemStackReactive(int maxItemCount = int.MaxValue)
        {
            stack = new ItemStack<T>(maxItemCount);
        }

        public ItemStackReactive(T item,
                                 int itemCount = 1,
                                 int maxItemCount = int.MaxValue)
        {
            stack = new ItemStack<T>(item, itemCount, maxItemCount);
        }

        public ItemStackReactive(ItemStackReactive<T> stack)
            :
            this(stack.Item, stack.ItemCount, stack.MaxItemCount)
        {
        }

        public IItemStack<T> AddItem(T item, int count)
        {
            IItemStack<T> rest = stack.AddItem(item, count);

            UpdateInfo();

            return rest;
        }

        public void AddItemFrom(IItemStack<T> itemStack, int count)
        {
            stack.AddItemFrom(itemStack, count);

            UpdateInfo();
        }
        public void AddItemFrom(IItemStack<T> itemStack)
        {
            stack.AddItemFrom(itemStack);

            UpdateInfo();
        }

        public void CopyItemFrom(IItemStack<T> itemStack)
        {
            stack.CopyItemFrom(itemStack);

            UpdateInfo();
        }

        public IItemStack<T> TakeItem(int count)
        {
            IItemStack<T> temp = stack.TakeItem(count);
            UpdateInfo();

            return temp;
        }

        public IItemStack<T> TakeItemAll()
        {
            IItemStack<T> temp = stack.TakeItemAll();
            UpdateInfo();

            return temp;
        }

        public bool CanHold(T item) => stack.CanHold(item);

        public bool Contains(T item) => stack.Contains(item);

        public bool IsSameItem(T item) => stack.IsSameItem(item);

        public void Clear()
        {
            stack.Clear();
            UpdateInfo();
        }

        private void UpdateInfo()
        {
            itemReactive.Value = stack.Item;
            itemCountReactive.Value = stack.ItemCount;
        }
    }
}
