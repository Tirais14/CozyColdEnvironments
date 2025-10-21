using CCEnvs.Unity.GameSystems.Storages;
using System;
using System.Collections.Generic;
using UniRx;

#nullable enable
namespace CCEnvs.Unity
{
    public interface IInventory
        : IItemAccessor,
        IItemContainerInfoItemless
    {
        event Action<(int id, IItemContainer value)> OnAdd;
        event Action<(int id, IItemContainer value)> OnRemove;

        IEnumerable<int> IDs { get; }
        IEnumerable<IItemContainer> Containers { get; }

        void Add(IItemContainer itemContainer);

        bool Remove(int id);
        bool Remove(IItemContainer itemContainer);

        bool Contains(int id);
        bool Contains(IItemContainer itemContainer);

        IObservable<(int id, IItemContainer value)> ObserveAdd();

        IObservable<(int id, IItemContainer value)> ObserveRemove();
    }
}
