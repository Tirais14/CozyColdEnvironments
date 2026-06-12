#nullable enable
namespace CCEnvs.UnityX.Items
{
    public readonly struct InventoryContainerReplaceEvent
    {
        public int ID { get; init; }

        public IItemContainer OldContainer { get; init; }
        public IItemContainer NewContainer { get; init; }

        public InventoryContainerReplaceEvent<TItemContainer> Convert<TItemContainer>()
            where TItemContainer : IItemContainer
        {
            return new InventoryContainerReplaceEvent<TItemContainer>
            {
                ID = ID,
                OldContainer = OldContainer.CastTo<TItemContainer>(),
                NewContainer = NewContainer.CastTo<TItemContainer>(),
            };
        }
    }

    public readonly struct InventoryContainerReplaceEvent<TItemContainer>
        where TItemContainer : IItemContainer
    {
        public int ID { get; init; }

        public TItemContainer OldContainer { get; init; }
        public TItemContainer NewContainer { get; init; }

        public static implicit operator InventoryContainerReplaceEvent(InventoryContainerReplaceEvent<TItemContainer> instance)
        {
            return new InventoryContainerReplaceEvent
            {
                ID = instance.ID,
                OldContainer = instance.OldContainer,
                NewContainer = instance.NewContainer,
            };
        }
    }
}
