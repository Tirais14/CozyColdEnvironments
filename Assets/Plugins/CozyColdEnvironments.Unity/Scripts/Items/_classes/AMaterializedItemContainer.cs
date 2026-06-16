using CCEnvs.UnityX.Components;

#nullable enable
#pragma warning disable S2933
namespace CCEnvs.UnityX.Items
{
    public abstract class AMaterializedItemContainer
        : CCBehaviour,
        ICollectable,
        IObjectProvider<IItemContainer>
    {
        protected IItemContainer itemContainer { get; private set; } = null!;

        IItemContainer IObjectProvider<IItemContainer>.InternalObject => itemContainer;

        public void SetItemContainer(IItemContainer itemContainer)
        {
            CC.Guard.IsNotNull(itemContainer, nameof(itemContainer));
            if (itemContainer.Item.IsNull())
                throw new System.ArgumentException("Item container has no item.");

            this.itemContainer = itemContainer;
            OnSetItemContainer();
        }

        public void SetItem(IItem item)
        {
            CC.Guard.IsNotNull(item, nameof(item));

            SetItemContainer(new ItemContainer(item));
        }

        public IItemContainer[] CollectItems()
        {
            Destroy(gameObject);
            return Range.From(itemContainer);
        }

        protected abstract void OnSetItemContainer();
    }
}
