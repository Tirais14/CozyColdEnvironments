using CCEnvs.FuncLanguage;
using CCEnvs.TypeMatching;
using R3;

#nullable enable
#pragma warning disable IDE1006
namespace CCEnvs.UnityX.Items
{
    public interface IItemContainerInfoItemless
    {
        int ItemCount { get; }
        int Capacity { get; set; }
        int FreeSpace { get; }

        Maybe<IInventory> ParentInventory { get; set; }

        bool IsEmpty { get; }
        bool IsFull { get; }

        bool ContainsItem();
        bool ContainsItem(IItem? item);
        bool ContainsItem(IItem? item, int count);

        bool CanPut();
        bool CanPut(IItem? item);
        bool CanPut(IItem? item, int count);

        Maybe<int> GetContainerID();

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
        new Maybe<TInventory> ParentInventory { get; set; }

        Maybe<IInventory> IItemContainerInfoItemless.ParentInventory {
            get => ParentInventory.Cast<IInventory>();
            set => ParentInventory = value.Cast<TInventory>();
        }
    }
}
