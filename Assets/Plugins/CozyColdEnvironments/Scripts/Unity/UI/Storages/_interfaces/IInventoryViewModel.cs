using CCEnvs.FuncLanguage;
using CCEnvs.Unity.Storages;
using CCEnvs.Unity.UI.MVVM;
using System;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

namespace CCEnvs.Unity.UI.Storages
{
    public interface IInventoryViewModel<T> : IViewModel<T>
        where T : IInventory
    {
        IReadOnlyReactiveProperty<Maybe<int>> ActiveContainerID { get; }

        IObservable<GameObject> ObserveAddContainer();

        IObservable<GameObject> ObserveRemoveContainer();

        IEnumerable<GameObject> GetInventoryContainerGameObjects();
    }
}
