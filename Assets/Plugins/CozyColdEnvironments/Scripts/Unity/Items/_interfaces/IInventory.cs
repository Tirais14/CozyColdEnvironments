using CCEnvs.Unity.Collections;
using UniRx;

#nullable enable
namespace CCEnvs.Unity.Items
{
    public interface IInventory
        : IItemAccessor,
        IItemContainerInfoItemless,
        IReactiveDictionaryExtended<int, IItemContainer>
    {
        void ResetItemContainers();

        void Add(IItemContainer itemContainer);
    }
}
