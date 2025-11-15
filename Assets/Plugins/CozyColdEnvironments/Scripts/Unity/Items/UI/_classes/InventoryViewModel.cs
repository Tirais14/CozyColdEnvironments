using CCEnvs.FuncLanguage;
using CCEnvs.Unity.Items;
using CCEnvs.Unity.UI.MVVM;
using System;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;

#nullable enable
namespace CCEnvs.Unity.Storages.UI
{
    public class InventoryViewModel<T> : ViewModel<T>, IInventoryViewModel<T>
        where T : IInventory
    {
        private readonly ReactiveProperty<Maybe<int>> activeContainerID = new();

        public IReadOnlyReactiveProperty<Maybe<int>> ActiveContainerID => activeContainerID;
        public override bool ModelMutable => true;

        public InventoryViewModel(T model, GameObject gameObject) 
            :
            base(model, gameObject)
        {
            BindActiveContainer();
        }

        private void BindActiveContainer()
        {
            model.ObserveActiveItemContainer()
                .Select(cnt => cnt.Map(cnt => cnt.GetContainerID()).Raw)
                .SubscribeWithState(activeContainerID, (id, prop) => prop.Value = id)
                .AddTo(disposables);

            activeContainerID.AddTo(disposables);
        }

        public IObservable<GameObject> ObserveAddContainer()
        {
            return from added in model.ObserveAddContainer()
                   select added.value.gameObject into go
                   where go.IsSome
                   select go.GetValueUnsafe() into go
                   select go.AppealTo().RootTransform().gameObject;
        }

        public IObservable<GameObject> ObserveRemoveContainer()
        {
            return from added in model.ObserveRemoveContainer()
                   select added.value.gameObject into go
                   where go.IsSome
                   select go.GetValueUnsafe() into go
                   select go.AppealTo().RootTransform().gameObject;
        }


        public IEnumerable<GameObject> GetInventoryContainerGameObjects()
        {
            return from cnt in model
                   select cnt.gameObject.Raw into go
                   where go != null
                   select go;
        }
    }
}
