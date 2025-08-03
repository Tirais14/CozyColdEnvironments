using UniRx;
using UTIRLib.GameSystems;
using UTIRLib.GameSystems.ItemStorageSystem;

#nullable enable
namespace UTIRLib.UI.ItemStorageSystem
{
    public class ItemStackReactive : IItemStackReactive
    {
        private readonly ItemStack stack;
        private readonly ReactiveProperty<IStorageItem> itemReactive = new(new StorageItem());
        private readonly ReactiveProperty<int> itemCountReactive = new();

        public IStorageItem Item => stack.Item;
        public int ItemCount => stack.ItemCount;
        public int MaxItemCount => stack.MaxItemCount;
        public bool IsEmpty => stack.IsEmpty;
        public bool IsFull => stack.IsFull;

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

        public IItemStack AddItem(IStorageItem item, int count)
        {
            IItemStack temp = stack.AddItem(item, count);
            UpdateInfo();

            return temp;
        }

        public void AddItemFrom(IItemStack itemStack, int count)
        {
            stack.AddItemFrom(itemStack, count);
            UpdateInfo();
        }
        public void AddItemFrom(IItemStack itemStack)
        {
            stack.AddItemFrom(itemStack);
            UpdateInfo();
        }

        public IItemStack TakeItem(int count)
        {
            IItemStack temp = stack.TakeItem(count);
            UpdateInfo();

            return temp;
        }

        public IItemStack TakeItemAll()
        {
            IItemStack temp = stack.TakeItemAll();
            UpdateInfo();

            return temp;
        }

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
    public class ItemStackReactive<T> : IItemStackReactive<T>
        where T : IStorageItem, new()
    {
        private readonly ItemStack<T> stack;
        private readonly ReactiveProperty<IStorageItem> itemReactive = new(new T());
        private readonly ReactiveProperty<int> itemCountReactive = new();

        public T Item => stack.Item;
        public int ItemCount => stack.ItemCount;
        public int MaxItemCount => stack.MaxItemCount;
        public bool IsEmpty => stack.IsEmpty;
        public bool IsFull => stack.IsFull;

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

        public void Clear() => stack.Clear();

        private void UpdateInfo()
        {
            itemReactive.Value = stack.Item;
            itemCountReactive.Value = stack.ItemCount;
        }
    }
}
