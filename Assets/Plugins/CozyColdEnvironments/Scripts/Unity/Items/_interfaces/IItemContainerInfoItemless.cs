using CCEnvs.FuncLanguage;
using System;
using UniRx;

#nullable enable
#pragma warning disable IDE1006
namespace CCEnvs.Unity.Items
{
    public interface IItemContainerInfoItemless
    {
        int ItemCount { get; }
        Maybe<IInventory> ParentInventory { get; set; }
        int Capacity { get; set; }
        bool IsEmpty { get; }
        bool IsFull { get; }

        bool ContainsItem();
        bool ContainsItem(IItem? item);
        bool ContainsItem(IItem? item, int count);

        Maybe<int> GetContainerID();

        IObservable<Pair<int>> ObserveItemCount();

        IObservable<Pair<int>> ObserveDecreasedItemCount();

        IObservable<Pair<int>> ObserveIncreaseItemCount();
    }
}
