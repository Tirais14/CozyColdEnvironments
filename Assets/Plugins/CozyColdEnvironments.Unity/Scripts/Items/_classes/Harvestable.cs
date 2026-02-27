#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using CCEnvs.Collections;
using CCEnvs.Unity.Components;
using UnityEngine;

namespace CCEnvs.Unity.Items
{
    public class Harvestable<TItem> : CCBehaviour, IHarvestable
        where TItem : IHarvestableItem
    {
        [field: SerializeField]
        protected TItem item;

        public void SetItem(TItem item)
        {
            CC.Guard.IsNotNull(item, nameof(item));

            this.item = item;
        }

        public IEnumerable<IItem> OutputItems {
            get
            {
                return GetHarvestedItems().Select(cnt => cnt.Item)
                    .Where(item => item.IsSome)
                    .Select(item => item.GetValueUnsafe()
                    );
            }
        }

        public bool CanHarvestedBy(IItem? item) => HarvestPredicate(item);

        public bool CanHarvested() => HarvestPredicate(item: null);

        public bool TryHarvest(out IItemContainer[] results)
        {
            if (!CanHarvested())
            {
                results = Array.Empty<IItemContainer>();
                return false;
            }

            results = GetHarvestedItems();
            return true;
        }

        public bool TryHarvestBy(IItem item, out IItemContainer[] results)
        {
            CC.Guard.IsNotNull(item, nameof(item));

            if (!CanHarvestedBy(item))
            {
                results = Array.Empty<IItemContainer>();
                return false;
            }

            results = GetHarvestedItems();
            return true;
        }

        protected virtual bool HarvestPredicate(IItem? item)
        {
            if (item.IsNull())
            {
                if (this.item.OutputItems.IsEmpty())
                    return true;
                else
                    return false;
            }

            return this.item.RequiredItems.Contains(item);
        }

        protected IItemContainer[] GetHarvestedItems()
        {
            return item.OutputItems.ToArray();
        }

        protected virtual void OnSetItem() { }
    }
}
