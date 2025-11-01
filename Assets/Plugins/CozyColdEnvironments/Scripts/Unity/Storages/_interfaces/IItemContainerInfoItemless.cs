using CCEnvs.FuncLanguage;
using UniRx;

#nullable enable
#pragma warning disable IDE1006
namespace CCEnvs.Unity.Storages
{
    public interface IItemContainerInfoItemless
    {
        IReadOnlyReactiveProperty<int> ItemCount { get; }
        int Capacity { get; set; }
        bool IsEmpty { get; }
        bool IsFull { get; }
        Maybe<IInventory> ParentInventory { get; set; }

        bool Contains();
        bool Contains(IItem? item);
        bool Contains(IItem? item, int count);
    }
}
