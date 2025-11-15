#nullable enable
using System;

namespace CCEnvs.Unity.Items
{
    public interface IDamageable
    {
        float Durability { get; }
        float MinDurability { get; set; }
        float MaxDurability { get; set; }

        float DecreaseDurability(float value);
        float DecreaseDurability(IDamager damager);

        float IncreaseDurability(float value);

        IObservable<ChangedDurabilityEvent> ObserveDecreaseDurability();

        IObservable<ChangedDurabilityEvent> ObserveIncreaseDurability();

        IObservable<float> ObserveOnMinDurability();

        IObservable<float> ObserveOnMaxDurability();
    }
}
