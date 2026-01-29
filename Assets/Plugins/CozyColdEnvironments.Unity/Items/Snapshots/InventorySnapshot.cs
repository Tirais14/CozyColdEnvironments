using CCEnvs.Snapshots;
using CCEnvs.TypeMatching;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

#nullable enable
namespace CCEnvs.Unity.Items.Snapshots
{
    public class InventorySnapshot : Snapshot<Inventory>
    {
        public ItemContainerSnapshot[] ItemContainers { get; set; } = Array.Empty<ItemContainerSnapshot>();
        public bool AutoSize { get; set; }

        public InventorySnapshot()
        {
        }

        public InventorySnapshot(Inventory target) : base(target)
        {
            ValidateInventory(target);
            ItemContainers = CaptureItemContainerStates(target);
        }

        public override bool TryRestore
            (Inventory? target,
            [NotNullWhen(true)] out Inventory? restored)
        {
            if (!CanRestore(target))
            {
                restored = null;
                return false;
            }

            target = new Inventory
            {
                AutoSize = AutoSize
            };

            foreach (var cnt in ItemContainers)
            {
                if (cnt.TryRestore(new ItemContainer(), out ItemContainer cntRestored))
                    target.AddContainer(cntRestored);
            }

            restored = target;
            return true;
        }

        public override bool CanRestore(Inventory? target) => true;

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
