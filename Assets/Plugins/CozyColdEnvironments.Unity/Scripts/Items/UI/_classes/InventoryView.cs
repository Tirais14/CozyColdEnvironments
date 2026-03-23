using CCEnvs.FuncLanguage;
using CCEnvs.Unity.Items;
using CCEnvs.Unity.UI;
using UnityEngine;

#nullable enable
namespace CCEnvs.Unity.Storages.UI
{
    public abstract class InventoryView<TViewModel>
        : View<TViewModel>

        where TViewModel : IInventoryViewModel
    {
        [Header("Inventory Settings")]
        [Space(8)]

        [SerializeField]
        protected GameObject containerPrefab;

        [SerializeField]
        protected int containerCount;

        [SerializeField]
        protected bool inventoryAutoSize;

        public ISelectableController<IItemContainer> SelectableController { get; private set; } = null!;

        protected override void Awake()
        {
            base.Awake();

            SelectableController = this.QueryTo()
                .Component<ISelectableController<IItemContainer>>()
                .Lax()
                .GetValue(() => gameObject.AddComponent<ItemContainerViewSelectableObserver>());
        }

        protected override void Start()
        {
            base.Start();
        }

        protected override void InitViewModel()
        {
            base.InitViewModel();
        }

        private void BindContainerAdd()
        {
            viewModelUnsafe.
        }
    }
    public class InventoryView : InventoryView<InventoryViewModel<IInventory>>
    {
        protected override Maybe<InventoryViewModel<IInventory>> CreateViewModel()
        {
            var inv = new Inventory(itemContainerCount);
            return new InventoryViewModel<IInventory>(inv, destroyCancellationToken);
        }
    }
}
