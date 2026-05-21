#nullable enable
namespace CCEnvs.UnityX.Items
{
    public interface IItemContainer
        : IItemAccessor,
        IItemContainerInfo,
        IShallowCloneable<IItemContainer>
    {
        bool IsReadOnlyContainer { get; }
        bool UnlockCapacity { get; set; }
    }
}
