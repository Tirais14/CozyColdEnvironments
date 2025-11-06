using CCEnvs.Collections;
using CCEnvs.Unity.Injections;
using CCEnvs.Unity.Storages;
using CCEnvs.Unity.UI.Elements;
using CCEnvs.Unity.UI.MVVM;
using System;
using UniRx;

#nullable enable
namespace CCEnvs.Unity.UI.Storages
{
    public abstract class InventoryView<TViewModel, TInventory>
        : View<TViewModel, TInventory>

        where TViewModel : ViewModel<TInventory>, IInventoryViewModel<TInventory>
        where TInventory : IInventory
    {
        [field: GetByChildren]
        public GameObjectBag SlotBag { get; private set; } = null!;

        protected override void Start()
        {
            base.Start();

            SetupSlotBag();

        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            SlotBag.Clear();
        }

        protected override void SetupViewModel()
        {
            Init();
            BindAddContainer();
            BindRemoveContainer();
        }

        private void Init()
        {
            SlotBag.Clear();

            SlotBag.AddRange(viewModel.GetInventoryContainerGameObjects());
        }

        private void SetupSlotBag()
        {
            SlotBag.settings = IGameObjectBag.Settings.ReparentByRootMarker
                |
                IGameObjectBag.Settings.ActivateOnAdd
                |
                IGameObjectBag.Settings.DeactivateOnRemove;
        }

        private void BindAddContainer()
        {
            viewModel.ObserveAddContainer()
                     .SubscribeWithState(SlotBag, (go, bag) => bag.Add(go))
                     .AddTo(this);
        }

        private void BindRemoveContainer()
        {
            viewModel.ObserveRemoveContainer()
                     .SubscribeWithState(SlotBag, (go, bag) => bag.Remove(go))
                     .AddTo(this);
        }
    }
    public class InventoryView : InventoryView<InventoryViewModel<Inventory>, Inventory>
    {
    }
}
