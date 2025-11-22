using CCEnvs.FuncLanguage;
using CCEnvs.Unity.Injections;
using CCEnvs.Unity.Items;
using CCEnvs.Unity.UI;
using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;

#nullable enable
namespace CCEnvs.Unity.Storages.UI
{
    public abstract class InventoryView<TViewModel>
        : View<TViewModel>

        where TViewModel : IInventoryViewModel
    {
        public GameObject ContainerPrefab;

        [GetByChildren]
        [SerializeField]
        protected GameObjectList slots;

        [SerializeField]
        protected int itemContainerCount;

        protected Dictionary<int, GameObject> instantiatedGameObjects = new();

        public GameObjectList Slots => slots;

        protected override void Start()
        {
            base.Start();
            SetupOnAddSlotGameObject();
        }

        protected override void Init()
        {
            base.Init();
            BindAddContainer();
            BindRemoveContainer();
            BindResetInventory();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            Slots.Clear();
        }

        private void BindAddContainer()
        {
            viewModelUnsafe.ObserveAdd()
                     .SubscribeWithState(this,
                     static (cnt, @this) =>
                     {
                         var go = Instantiate(@this.ContainerPrefab, @this.transform);
                         go.transform.SetSiblingIndex(cnt.Key);
                         var view = go.QueryTo().ByChildren().Views().First(view => view.model.Raw is IItemContainer);
                         view.SetViewModelUnsafe(new ItemContainerViewModel<IItemContainer>(cnt.Value));
                         @this.slots.Add(go);
                         @this.instantiatedGameObjects.Add(cnt.Key, go);
                     })
                     .AddTo(this);
        }

        private void BindRemoveContainer()
        {
            viewModelUnsafe.ObserveRemove()
                     .SubscribeWithState(this,
                     static (cnt, @this) =>
                     {
                         if (@this.instantiatedGameObjects.Remove(cnt.Key, out GameObject go))
                             @this.slots.Remove(go);
                     })
                     .AddTo(this);
        }

        private void BindResetInventory()
        {
            viewModelUnsafe.ObserveReset()
                           .SubscribeWithState(this,
                           static (_, @this) =>
                           {
                               @this.slots.Clear();
                               @this.instantiatedGameObjects.Clear();
                           })
                           .AddTo(this);
        }

        private void SetupOnAddSlotGameObject()
        {
            slots.settings |= IGameObjectBag.Settings.DestroyOnRemove;
            Slots.ObserveAdd()
                 .SubscribeWithState(this, static (_, @this) => @this.Redraw())
                 .AddTo(this);
        }
    }
    public class InventoryView : InventoryView<InventoryViewModel<Inventory>>
    {
        protected override Maybe<InventoryViewModel<Inventory>> ViewModelFactory()
        {
            var inv = new Inventory(itemContainerCount);
            return new InventoryViewModel<Inventory>(inv);
        }
    }
}
