#nullable enable
namespace CCEnvs.UnityX.Items
{
    public interface IItemContainer
        :
        IItemAccessor,
        IItemContainerInfo,
        IIDProvider<int?>,
        IShallowCloneable<IItemContainer>
    {
        bool IsReadOnlyContainer { get; }
        bool IgnoreMaxItemCount { get; set; }

        int Capacity { get; set; }

        new int? ID { get; set; }

        int? IIDProvider<int?>.ID => ID;

        IInventory? ParentInventory { get; }

        void SetParentInventory(IInventory? inventory);
    }

    public interface IItemContainer<TItem>
        :
        IItemContainer,
        IItemAccessor<TItem>,
        IItemContainerInfo<TItem>

        where TItem : IItem
    {
    }

    public static class IItemContainerExtensions
    {
        public static ReadOnlyItemContainer ToReadOnly(this IItemContainer container)
        {
            CC.Guard.IsNotNull(container, nameof(container));
            return new ReadOnlyItemContainer(container.Item, container.ItemCount);
        }

        public static ReadOnlyItemContainer<TItem> ToReadOnly<TItem>(this IItemContainer<TItem> container)
            where TItem : IItem
        {
            CC.Guard.IsNotNull(container, nameof(container));
            return new ReadOnlyItemContainer<TItem>(container.Item, container.ItemCount);
        }
    }

    //public static class IItemContainerExtensions
    //{
    //    public static ReadOnlyItemContainer<IItem> TakeItemOrFromParentInventory()
    //}
}
