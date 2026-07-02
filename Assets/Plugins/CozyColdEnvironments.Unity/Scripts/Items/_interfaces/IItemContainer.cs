#nullable enable
namespace CCEnvs.UnityX.Items
{
    public interface IItemContainer
        :
        IItemAccessor,
        IItemContainerInfo,
        IIDMarked<int?>,
        IShallowCloneable<IItemContainer>
    {
        bool IsReadOnlyContainer { get; }
        bool IgnoreMaxItemCount { get; set; }

        int Capacity { get; set; }

        new int? ID { get; set; }

        int? IIDMarked<int?>.ID => ID;

        IInventory? ParentInventory { get; }

        ReadOnlyItemContainer ToReadOnly();

        void SetParentInventory(IInventory? inventory);
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
