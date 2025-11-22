using CCEnvs.Unity.Injections;
using CCEnvs.Unity.Items;
using CCEnvs.Unity.UI;
using Cysharp.Threading.Tasks;
using System.Collections.Generic;
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
        protected GameObjectList slotBag;

        [SerializeField]
        protected int itemContainerCount;

        public GameObjectList SlotBag => slotBag;

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
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            SlotBag.Clear();
        }

        private void BindAddContainer()
        {
            viewModelUnsafe.ObserveAdd()
                     .SubscribeWithState(this,
                     static (cnt, @this) =>
                     {
                         var go = Instantiate(@this.ContainerPrefab, @this.transform);
                         go.transform.SetSiblingIndex(cnt.Key);
                         var goCnt = go.QueryTo().ByChildren().Model<IItemContainer>().Strict();

                         @this.viewModelUnsafe.Replace.Execute(KeyValuePair.Create(cnt.Key, goCnt));
                         @this.slotBag.Add(go);
                     })
                     .AddTo(this);
        }

        private void BindRemoveContainer()
        {
            viewModelUnsafe.ObserveRemove()
                     .SubscribeWithState(this,
                     static (cnt, @this) =>
                     {
                         @this.viewModelUnsafe.Remove.Execute(cnt.Key);
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
    public class InventoryView : InventoryView<InventoryViewModel<Inventory>>
    {
    }
}
