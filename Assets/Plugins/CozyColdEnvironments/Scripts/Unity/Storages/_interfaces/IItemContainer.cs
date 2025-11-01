#nullable enable
using CCEnvs.Unity.UI.MVVM;

namespace CCEnvs.Unity.Storages
{
    public interface IItemContainer
        : IItemAccessor,
        IItemContainerInfo,
        IShallowCloneable<IItemContainer>, 
        IModel
    {
    }
}
