using CCEnvs.Snapshots;
using CCEnvs.TypeMatching;
using Newtonsoft.Json;
using System;
using System.Linq;

#nullable enable
namespace CCEnvs.Unity.Items.Snapshots
{
    public class InventorySnapshot : Snapshot<Inventory>
    {
        [JsonProperty("itemContainers")]
        private ItemContainerSnapshot[] itemContainers = Array.Empty<ItemContainerSnapshot>();

        [JsonProperty("autoSize")]
        private bool autoSize;

        public InventorySnapshot()
        {
        }

        public InventorySnapshot(Inventory target) : base(target)
        {
            ValidateInventory(target);
            itemContainers = CaptureItemContainerStates(target);
        }

        public override Inventory Restore(Inventory target)
        {
            var inv = new Inventory
            {
                AutoSize = autoSize
            };

            foreach (var cnt in itemContainers.Select(x => x.Restore()))
                inv.AddContainer(cnt);

            return inv;
        }

        private static bool ValidateInventory(Inventory target)
        {
            if (target.FirstOrDefault(x => x.IsNot<ItemContainer>()).Let(out var cnt))
            {
                typeof(InventorySnapshot).PrintError($"{nameof(ItemContainer)}: {cnt} not supported. Will be used {typeof(ItemContainer)} instead");
                return false;
            }

            return true;
        }

        private static ItemContainerSnapshot[] CaptureItemContainerStates(Inventory target)
        {
            return target.Containers.Select(cnt => new ItemContainerSnapshot(cnt)).ToArray();
        }
    }

    public static class InventorySnapshotExntesions
    {
        public static InventorySnapshot CaptureState(this Inventory source)
        {
            return new InventorySnapshot(source);
        }
    }
}
