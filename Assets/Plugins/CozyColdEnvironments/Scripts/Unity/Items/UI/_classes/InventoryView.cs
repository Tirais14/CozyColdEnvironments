using CCEnvs.FuncLanguage;
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
        protected GameObjectList slots;

        [SerializeField]
        protected int itemContainerCount;

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
                         var goCnt = go.QueryTo().ByChildren().Model<IItemContainer>().Strict();
                         goCnt.CopyFrom(cnt.Value);

                         @this.viewModelUnsafe.Replace.Execute(KeyValuePair.Create(cnt.Key, goCnt));
                         @this.slots.Add(go);
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
                         @this.slots.Remove(@this.transform.GetChild(cnt.Key).gameObject);
                     })
                     .AddTo(this);
        }

        private void BindResetInventory()
        {
            viewModelUnsafe.ObserveReset()
                           .SubscribeWithState(this, (_, @this) => @this.slots.Clear())
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
