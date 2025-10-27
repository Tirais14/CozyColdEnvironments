using CCEnvs.Language;
using UniRx;

#nullable enable
#pragma warning disable IDE1006
namespace CCEnvs.Unity.GameSystems.Storages
{
    public interface IItemContainerInfoItemless 
    {
        IReadOnlyReactiveProperty<int> ItemCount { get; }
        int Capacity { get; set; }
        bool IsEmpty { get; }
        bool IsFull { get; }
        Maybe<IItemContainer> ParentContainer { get; }

        bool Contains();
        bool Contains(IItem? item);
        bool Contains(IItem? item, int count);
    }
}
