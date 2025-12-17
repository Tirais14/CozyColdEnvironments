using CCEnvs.FuncLanguage;
using CCEnvs.Snapshots;
using System;
using System.Text.Json.Serialization;
using UnityEngine;

#nullable enable
namespace CCEnvs.Unity.Items.Snapshots
{
    [Serializable]
    public class ItemContainerSnapshot : Snapshot<ItemContainer>
    {
        [JsonInclude]
        [SerializeField]
        protected Maybe<IItem> item;

        [JsonInclude]
        [SerializeField]
        protected int itemCount;

        [JsonInclude]
        [SerializeField]
        protected int capacity;

        [JsonInclude]
        [SerializeField]
        protected bool isReadOnlyContainer;

        [JsonInclude]
        [SerializeField]
        protected bool unlockCapacity;

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

        [JsonConstructor]
        public ItemContainerSnapshot(Maybe<IItem> item, int itemCount, int capacity, bool isReadOnlyContainer, bool unlockCapacity)
        {
            this.item = item;
            this.itemCount = itemCount;
            this.capacity = capacity;
            this.isReadOnlyContainer = isReadOnlyContainer;
            this.unlockCapacity = unlockCapacity;
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

    public static class ItemContainerSnapshotExtensions
    {
        public static ItemContainerSnapshot CaptureState(this ItemContainer source)
        {
            return new ItemContainerSnapshot(source); 
        }
    }
}
