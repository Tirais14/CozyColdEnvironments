using CCEnvs.Collections;
using CCEnvs.Unity.Injections;
using CCEnvs.Unity.Items;
using CCEnvs.Unity.UI;
using CCEnvs.Unity.UI.MVVM;
using Cysharp.Threading.Tasks;
using UniRx;

#nullable enable
namespace CCEnvs.Unity.Storages.UI
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
            SetupOnAddSlotGameObject();
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

        private void SetupOnAddSlotGameObject()
        {
            SlotBag.ObserveAdd()
                .Select(ev => ev.Value)
                .Where(go => go.activeSelf)
                .SubscribeWithState(this, static (go, @this) =>
                {
                    go.SetActive(false);

                    @this.PreUpdateAction(() =>
                    {
                        if (!@this.IsVisible) //pass control of visibility state to @this
                        {
                            @this.Show();
                            @this.Hide();
                        }

                        go.SetActive(true);
                    });
                })
                .AddTo(this);
        }
    }
    public class InventoryView : InventoryView<InventoryViewModel<Inventory>, Inventory>
    {
    }
}
