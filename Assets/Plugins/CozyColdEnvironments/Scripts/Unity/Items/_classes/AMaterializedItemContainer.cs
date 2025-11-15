using CCEnvs.Unity.Components;

#nullable enable
#pragma warning disable S2933
namespace CCEnvs.Unity.Items
{
    public abstract class AMaterializedItemContainer : CCBehaviour
    {
        protected IItemContainer itemContainer { get; private set; } = null!;

        public void SetItemContainer(IItemContainer itemContainer)
        {
            CC.Guard.IsNotNull(itemContainer, nameof(itemContainer));
            if (itemContainer.Item.IsNone)
                throw new System.ArgumentException("Item container has no item.");

            this.itemContainer = itemContainer;
            OnSetItemContainer();
        }

        public IItemContainer Dematerialize()
        {
            Destroy(gameObject);

            return itemContainer;
        }

        protected abstract void OnSetItemContainer();
    }
}
