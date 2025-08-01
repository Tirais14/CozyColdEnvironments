#nullable enable
using UnityEngine;
using UTIRLib.Diagnostics;
using UTIRLib.Reflection;

namespace UTIRLib.GameSystems.ItemStorageSystem
{
    public interface IItemStack
    {
        IStorageItem Item { get; }
        int ItemCount { get; }
        int MaxItemCount { get; }
        bool IsEmpty { get; }
        bool IsFull { get; }

        IItemStack AddItem(IStorageItem item, int count);

        void AddItem(IItemStack itemStack, int count);

        IItemStack Take(int count);

        IItemStack TakeAll();

        void Clear();
    }

    public interface IItemStack<T> : IItemStack
        where T : IStorageItem, new()
    {
        new T Item { get; }

        IStorageItem IItemStack.Item => Item;

        IItemStack<T> AddItem(T item, int count);

        void AddItem(IItemStack<T> itemStack, int count);

        new IItemStack<T> Take(int count);

        new IItemStack<T> TakeAll();

        IItemStack IItemStack.AddItem(IStorageItem item, int count)
        {
            if (item is not T typed)
                throw new System.InvalidOperationException($"Cannot add item {item?.GetType().GetName()}.");

            return AddItem(typed, count);
        }

        void IItemStack.AddItem(IItemStack itemStack, int count)
        {
            if (itemStack.IsNull())
                throw new System.ArgumentNullException(nameof(itemStack));
            if (itemStack.IsEmpty)
            {
                Debug.LogWarning("Try to move items from empty item stack. This is incorrect behavior.");
                return;
            }
            if (itemStack.Item is not T typedItem)
                throw new System.InvalidOperationException($"Cannot add item from {itemStack?.GetType().GetName()}.");

            var temp = new ItemStack<T>(typedItem, itemStack.ItemCount);

            AddItem(temp, count);

            if (!temp.IsEmpty)
                itemStack.AddItem(temp.Item!, temp.ItemCount);
        }

        IItemStack IItemStack.Take(int count)
        {
            return Take(count);
        }

        IItemStack IItemStack.TakeAll()
        {
            return TakeAll();
        }
    }
}
