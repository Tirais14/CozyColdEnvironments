#nullable enable
using System;
using System.Collections.Immutable;

namespace CCEnvs.Unity.Items
{
    public interface IHarvestableConfig
    {
        ImmutableArray<IItemContainer> OutputItems { get; }
        ImmutableArray<IItem> RequiredItems { get; }
    }
}
