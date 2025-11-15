#nullable enable
using CCEnvs.Diagnostics;
using CCEnvs.Unity.Components;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CCEnvs.Unity.Items
{
    public class Harvestable<TConfig> : CCBehaviour, IHarvestable
        where TConfig : IHarvestableConfig
    {
        protected TConfig config { get; private set; } = default!;

        public void SetConfig(TConfig config)
        {
            CC.Guard.IsNotNull(config, nameof(config));

            this.config = config;
        }

        public IEnumerable<IItem> OutputItems => GetHarvestedItems()
            .Select(cnt => cnt.Item)
            .Where(item => item.IsSome)
            .Select(item => item.GetValueUnsafe());

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
                if (config.OutputItems.Value.IsEmpty())
                    return true;
                else
                    return false;
            }

            return config.RequiredItems.Value.Contains(item);
        }

        protected IItemContainer[] GetHarvestedItems()
        {
            return config.OutputItems.Value;
        }

        protected virtual void OnSetConfig() { }
    }
}
