using UniRx;

#nullable enable
namespace CCEnvs.Unity.GameSystems.Storages
{
    public interface IItemContainerInfo
    {
        IReadOnlyReactiveProperty<IItem?> Item { get; }
        IReadOnlyReactiveProperty<int> ItemCount { get; }
        int Capacity { get; }
        bool IsEmpty { get; }

        bool Contains();
        bool Contains(IItem? item);
        bool Contains(IItem? item, int count);
    }
}
