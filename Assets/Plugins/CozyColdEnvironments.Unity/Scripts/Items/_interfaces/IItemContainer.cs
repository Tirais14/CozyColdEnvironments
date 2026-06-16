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
        bool IgnoreMaxItemCount { get; set; }

        ReadOnlyItemContainer ToReadOnly();
    }

    public interface IItemContainer<TItem>
        :
        IItemContainer,
        IItemAccessor<TItem>,
        IItemContainerInfo<TItem>

        where TItem : IItem
    {
        new ReadOnlyItemContainer<TItem> ToReadOnly();

        ReadOnlyItemContainer IItemContainer.ToReadOnly() => ToReadOnly();
    }
}
