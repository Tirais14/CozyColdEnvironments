using CCEnvs.FuncLanguage;
using CCEnvs.Unity.Storages;
using CCEnvs.Unity.UI.MVVM;
using System;
using UniRx;
using UnityEngine;

namespace CCEnvs.Unity.UI.Storages
{
    public interface IInventoryViewModel<out T> : IViewModel<T>
        where T : IInventory
    {
        IReadOnlyReactiveProperty<MaybeStruct<int>> ActiveContainerID { get; }

        IObservable<GameObject> ObserveAddContainer();

        IObservable<GameObject> ObserveRemoveContainer();
    }
}
