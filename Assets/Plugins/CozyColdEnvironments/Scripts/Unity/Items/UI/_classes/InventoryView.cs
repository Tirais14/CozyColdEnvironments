using CCEnvs.FuncLanguage;
using CCEnvs.TypeMatching;
using CCEnvs.Unity.Injections;
using CCEnvs.Unity.Items;
using CCEnvs.Unity.UI;
using CCEnvs.Unity.UI.MVVM;
using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;

#nullable enable
namespace CCEnvs.Unity.Storages.UI
{
    public abstract class InventoryView<TViewModel, TInventory>
        : View<TViewModel, TInventory>

        where TViewModel : ViewModel<TInventory>, IInventoryViewModel<TInventory>
        where TInventory : IInventory
    {
        public GameObject ContainerPrefab;

        [GetByChildren]
        [SerializeField]
        protected GameObjectList slotBag;

        public GameObjectList SlotBag => slotBag;

        protected override void Start()
        {
            base.Start();
            SetupOnAddSlotGameObject();
        }

        protected override void InstallBingings()
        {
            base.InstallBingings();
            BindAddContainer();
            BindRemoveContainer();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            SlotBag.Clear();
        }

        private void BindAddContainer()
        {
            viewModel.ObserveAdd()
                     .SubscribeWithState(this,
                     static (cnt, @this) =>
                     {
                         var go = Instantiate(@this.ContainerPrefab, @this.transform);
                         go.transform.SetSiblingIndex(cnt.Key);
                         var goCnt = go.QueryTo().ByChildren().Model<IItemContainer>().Strict();

                         @this.viewModel.Replace.Execute(KeyValuePair.Create(cnt.Key, goCnt));
                         @this.slotBag.Add(go);
                     })
                     .AddTo(this);
        }

        private void BindRemoveContainer()
        {
            viewModel.ObserveRemove()
                     .SubscribeWithState(this,
                     static (cnt, @this) =>
                     {
                         @this.viewModel.Remove.Execute(cnt.Key);
                         @this.slotBag.Remove(@this.transform.GetChild(cnt.Key).gameObject);
                     })
                     .AddTo(this);
        }

        private void SetupOnAddSlotGameObject()
        {
            SlotBag.ObserveAdd()
                   .SubscribeWithState(this, static (_, @this) => @this.Redraw())
                   .AddTo(this);
        }
    }
    public class InventoryView : InventoryView<InventoryViewModel<Inventory>, Inventory>
    {
    }
}
