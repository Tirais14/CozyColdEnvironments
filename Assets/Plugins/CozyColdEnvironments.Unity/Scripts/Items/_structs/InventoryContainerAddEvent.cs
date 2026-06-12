#nullable enable
namespace CCEnvs.UnityX.Items
{
    public readonly struct InventoryContainerAddEvent
    {
        public int ID { get; init; }

        public IItemContainer Container { get; init; }

        public InventoryContainerAddEvent<TItemContainer> Convert<TItemContainer>()
            where TItemContainer : IItemContainer
        {
            return new InventoryContainerAddEvent<TItemContainer> { ID = ID, Container = Container.CastTo<TItemContainer>() };
        }
    }

    public readonly struct InventoryContainerAddEvent<TItemContainer>
        where TItemContainer : IItemContainer
    {
        public int ID { get; init; }

        public TItemContainer Container { get; init; }

        public static implicit operator InventoryContainerAddEvent(InventoryContainerAddEvent<TItemContainer> instance)
        {
            return new InventoryContainerAddEvent { ID = instance.ID, Container = instance.Container };
        }
    }
}
