using CCEnvs.FuncLanguage;
using CCEnvs.Snapshots;
using System;
using Newtonsoft.Json.Serialization;
using UnityEngine;

#nullable enable
namespace CCEnvs.Unity.Items.Snapshots
{
    [Serializable]
    public class ItemContainerSnapshot : Snapshot<ItemContainer>
    {
        public Maybe<IItem> item { get; set; }
        public int itemCount { get; set; }
        public int capacity { get; set; }
        public bool isReadOnlyContainer { get; set; }
        public bool unlockCapacity { get; set; }

        public ItemContainerSnapshot()
        {
        }

        public ItemContainerSnapshot(ItemContainer target) : base(target)
        {
            item = target.Item;
            itemCount = target.ItemCount;
            capacity = target.Capacity;
            isReadOnlyContainer = target.IsReadOnlyContainer;
            unlockCapacity = target.UnlockCapacity;
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

        public override ItemContainer Restore(ItemContainer? target)
        {
            CC.Guard.IsNotNull(target, nameof(target));

            return new ItemContainer(
                item: item.Raw,
                count: itemCount,
                isReadOnlyContainer: isReadOnlyContainer)
            {
                UnlockCapacity = unlockCapacity,
                Capacity = capacity,
            };
        }
    }
}
