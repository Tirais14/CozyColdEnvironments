using CCEnvs.FuncLanguage;
using CCEnvs.Snapshots;
using System;

#nullable enable
namespace CCEnvs.Unity.Items.Snapshots
{
    [Serializable]
    public class ItemContainerSnapshot : Snapshot<ItemContainer>
    {
        public Maybe<IItem> Item { get; set; }
        public int ItemCount { get; set; }
        public int Capacity { get; set; }
        public bool IsReadOnlyContainer { get; set; }
        public bool UnlockCapacity { get; set; }

        public override bool IgnoreTarget => true;

        public ItemContainerSnapshot()
        {
        }

        public ItemContainerSnapshot(ItemContainer target) : base(target)
        {
            Item = target.Item;
            ItemCount = target.ItemCount;
            Capacity = target.Capacity;
            IsReadOnlyContainer = target.IsReadOnlyContainer;
            UnlockCapacity = target.UnlockCapacity;
        }

        public ItemContainerSnapshot(IItemContainer target)
            :
            this(new ItemContainer(
                item: target.Item.Raw,
                count: target.ItemCount,
                capacity: target.Capacity,
                isReadOnlyContainer: target.IsReadOnlyContainer)
                {
                    UnlockCapacity = target.UnlockCapacity,
                    Capacity = target.Capacity
                })
        {
        }

        public override Maybe<ItemContainer> Restore(ItemContainer? target)
        {
            CC.Guard.IsNotNull(target, nameof(target));

            return new ItemContainer(
                item: Item.Raw,
                count: ItemCount,
                isReadOnlyContainer: IsReadOnlyContainer)
            {
                UnlockCapacity = UnlockCapacity,
                Capacity = Capacity,
            };
        }
    }
}
