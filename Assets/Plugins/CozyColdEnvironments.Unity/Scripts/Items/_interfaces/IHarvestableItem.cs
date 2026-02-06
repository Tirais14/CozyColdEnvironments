using System.Collections.Immutable;

#nullable enable
namespace CCEnvs.Unity.Items
{
    public interface IHarvestableItem : IItem
    {
        ImmutableArray<IItemContainer> OutputItems { get; }
        ImmutableArray<IItem> RequiredItems { get; }
    }
}
