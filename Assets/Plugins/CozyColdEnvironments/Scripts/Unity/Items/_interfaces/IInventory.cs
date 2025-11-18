using CCEnvs.FuncLanguage;
using CCEnvs.Unity.UI.MVVM;
using System;
using System.Collections.Generic;
using UnityEngine;

#nullable enable
namespace CCEnvs.Unity.Items
{
    public interface IInventory
        : IItemAccessor,
        IItemContainerInfoItemless,
        IEnumerable<IItemContainer>,
        IModel
    {
        IItemContainer this[int id] { get; }

        IEnumerable<int> IDs { get; }
        IEnumerable<IItemContainer> Containers { get; }
        int ContainerCount { get; }
        Maybe<IItemContainer> ActiveContainer { get; set; }
        Maybe<GameObject> ItemContainerPrefab { get; set; }

        void AddContainer(IItemContainer itemContainer);
        IItemContainer AddContainerByPrefab(in IItemContainer itemContainer, GameObject prefab);
        IItemContainer AddContainerByPrefab(in IItemContainer itemContainer);

        IItemContainer[] AddContainerCount<T>(int count) where T : IItemContainer, new();
        IItemContainer[] AddContainerCountByPrefab(int count, GameObject prefab);
        IItemContainer[] AddContainerCountByPrefab(int count);

        IItemContainer[] SetContainerCount<T>(int count) where T : IItemContainer, new();
        IItemContainer[] SetContainerCountByPrefab(int count, GameObject prefab);
        IItemContainer[] SetContainerCountByPrefab(int count);

        Maybe<IItemContainer> RemoveContainer(int id);
        bool RemoveContainer(IItemContainer itemContainer);

        IItemContainer[] RemoveContainerCount(int count);

        bool ContainsContainer(int id);
        bool ContainsContainer(IItemContainer itemContainer);

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
