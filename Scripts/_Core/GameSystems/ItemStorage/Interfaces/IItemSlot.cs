#nullable enable
using System;

namespace UTIRLib.GameSystems.ItemStorageSystem
{
    public interface IItemSlot
    {
        IItemStack ItemStack { get; }
        int CapacityLimit { get; set; }
        bool HasCapacityLimit { get; }
        bool IsEmpty { get; }
        bool IsFull { get; }
        int ItemCount { get; }

        IItemStack AddItem(IStorageItem item, int count);

        void AddItemFrom(IItemStack itemStack, int count);
        void AddItemFrom(IItemStack itemStack);

        IItemStack TakeItem(int count);

        IItemStack TakeItemAll();

        bool IsSameItem(IStorageItem item);
    }
    public interface IItemSlot<T> : IItemSlot
        where T : IItemStack
    {
        new T ItemStack { get; }

        IItemStack IItemSlot.ItemStack => ItemStack;

        new T AddItem(IStorageItem item, int count);

        void AddItemFrom(T itemStack, int count);
        void AddItemFrom(T itemStack);

        new T TakeItem(int count);

        new T TakeItemAll();

        IItemStack IItemSlot.AddItem(IStorageItem item, int count)
        {
            return AddItem(item, count);
        }

        void IItemSlot.AddItemFrom(IItemStack itemStack, int count)
        {
            AddItemFrom(itemStack, count);
        }
        void IItemSlot.AddItemFrom(IItemStack itemStack)
        {
            AddItemFrom(itemStack, itemStack.ItemCount);
        }

        IItemStack IItemSlot.TakeItem(int count)
        {
            return TakeItem(count);
        }

        IItemStack IItemSlot.TakeItemAll()
        {
            return TakeItemAll();
        }
    }
}
