#nullable enable
using R3;

namespace CCEnvs.Unity.Items
{
    public interface IDamageable
    {
        float Durability { get; }
        float MinDurability { get; set; }
        float MaxDurability { get; set; }

        float DecreaseDurability(float value);

        float DecreaseDurabilityBy(IDamager damager);

        float IncreaseDurability(float value);

        Observable<ChangedDurabilityEvent> ObserveDecreaseDurability();

        Observable<ChangedDurabilityEvent> ObserveIncreaseDurability();

        Observable<float> ObserveOnMinDurability();

        Observable<float> ObserveOnMaxDurability();
    }
}
