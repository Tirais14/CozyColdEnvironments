using CCEnvs.Pools;
using CCEnvs.TypeMatching;
using CCEnvs.UnityX.ComponentInjections;
using CCEnvs.UnityX.UI;
using CCEnvs.UnityX.UI.Elements;
using System;
using System.Runtime.CompilerServices;
using UnityEngine;

#nullable enable
namespace CCEnvs.UnityX.Items.UI
{
    public abstract class InventoryView<TViewModel>
        :
        View<TViewModel>

        where TViewModel : IInventoryViewModel
    {
        [Header("Inventory Settings")]
        [Space(8)]

        [SerializeField]
        [Tooltip("Must be contains in childrens or self " + nameof(IView) + " with " + nameof(IItemContainerViewModel) + " and " + nameof(IItemContainer) + " as model. Also " + nameof(ShowableElement.ShowOnInited) + " must be true")]
        protected GameObject containerPrefab;

        [SerializeField]
        protected Transform containersRoot;

        [SerializeField, GetByChildren(IsOptional = true)]
        protected ItemContainerViewSelectableController? containerSelectableController;

        [SerializeField, Min(0)]
        protected int containerCount;

        [SerializeField]
        protected bool inventoryAutoSize;

        public ItemContainerViewSelectableController? ContainerSelectableController => containerSelectableController;

        /// <summary>
        /// Must be contains in childrens or self <see cref="IView"/> with <see cref="IItemContainerViewModel"/> and <see cref="IItemContainer"/> as model. Also <see cref="ShowableBase{TSelf}.ShowOnInited"/> must be true
        /// </summary>
        public GameObject ContainerPrefab {
            get => containerPrefab;
            set => SetContainerPrefab(value);
        }

        public Transform? ContainersRoot {
            get => containersRoot;
            set => SetContainersRoot(value);
        }

        public int ContainerCount {
            get => containerCount;
            set => SetContainerCount(value);
        }

        public bool InventoryAutoSize {
            get => inventoryAutoSize;
            set => SetInventoryAutoSize(value);
        }

        protected override void Start()
        {
            base.Start();
            SetContainersRoot(containersRoot);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public InventoryView<TViewModel> SetContainerCount(int value)
        {
            containerCount = Math.Max(value, 0);
            return this;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public InventoryView<TViewModel> SetInventoryAutoSize(bool value)
        {
            inventoryAutoSize = value;
            return this;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public InventoryView<TViewModel> SetContainerPrefab(GameObject value)
        {
            CC.Guard.IsNotNull(value);
            containerPrefab = value;
            return this;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public InventoryView<TViewModel> SetContainersRoot(Transform? value)
        {
            containersRoot = value.IfNull(transform);
            return this;
        }

        protected override void OnSetViewModel(TViewModel? vm)
        {
        }

        protected override void InitViewModel(TViewModel vm)
        {
            InitItemContainers(vm);
        }

        protected virtual IItemContainer CreateItemContainer() => new ItemContainer();

        protected virtual void InitItemContainers(TViewModel viewModel)
        {
            var containerViews = containersRoot.Q()
                .FromChildrens()
                .ExcludeSelf()
                .IncludeInactive()
                .Components<IView>();

            using var containers = ListPool<IItemContainer>.Shared.Get();

            foreach (var containerView in containerViews)
            {
                if (containerView.Model.IsNot<IItemContainer>(out var container))
                    container = CreateItemContainer();

                containers.Value.Add(container);
            }

            foreach (var container in containers.Value)
                viewModel.AddContainer(container);
        }
    }
    public class InventoryView : InventoryView<InventoryViewModel<IInventory>>
    {
        protected override InventoryViewModel<IInventory> CreateViewModel()
        {
            var inv = Inventory.CreateWith<ItemContainer>(containerCount);

            return new InventoryViewModel<IInventory>(
                inv,
                containerPrefab,
                containersRoot
                );
        }
    }
}
