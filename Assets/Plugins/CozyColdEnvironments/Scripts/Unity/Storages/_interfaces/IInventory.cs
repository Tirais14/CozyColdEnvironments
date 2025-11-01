using CCEnvs.FuncLanguage;
using System;
using System.Collections.Generic;
using UnityEngine;

#nullable enable
namespace CCEnvs.Unity.Storages
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
        void Add(GameObject toInstantiate);

        void AddCount(int count, GameObject toInstantiate);
        void AddCount<T>(int count) where T : IItemContainer, new();

        bool Remove(int id);
        bool Remove(IItemContainer itemContainer);

        void RemoveCount(int count);

        bool Contains(int id);
        bool Contains(IItemContainer itemContainer);

        void SetCount(int count, GameObject toInstantiate);
        void SetCount<T>(int count) where T : IItemContainer, new();

        MaybeStruct<int> GetID(IItemContainer itemContainer);

        IObservable<(int id, IItemContainer value)> ObserveAdd();

        IObservable<(int id, IItemContainer value)> ObserveRemove();
    }
}
