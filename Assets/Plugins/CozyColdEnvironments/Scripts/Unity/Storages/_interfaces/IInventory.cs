using CCEnvs.FuncLanguage;
using System;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

#nullable enable
namespace CCEnvs.Unity.Storages
{
    public interface IInventory
        : IItemAccessor,
        IItemContainerInfoItemless,
        IEnumerable<IItemContainer>
    {
        IItemContainer this[int id] { get; }

        IEnumerable<int> IDs { get; }
        IEnumerable<IItemContainer> Containers { get; }
        int ContainerCount { get; }
        Maybe<IItemContainer> ActiveContainer { get; set; }

        IObservable<(int id, IItemContainer value)> ObserveAddContainer();

        IObservable<(int id, IItemContainer value)> ObserveRemoveContainer();

        void AddContainer(IItemContainer itemContainer);
        void AddContainer(GameObject toInstantiate);

        void AddContainerCount(int count, GameObject toInstantiate);
        void AddContainerCount<T>(int count) where T : IItemContainer, new();

        bool RemoveContainer(int id);
        bool RemoveContainer(IItemContainer itemContainer);

        void RemoveContainerCount(int count);

        bool ContainsContainer(int id);
        bool ContainsContainer(IItemContainer itemContainer);

        void SetContainerCount(int count, GameObject toInstantiate);
        void SetContainerCount<T>(int count) where T : IItemContainer, new();

        bool IsContainerActive(int id);
        bool IsContainerActive(IItemContainer itemContainer);

        void ActivateContainer(int id);

        bool SwitchContainerActiveState(int id);

        void ClearContainers();

        Maybe<int> GetContainerID(IItemContainer itemContainer);

        IObservable<Maybe<IItemContainer>> ObserveActiveItemContainer();
    }
}
