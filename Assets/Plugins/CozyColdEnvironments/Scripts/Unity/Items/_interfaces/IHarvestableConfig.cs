#nullable enable
namespace CCEnvs.Unity.Items
{
    public interface IHarvestableConfig
    {
        IItemContainer[] OutputItems { get; }
        IItem[] RequiredItems { get; }
    }
}
