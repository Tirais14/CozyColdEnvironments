using CCEnvs.Unity.Components;

#nullable enable
#pragma warning disable S4035
namespace CCEnvs.Unity.Items
{
    public abstract class AMaterializedItem : CCBehaviour, ICollectable
    {
        protected IItem item { get; set; } = null!;

        public void SetItem(IItem item)
        {
            CC.Guard.IsNotNull(item, nameof(item));

            this.item = item;
            OnSetInternalItem();
        }

        public IItemContainer[] CollectItems()
        {
            Destroy(gameObject);
            return Range.From(new ItemContainer(item));
        }

        protected abstract void OnSetInternalItem();
    }
}
