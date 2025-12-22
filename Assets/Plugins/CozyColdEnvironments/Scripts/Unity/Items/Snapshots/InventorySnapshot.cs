using CCEnvs.FuncLanguage;
using CCEnvs.Snapshots;
using CCEnvs.TypeMatching;
using System;
using System.Linq;

#nullable enable
namespace CCEnvs.Unity.Items.Snapshots
{
    public class InventorySnapshot : Snapshot<Inventory>
    {
        public ItemContainerSnapshot[] ItemContainers { get; set; } = Array.Empty<ItemContainerSnapshot>();
        public bool AutoSize { get; set; }

        public override bool IgnoreTarget => false;

        public InventorySnapshot()
        {
        }

        public InventorySnapshot(Inventory target) : base(target)
        {
            ValidateInventory(target);
            ItemContainers = CaptureItemContainerStates(target);
        }

        public override Maybe<Inventory> Restore(Inventory? target)
        {
            var inv = new Inventory
            {
                AutoSize = AutoSize
            };

            foreach (var cnt in ItemContainers.Select(x => x.Restore())
                .Where(x => x.IsSome)
                .Select(x => x.Raw!))
            {
                inv.AddContainer(cnt);
            }

            return inv;
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
