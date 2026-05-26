using CCEnvs.Pools;
using CCEnvs.TypeMatching;
using CCEnvs.UnityX.Injections;
using CCEnvs.UnityX.UI;
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
        protected GameObject containerPrefab;

        [SerializeField]
        protected Transform containersRoot;

        [SerializeField, GetByChildren(IsOptional = true)]
        protected ItemContainerViewSelectableController? containerSelectableController;

        public ItemContainerViewSelectableController? ContainerSelectableController => containerSelectableController;

        public GameObject ContainerPrefab {
            get => containerPrefab;
            set => SetContainerPrefab(value);
        }

        public Transform? ContainersRoot {
            get => containersRoot;
            set => SetContainersRoot(value);
        }

        protected override void Start()
        {
            base.Start();
            SetContainersRoot(containersRoot);
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

        private void InitItemContainers(TViewModel vm)
        {
            var cntViews = containersRoot.Q()
                .FromChildrens()
                .ExcludeSelf()
                .IncludeInactive()
                .Components<IView>();

            using var cnts = ListPool<IItemContainer>.Shared.Get();

            foreach (var cntView in cntViews)
            {
                if (!cntView.Model.Is<IItemContainer>(out var cnt))
                    cnt = new ItemContainer();

                cnts.Value.Add(cnt);
            }

            foreach (var cnt in cnts.Value)
                vm.AddContainer(cnt);
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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public InventoryView SetContainerCount(int value)
        {
            containerCount = Math.Max(value, 0);
            return this;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public InventoryView SetInventoryAutoSize(bool value)
        {
            inventoryAutoSize = value;
            return this;
        }

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
