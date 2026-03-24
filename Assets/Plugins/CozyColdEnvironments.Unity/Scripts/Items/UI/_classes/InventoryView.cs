using CCEnvs.FuncLanguage;
using CCEnvs.Pools;
using CCEnvs.Reflection;
using CCEnvs.TypeMatching;
using CCEnvs.Unity.Injections;
using CCEnvs.Unity.Items;
using CCEnvs.Unity.UI;
using NUnit.Framework;
using System;
using System.Linq;
using UnityEngine;

#if ZLINQ_PLUGIN
using ZLinq;
#endif

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

        protected override void OnSetViewModel(TViewModel? vm)
        {
        }

        protected override void InitViewModel(TViewModel vm)
        {
            base.InitViewModel(vm);
            InitItemContainers(vm);
        }

        private void InitItemContainers(TViewModel vm)
        {
            var cntViews = containersRoot.Q()
                .FromChildrens()
                .ExcludeSelf()
                .IncludeInactive()
                .Components<IView>()
                .ToArray();
//#if ZLINQ_PLUGIN
//                .AsValueEnumerable();
//#endif

            using var cnts = ListPool<IItemContainer>.Shared.Get();

            foreach (var cntView in cntViews)
            {
                if (!cntView.Model.Is<IItemContainer>(out var cnt))
                    cnt = new ItemContainer();

                cnts.Value.Add(cnt);
            }

            foreach (var cmp in cntViews.OfType<Component>())
                Destroy(cmp);

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
                containersRoot
                );
        }
    }
}
