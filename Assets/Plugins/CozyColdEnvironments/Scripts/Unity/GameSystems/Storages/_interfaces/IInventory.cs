using CCEnvs.Unity.GameSystems.Storages;
using System;
using System.Collections.Generic;

#nullable enable
namespace CCEnvs.Unity
{
    public interface IInventory
        : IItemAccessor,
        IItemContainerInfoItemless
    {
        IItemContainer this[int id] { get; }

        event Action<(int id, IItemContainer value)> OnAdd;
        event Action<(int id, IItemContainer value)> OnRemove;

        IEnumerable<int> IDs { get; }
        IEnumerable<IItemContainer> Containers { get; }
        int Count { get; }

        void Add(IItemContainer itemContainer);

        void AddCount<T>(int count) where T : IItemContainer, new();

        bool Remove(int id);
        bool Remove(IItemContainer itemContainer);

        bool Contains(int id);
        bool Contains(IItemContainer itemContainer);

        void RemoveCount(int count);

        void SetCount<T>(int count) where T : IItemContainer, new();

        IObservable<(int id, IItemContainer value)> ObserveAdd();

        IObservable<(int id, IItemContainer value)> ObserveRemove();
    }
}
