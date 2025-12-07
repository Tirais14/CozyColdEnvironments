#nullable enable
using CCEnvs.Unity.UI;

namespace CCEnvs.Unity.Items
{
    public interface IItemContainer
        : IItemAccessor,
        IItemContainerInfo,
        IShallowCloneable<IItemContainer>
    {
        bool IsReadOnlyContainer { get; }
    }
}
