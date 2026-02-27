using CCEnvs.Unity.Injections;
using Cysharp.Threading.Tasks;
using R3;
using UnityEngine;

#nullable enable
namespace CCEnvs.Unity.Items
{
    public abstract class AHarvestableDamageable<TItem, TDamageable> : Harvestable<TItem>
        where TItem : IHarvestableItem
        where TDamageable : Component, IDamageable
    {
        [GetBySelf]
        [SerializeField]
        protected TDamageable damageable = null!;

        protected override void Start()
        {
            base.Start();
            damageable.ObserveOnMinDurability()
                .Subscribe(this,
                static (_, @this) =>
                {
                    @this.OnDurablityOut();
                })
                .AddTo(damageable.GetCancellationTokenOnDestroy());
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
