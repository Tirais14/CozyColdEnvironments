#nullable enable
namespace CCEnvs.UnityX.Items
{
    public readonly struct InventoryContainerRemoveEvent
    {
        public int ID { get; init; }

        public IItemContainer Container { get; init; }

        public InventoryContainerRemoveEvent<TItemContainer> Convert<TItemContainer>()
            where TItemContainer : IItemContainer
        {
            return new InventoryContainerRemoveEvent<TItemContainer> { ID = ID, Container = Container.CastTo<TItemContainer>() };
        }
    }

    public readonly struct InventoryContainerRemoveEvent<TItemContainer>
        where TItemContainer : IItemContainer
    {
        public int ID { get; init; }

        public TItemContainer Container { get; init; }

        public static implicit operator InventoryContainerRemoveEvent(InventoryContainerRemoveEvent<TItemContainer> instance)
        {
            return new InventoryContainerRemoveEvent { ID = instance.ID, Container = instance.Container };
        }
    }
}
