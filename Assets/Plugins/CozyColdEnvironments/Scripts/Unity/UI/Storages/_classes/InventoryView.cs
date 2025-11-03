using CCEnvs.Collections;
using CCEnvs.Unity.Injections;
using CCEnvs.Unity.Storages;
using CCEnvs.Unity.UI.Elements;
using CCEnvs.Unity.UI.MVVM;
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
            BindAddContainer();
            BindRemoveContainer();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            SlotBag.Clear();
        }

        public void SetInventory(TInventory inventory)
        {
            CC.Guard.IsNotNull(inventory, nameof(inventory));
        }

        private void SetupSlotBag()
        {
            SlotBag.settings = IGameObjectBag.Settings.ReparentByRootMarker
                |
                IGameObjectBag.Settings.ActivateOnAdd
                |
                IGameObjectBag.Settings.DeactivateOnRemove;

            SlotBag.AddRange(viewModel.GetInventoryContainerGameObjects());
        }

        private void BindAddContainer()
        {
            viewModel.ObserveAddContainer().SubscribeWithState(SlotBag, (go, bag) => bag.Add(go));
        }

        private void BindRemoveContainer()
        {
            viewModel.ObserveRemoveContainer().SubscribeWithState(SlotBag, (go, bag) => bag.Remove(go));
        }
    }
    public class InventoryView : InventoryView<InventoryViewModel<Inventory>, Inventory>
    {
    }
}
