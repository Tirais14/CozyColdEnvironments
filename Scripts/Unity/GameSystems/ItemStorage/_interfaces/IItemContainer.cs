using CCEnvs.Reflection;
using System;

#nullable enable
namespace CCEnvs.GameSystems.Storages
{
    public interface IItemContainer : IItemContainerInfo
    {
        IItemContainer AddItem(IStorageItem item, int count);

        void AddItemFrom(IItemContainer itemContainer, int count);

        void AddItemFrom(IItemContainer itemContainer);

        void CopyItemFrom(IItemContainer itemContainer);

        IItemContainer TakeItem(int count);

        IItemContainer TakeItemAll();

        void Clear();
    }
    public interface IItemContainer<TContainer> : IItemContainer
        where TContainer : IItemContainer
    {
        new TContainer AddItem(IStorageItem item, int count);

        void AddItemFrom(TContainer itemContainer, int count);

        void AddItemFrom(TContainer itemContainer);

        void CopyItemFrom(TContainer itemContainer);

        new TContainer TakeItem(int count);

        new TContainer TakeItemAll();

        IItemContainer IItemContainer.AddItem(IStorageItem item, int count)
        {
            if (!CanHold(item))
                throw new ArgumentException($"Invalid item type = {item.GetTypeName()}");

            return AddItem(item, count);
        }

        /// <exception cref="ArgumentException"></exception>
        void IItemContainer.AddItemFrom(IItemContainer itemContainer, int count)
        {
            if (itemContainer is not TContainer typed)
                throw new ArgumentException($"Expected {typeof(TContainer).GetName()}");

            AddItemFrom(typed, count);
        }

        /// <exception cref="ArgumentException"></exception>
        void IItemContainer.CopyItemFrom(IItemContainer itemContainer)
        {
            if (itemContainer is not TContainer typed)
                throw new ArgumentException($"Expected {typeof(TContainer).GetName()}");

            CopyItemFrom(typed);
        }

        /// <exception cref="ArgumentException"></exception>
        void IItemContainer.AddItemFrom(IItemContainer itemContainer)
        {
            if (itemContainer is not TContainer typed)
                throw new ArgumentException($"Expected {typeof(TContainer).GetName()}");

            AddItemFrom(typed);
        }

        IItemContainer IItemContainer.TakeItem(int count) => TakeItem(count);

        IItemContainer IItemContainer.TakeItemAll() => TakeItemAll();
    }
    public interface IItemContainer<TContainer, TItem> :
        IItemContainer<TContainer>,
        IItemContainerInfo<TItem>
        where TContainer : IItemContainer
        where TItem : IStorageItem
    {
        TContainer AddItem(TItem item, int count);

        TContainer IItemContainer<TContainer>.AddItem(IStorageItem item, int count)
        {
            if (item is not TItem typedItem)
                throw new ArgumentException($"Expected {typeof(TItem).GetName()}");

            return AddItem(typedItem, count);
        }
    }
}
