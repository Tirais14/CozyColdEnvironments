#nullable enable
using System.Collections.Generic;

namespace CCEnvs.Unity.Items
{
    public interface IHarvestable
    {
        IEnumerable<IItem> OutputItems { get; }

        bool CanHarvested();

        bool CanHarvestedBy(IItem? item);

        bool TryHarvest(out IItemContainer[] results);

        bool TryHarvestBy(IItem item, out IItemContainer[] results);
    }
}
