using CCEnvs.FuncLanguage;
using CCEnvs.TypeMatching;
using R3;

#nullable enable
#pragma warning disable IDE1006
namespace CCEnvs.UnityX.Items
{
    public interface IItemContainerInfoItemless : IIDMarked<int?>
    {
        int ItemCount { get; }
        int Capacity { get; set; }
        int FreeSpace { get; }

        int? ID { get; set; }

        IInventory? ParentInventory { get; }

        bool IsEmpty { get; }
        bool IsFull { get; }

        int? IIDMarked<int?>.ID => ID;

        bool ContainsItem();
        bool ContainsItem(IItem? item);
        bool ContainsItem(IItem? item, int count);

        bool CanPut();
        bool CanPut(IItem? item);
        bool CanPut(IItem? item, int count);

        void SetParentInventory(IInventory? inventory);

        Observable<int> ObserveItemCount();
    }

    public interface IItemContainerInfoItemless<TItem> : IItemContainerInfoItemless
        where TItem : IItem
    {
        bool ContainsItem(TItem? item);
        bool ContainsItem(TItem? item, int count);

        bool CanPut(TItem? item);
        bool CanPut(TItem? item, int count);

        bool IItemContainerInfoItemless.ContainsItem(IItem? item)
        {
            if (item.IsNot<TItem>(out var typed))
                return false;

            return ContainsItem(typed);
        }

        bool IItemContainerInfoItemless.ContainsItem(IItem? item, int count)
        {
            if (item.IsNot<TItem>(out var typed))
                return false;

            return ContainsItem(typed, count);
        }

        bool IItemContainerInfoItemless.CanPut(IItem? item)
        {
            if (item.IsNot<TItem>(out var typed))
                return false;

            return CanPut(typed);
        }

        bool IItemContainerInfoItemless.CanPut(IItem? item, int count)
        {
            if (item.IsNot<TItem>(out var typed))
                return false;

            return CanPut(typed, count);
        }
    }

    public interface IItemContainerInfoItemless<TItem, TInventory> : IItemContainerInfoItemless<TItem>
        where TItem : IItem
        where TInventory : IInventory
    {
        new TInventory? ParentInventory { get; set; }

        IInventory? IItemContainerInfoItemless.ParentInventory {
            get => ParentInventory;
        }
    }
}
