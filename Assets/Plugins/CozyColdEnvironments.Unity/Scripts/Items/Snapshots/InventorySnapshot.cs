using CCEnvs.Collections;
using CCEnvs.Snapshots;
using CCEnvs.TypeMatching;
using System;
using System.Linq;
using UnityEngine;

#nullable enable
namespace CCEnvs.Unity.Items.Snapshots
{
    [Serializable]
    public class InventorySnapshot : Snapshot<Inventory>
    {
        [Header(nameof(Inventory))]
        [Space(8f)]

        [SerializeField]
        protected ItemContainerSnapshot[] itemContainers = new Arr<ItemContainerSnapshot>();

        [SerializeField]
        protected bool autoSize;

        public ItemContainerSnapshot[] ItemContainers {
            get => itemContainers;
            set => itemContainers = value;
        }

        public bool AutoSize {
            get => autoSize;
            set => autoSize = value;
        }

        public InventorySnapshot()
        {
        }

        public InventorySnapshot(Inventory target) : base(target)
        {
            ValidateInventory(target);
            ItemContainers = CaptureItemContainerStates(target);
        }

        public override bool CanRestore(Inventory? target) => true;

        protected override Inventory? CreateValue()
        {
            return new Inventory()
            {
                AutoSize = AutoSize
            };
        }

        protected override void OnRestore(ref Inventory target)
        {
            foreach (var cnt in ItemContainers)
            {
                if (cnt.TryRestore(new ItemContainer(), out ItemContainer? cntRestored))
                    target.AddContainer(cntRestored);
            }
        }

        private static void ValidateInventory(Inventory target)
        {
            if (target.FirstOrDefault(x => x.IsNot<ItemContainer>()).Let(out var cnt))
            {
                typeof(InventorySnapshot).PrintError($"{nameof(ItemContainer)}: {cnt} not supported. Will be used {typeof(ItemContainer)} instead");
            }
        }

        private static ItemContainerSnapshot[] CaptureItemContainerStates(Inventory target)
        {
            return target.Containers.Select(cnt => new ItemContainerSnapshot(cnt)).ToArray();
        }
    }
}
