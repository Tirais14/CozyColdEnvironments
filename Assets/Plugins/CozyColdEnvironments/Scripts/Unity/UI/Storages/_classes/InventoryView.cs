using CCEnvs.Diagnostics;
using CCEnvs.Unity.Injections;
using CCEnvs.Unity.Storages;
using CCEnvs.Unity.UI.Elements;
using CCEnvs.Unity.UI.MVVM;
using System.Net.NetworkInformation;
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

        protected override void Awake()
        {
            base.Awake();

            BindActiveContainer();
            BindAddContainer();
            BindRemoveContainer();
        }

        private void BindActiveContainer()
        {
            viewModel.ActiveContainerID.Where(x => x.IsSome)
                                         .Select(x => x.Target)
                                         .SubscribeWithState(model, (id, inv) => inv.ActivateContainer(id))
                                         .AddTo(this);
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
