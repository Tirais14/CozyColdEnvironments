using CCEnvs.FuncLanguage;
using System;
using UniRx;

#nullable enable
#pragma warning disable IDE1006
namespace CCEnvs.Unity.Storages
{
    public interface IItemContainerInfoItemless
    {
        int ItemCount { get; }
        Maybe<IInventory> ParentInventory { get; set; }
        bool IsActiveContainer { get; }
        int Capacity { get; set; }
        bool IsEmpty { get; }
        bool IsFull { get; }

        bool ContainsItem();
        bool ContainsItem(IItem? item);
        bool ContainsItem(IItem? item, int count);

        MaybeStruct<int> GetContainerID();

        void ActivateContainer();

        void DeactivateContainer();

        bool SwitchContainerActiveState();

        IObservable<bool> ObserveIsActiveContainer();

        IObservable<int> ObserveItemCount();
    }
}
