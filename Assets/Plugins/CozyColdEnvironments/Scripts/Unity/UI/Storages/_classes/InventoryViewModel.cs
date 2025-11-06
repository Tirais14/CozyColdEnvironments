using CCEnvs.FuncLanguage;
using CCEnvs.Unity.Storages;
using CCEnvs.Unity.UI.MVVM;
using System;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;

#nullable enable
namespace CCEnvs.Unity.UI.Storages
{
    public class InventoryViewModel<T> : ViewModel<T>, IInventoryViewModel<T>
        where T : IInventory
    {
        private readonly ReactiveProperty<MaybeStruct<int>> activeContainerID = new();
        private readonly Dictionary<GameObject, GameObject> addedContainerGameObjects = new();

        public IReadOnlyReactiveProperty<MaybeStruct<int>> ActiveContainerID => activeContainerID;
        public override bool ModelMutable => true;

        public InventoryViewModel(T model, GameObject gameObject) 
            :
            base(model, gameObject)
        {
            BindActiveContainer();
        }

        private void BindActiveContainer()
        {
            model.ActiveContainer.Select(cnt => cnt.Map(cnt => cnt.GetContainerID()).Raw)
                                 .SubscribeWithState(activeContainerID, (id, prop) => prop.Value = id)
                                 .AddTo(disposables);
        }

        public IObservable<GameObject> ObserveAddContainer()
        {
            return (from added in model.ObserveAddContainer()
                    select added.value.gameObject into go
                    where go.IsSome
                    select (src: go.Raw, root: go.AccessUnsafe().transform.root.gameObject))
                    .Do(x => addedContainerGameObjects.Add(x.src, x.root))
                    .Select(x => x.root);
        }

        public IObservable<GameObject> ObserveRemoveContainer()
        {
            return (from removed in model.ObserveRemoveContainer()
                    select removed.value.gameObject into go
                    where go.IsSome
                    select go.Raw)
                    .Do(go => addedContainerGameObjects.Remove(go));
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
