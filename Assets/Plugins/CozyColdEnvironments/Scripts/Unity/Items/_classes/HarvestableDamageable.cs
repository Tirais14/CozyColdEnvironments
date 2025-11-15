using CCEnvs.Unity.Injections;
using UniRx;
using UnityEngine;

namespace CCEnvs.Unity.Items
{
    public class HarvestableDamageable<TConfig, TDamageable> : Harvestable<TConfig>
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

        private void OnDurablityOut()
        {


            Destroy(gameObject);
        }
    }
}
