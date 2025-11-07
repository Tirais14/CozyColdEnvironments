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
        public override bool IsMutable => true;

        protected override void Start()
        {
            base.Start();
            SetSlotBagSettings();
        }

        protected override void InstallBingings()
        {
            base.InstallBingings();
            AddSlotGameObjectsToBag();
            BindAddContainer();
            BindRemoveContainer();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            SlotBag.Clear();
        }

        private void AddSlotGameObjectsToBag()
        {
            SlotBag.Clear();
            SlotBag.AddRange(viewModel.GetInventoryContainerGameObjects());
        }

        private void SetSlotBagSettings()
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
