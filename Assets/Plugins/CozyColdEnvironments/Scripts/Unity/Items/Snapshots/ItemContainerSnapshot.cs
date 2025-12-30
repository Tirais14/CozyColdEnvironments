using CCEnvs.FuncLanguage;
using CCEnvs.Snapshots;
using System;
using System.Diagnostics.CodeAnalysis;

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

        public override bool Restore(
            ItemContainer? target, 
            [NotNullWhen(true)] out ItemContainer? restored)
        {
            if (!CanRestore(target))
            {
                restored = null;
                return false;
            }

           target = new ItemContainer(
                item: Item.Raw,
                count: ItemCount,
                isReadOnlyContainer: IsReadOnlyContainer)
            {
                UnlockCapacity = UnlockCapacity,
                Capacity = Capacity,
            };

            restored = target;
            return true;
        }

        public override bool CanRestore(ItemContainer? target) => true;
    }
}
