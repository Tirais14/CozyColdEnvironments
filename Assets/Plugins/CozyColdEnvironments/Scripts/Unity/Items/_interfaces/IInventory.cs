using CCEnvs.Unity.Collections;
using CCEnvs.Unity.UI.MVVM;

#nullable enable
namespace CCEnvs.Unity.Items
{
    public interface IInventory
        : IItemAccessor,
        IItemContainerInfoItemless,
        INodeCollection<int, IItemContainer>,
        IActivatableController<int, IItemContainer>,
        IModel
    {
        void ResetItemContainers();
    }
}
