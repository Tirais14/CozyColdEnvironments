#nullable enable
using System;

namespace CCEnvs.Unity.Items
{
    public interface IHarvestableConfig
    {
        Lazy<IItemContainer[]> OutputItems { get; }
        Lazy<IItem[]> RequiredItems { get; }
    }
}
