#nullable enable
namespace CCEnvs.Unity.GameSystems.Storages
{
    public interface IItemContainer
        : IItemAccessor,
        IItemContainerInfo,
        IShallowCloneable<IItemContainer>
    {
    }
}
