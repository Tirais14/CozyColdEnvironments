using CCEnvs.TypeMatching;
using R3;

#nullable enable
#pragma warning disable IDE1006
namespace CCEnvs.UnityX.Items
{
    public interface IItemContainerInfoItemless
    {
        int ItemCount { get; }
        int FreeSpace { get; }

        bool IsEmpty { get; }
        bool IsFull { get; }

        bool ContainsItem();
        bool ContainsItem(IItem? item);
        bool ContainsItem(IItem? item, int count);

        bool CanPutItem();
        bool CanPutItem(IItem? item);
        bool CanPutItem(IItem? item, int count);

        Observable<int> ObserveItemCount();
    }

    public interface IItemContainerInfoItemless<TItem> : IItemContainerInfoItemless
        where TItem : IItem
    {
        bool ContainsItem(TItem? item);
        bool ContainsItem(TItem? item, int count);

        bool CanPutItem(TItem? item);
        bool CanPutItem(TItem? item, int count);

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

        bool IItemContainerInfoItemless.CanPutItem(IItem? item)
        {
            if (item.IsNot<TItem>(out var typed))
                return false;

            return CanPutItem(typed);
        }

        bool IItemContainerInfoItemless.CanPutItem(IItem? item, int count)
        {
            if (item.IsNot<TItem>(out var typed))
                return false;

            return CanPutItem(typed, count);
        }
    }
}
