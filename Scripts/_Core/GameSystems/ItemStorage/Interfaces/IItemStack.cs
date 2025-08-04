#nullable enable
using UTIRLib.Diagnostics;
using UTIRLib.Reflection;

namespace UTIRLib.GameSystems.ItemStorageSystem
{
    public interface IItemStack : IItemContainerInfo
    {
        IItemStack AddItem(IStorageItem item, int count);

        void AddItemFrom(IItemStack itemStack, int count);

        void AddItemFrom(IItemStack itemStack);

        IItemStack TakeItem(int count);

        IItemStack TakeItemAll();

        void Clear();
    }

    public interface IItemStack<T> : IItemStack, IItemContainerInfo<T>
        where T : IStorageItem, new()
    {

        IItemStack<T> AddItem(T item, int count);

        void AddItemFrom(IItemStack<T> itemStack, int count);

        void AddItemFrom(IItemStack<T> itemStack);

        new IItemStack<T> TakeItem(int count);

        new IItemStack<T> TakeItemAll();

        IItemStack IItemStack.AddItem(IStorageItem item, int count)
        {
            if (item is not T typed)
                throw new System.InvalidOperationException($"Cannot add item {item?.GetType().GetName()}.");

            return AddItem(typed, count);
        }

        void IItemStack.AddItemFrom(IItemStack itemStack, int count)
        {
            if (itemStack.IsNull())
                throw new System.ArgumentNullException(nameof(itemStack));
            if (!itemStack.HasItem)
                throw new System.Exception("Nothing to add from item stack.");
            if (itemStack.Item is not T typedItem)
                throw new System.InvalidOperationException($"Cannot add item from {itemStack?.GetType().GetName()}.");

            var temp = new ItemStack<T>(typedItem, itemStack.ItemCount);

            AddItemFrom(temp, count);

            if (temp.HasItem)
                itemStack.AddItem(temp.Item!, temp.ItemCount);
        }

        void IItemStack.AddItemFrom(IItemStack itemStack)
        {
            AddItemFrom(itemStack, itemStack.ItemCount);
        }

        IItemStack IItemStack.TakeItem(int count)
        {
            return TakeItem(count);
        }

        IItemStack IItemStack.TakeItemAll()
        {
            return TakeItemAll();
        }
    }
}
