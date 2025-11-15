#nullable enable
using CCEnvs.Unity.UI.MVVM;

namespace CCEnvs.Unity.Items
{
    public interface IItemContainer
        : IItemAccessor,
        IItemContainerInfo,
        IShallowCloneable<IItemContainer>, 
        IModel
    {
    }
}
