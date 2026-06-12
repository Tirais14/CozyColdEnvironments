#nullable enable
namespace CCEnvs.UnityX.Items
{
    public interface IItemContainer
        :
        IItemAccessor,
        IItemContainerInfo,
        IShallowCloneable<IItemContainer>
    {
        bool IsReadOnlyContainer { get; }
        bool UnlockCapacity { get; set; }
    }

    public interface IItemContainer<TItem>
        :
        IItemContainer,
        IItemAccessor<TItem>,
        IItemContainerInfo<TItem>

        where TItem : IItem
    {

    }

    public interface IItemContainer<TItem, TItemContainerInfo>
        :
        IItemContainer,
        IItemAccessor<TItem, TItemContainerInfo>,
        IItemContainerInfo<TItem>

        where TItem : IItem
        where TItemContainerInfo : IItemContainerInfo<TItem>
    {

    }
}
