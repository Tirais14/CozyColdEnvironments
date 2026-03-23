using CCEnvs.FuncLanguage;
using R3;

#nullable enable
#pragma warning disable IDE1006
namespace CCEnvs.Unity.Items
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
}
