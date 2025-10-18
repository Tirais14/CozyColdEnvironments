using UniRx;
using UnityEngine;

#nullable enable
namespace CCEnvs.Unity.GameSystems.Storages
{
    public interface IItemContainerInfoItemless
    {
        IReadOnlyReactiveProperty<int> ItemCount { get; }
        int Capacity { get; }
        bool IsEmpty { get; }
        bool IsFull { get; }

        bool Contains();
        bool Contains(IItem? item);
        bool Contains(IItem? item, int count);
    }
}
