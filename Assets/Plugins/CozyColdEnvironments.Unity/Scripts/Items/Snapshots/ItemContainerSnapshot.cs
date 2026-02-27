using System;
using CCEnvs.FuncLanguage;
using CCEnvs.Snapshots;
using UnityEngine;

#nullable enable
namespace CCEnvs.Unity.Items.Snapshots
{
    [Serializable]
    public class ItemContainerSnapshot : Snapshot<ItemContainer>
    {
        [SerializeField]
        protected int itemCount;

        [SerializeField]
        protected int capacity;

        [SerializeField]
        protected bool isReadOnlyContainer;

        [SerializeField]
        protected bool unlockCapacity;

        public Maybe<IItem> Item { get; set; }

        public int ItemCount {
            get => itemCount;
            set => itemCount = value;
        }

        public int Capacity {
            get => capacity;
            set => capacity = value;
        }

        public bool IsReadOnlyContainer {
            get => isReadOnlyContainer;
            set => isReadOnlyContainer = value;
        }

        public bool UnlockCapacity {
            get => unlockCapacity;
            set => unlockCapacity = value;
        }

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

        public override bool CanRestore(ItemContainer? target) => false;


        protected override ItemContainer? CreateValue()
        {
            return new ItemContainer(
                item: Item.Raw,
                count: itemCount,
                isReadOnlyContainer: isReadOnlyContainer)
            {
                UnlockCapacity = unlockCapacity,
                Capacity = Capacity
            };
        }

        protected override void OnRestore(ref ItemContainer target)
        {
        }
    }
}
