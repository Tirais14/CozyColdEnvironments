using CCEnvs.FuncLanguage;
using System;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

#nullable enable
namespace CCEnvs.Unity.Items
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
        Maybe<GameObject> ItemContainerPrefab { get; set; }

        void AddContainer(IItemContainer itemContainer);
        void AddContainerByPrefab(in IItemContainer itemContainer, GameObject prefab);
        void AddContainerByPrefab(in IItemContainer itemContainer);

        void AddContainerCount<T>(int count) where T : IItemContainer, new();
        void AddContainerCountByPrefab(int count, GameObject prefab);
        void AddContainerCountByPrefab(int count);

        bool RemoveContainer(int id);
        bool RemoveContainer(IItemContainer itemContainer);

        void RemoveContainerCount(int count);

        bool ContainsContainer(int id);
        bool ContainsContainer(IItemContainer itemContainer);

        void SetContainerCount<T>(int count) where T : IItemContainer, new();
        void SetContainerCountByPrefab(int count, GameObject prefab);
        void SetContainerCountByPrefab(int count);

        bool IsContainerActive(int id);
        bool IsContainerActive(IItemContainer itemContainer);

        void ActivateContainer(int id);

        bool SwitchContainerActiveState(int id);

        void ClearContainers();

        Maybe<int> GetContainerID(IItemContainer itemContainer);

        IObservable<(int id, IItemContainer value)> ObserveAddContainer();

        IObservable<(int id, IItemContainer value)> ObserveRemoveContainer();

        IObservable<Maybe<IItemContainer>> ObserveActiveItemContainer();
    }
}
