using CCEnvs.FuncLanguage;
using CCEnvs.Unity.Items;
using CCEnvs.Unity.UI;
using System;
using UnityEngine;

#nullable enable
namespace CCEnvs.Unity.Storages.UI
{
    public abstract class InventoryView<TViewModel>
        :
        View<TViewModel>

        where TViewModel : IInventoryViewModel
    {
        [Header("Inventory Settings")]
        [Space(8)]

        [SerializeField]
        protected GameObject containerPrefab;

        [SerializeField]
        protected Transform? containersRoot;

        public ISelectableController<IItemContainer> SelectableController { get; private set; } = null!;

        public GameObject ContainerPrefab {
            get => containerPrefab;
            set => SetContainerPrefab(value);
        }

        public Transform? ContainersRoot {
            get => containersRoot;
            set => SetContainersRoot(value);
        }

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
            SetContainersRoot(containersRoot);
        }

        public InventoryView<TViewModel> SetContainerPrefab(GameObject value)
        {
            CC.Guard.IsNotNull(value);
            containerPrefab = value;
            return this;
        }

        public InventoryView<TViewModel> SetContainersRoot(Transform? value)
        {
            containersRoot = value.IfNull(transform);
            return this;
        }
    }
    public class InventoryView : InventoryView<InventoryViewModel<IInventory>>
    {
        [SerializeField, Min(0)]
        protected int containerCount;

        [SerializeField]
        protected bool inventoryAutoSize;

        public int ContainerCount {
            get => containerCount;
            set => SetContainerCount(value);
        }

        public bool InventoryAutoSize {
            get => inventoryAutoSize;
            set => SetInventoryAutoSize(value);
        }

        public InventoryView SetContainerCount(int value)
        {
            containerCount = Math.Max(value, 0);
            return this;
        }

        public InventoryView SetInventoryAutoSize(bool value)
        {
            inventoryAutoSize = value;
            return this;
        }

        protected override Maybe<InventoryViewModel<IInventory>> CreateViewModel()
        {
            var inv = new Inventory(containerCount);

            return new InventoryViewModel<IInventory>(
                inv, 
                destroyCancellationToken,
                containerPrefab,
                containersRoot.IfNull(transform)
                );
        }
    }
}
