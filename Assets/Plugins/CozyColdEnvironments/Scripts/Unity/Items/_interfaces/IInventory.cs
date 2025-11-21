using CCEnvs.Unity.Collections;
using UniRx;

#nullable enable
namespace CCEnvs.Unity.Items
{
    public interface IInventory
        : IItemAccessor,
        IItemContainerInfoItemless,
        IReactiveDictionary<int, IItemContainer>
    {
        void ResetItemContainers();

        void Add(IItemContainer itemContainer);
    }
}
