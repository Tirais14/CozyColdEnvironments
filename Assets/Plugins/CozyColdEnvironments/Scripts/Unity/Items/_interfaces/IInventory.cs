using CCEnvs.Unity.Collections;

#nullable enable
namespace CCEnvs.Unity.Items
{
    public interface IInventory
        : IItemAccessor,
        IItemContainerInfoItemless,
        IReactiveDictionaryExtended<int, IItemContainer>
    {
        bool AutoSize { get; set; }

        void ResetItemContainers();

        void Add(IItemContainer itemContainer);
    }
}
