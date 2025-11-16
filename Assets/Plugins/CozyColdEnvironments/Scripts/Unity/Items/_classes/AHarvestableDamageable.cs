using CCEnvs.Unity.Injections;
using UniRx;
using UnityEngine;

#nullable enable
namespace CCEnvs.Unity.Items
{
    public abstract class AHarvestableDamageable<TConfig, TDamageable> : Harvestable<TConfig>
        where TConfig : IHarvestableConfig
        where TDamageable : Component, IDamageable
    {
        [GetBySelf]
        private TDamageable damageable = null!;

        protected override void Start()
        {
            base.Start();
            damageable.ObserveOnMinDurability()
                .SubscribeWithState(this, static (_, @this) => @this.OnDurablityOut())
                .AddTo(damageable);
        }

        protected override bool HarvestPredicate(IItem? item) => false;

        protected abstract void OnHarvested();

        private void OnDurablityOut()
        {
            OnHarvested();
            Destroy(gameObject);
        }
    }
}
