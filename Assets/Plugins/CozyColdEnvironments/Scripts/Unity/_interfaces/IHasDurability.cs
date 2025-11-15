#nullable enable
using CCEnvs.Unity.Items;
using System;

namespace CCEnvs.Unity
{
    public interface IHasDurability
    {
        float Durability { get; }
        float MinDurability { get; set; }
        float MaxDurability { get; set; }

        float DecreaseDurability(float value);

        float IncreaseDurability(float value);

        IObservable<ChangedDurabilityEvent> ObserveDecreaseDurability();

        IObservable<ChangedDurabilityEvent> ObserveIncreaseDurability();
    }
}
