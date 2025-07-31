#nullable enable
using UTIRLib.Reflection;

namespace UTIRLib.GameSystems.Storage
{
    public interface IItemStack
    {
        IItem Item { get; }
        int ItemCount { get; }
        int MaxItemCount { get; }
        bool IsEmpty { get; }
        bool IsFull { get; }

        IItemStack AddItem(IItem item, int count);

        void AddItem(IItemStack itemStack, int count);

        IItemStack Take(int count);

        IItemStack TakeAll();

        void Clear();
    }

    public interface IItemStack<T> : IItemStack
        where T : IItem
    {
        new T Item { get; }

        IItem IItemStack.Item => Item;

        IItemStack<T> AddItem(T item, int count);

        void AddItem(IItemStack<T> itemStack, int count);

        new IItemStack<T> Take(int count);

        new IItemStack<T> TakeAll();

        IItemStack IItemStack.AddItem(IItem item, int count)
        {
            if (item is not T typed)
                throw new System.InvalidOperationException($"Cannot add item {item?.GetType().GetName()}.");

            return AddItem(typed, count);
        }

        void IItemStack.AddItem(IItemStack itemStack, int count)
        {
            if (itemStack is not IItemStack<T> typed)
                throw new System.InvalidOperationException($"Cannot add item from {itemStack?.GetType().GetName()}.");

            AddItem(typed, count);
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
