using CCEnvs.Unity.Components;
using System;
using UniRx;
using UnityEngine;

#nullable enable
#pragma warning disable S1244
namespace CCEnvs.Unity.Items.Components
{
    public class HasDurability : CCBehaviour, IHasDurability
    {
        [SerializeField]
        private ReactiveProperty<float> durability = new();

        public float Durability {
            get => durability.Value;
            private set => durability.Value = value;
        }

        [field: SerializeField]
        public float MinDurability { get; set; }

        [field: SerializeField]
        public float MaxDurability { get; set; }

        public float DecreaseDurability(float value)
        {
            value = Mathf.Abs(value);

            var previous = Durability;

            Durability = Mathf.Clamp(Durability - value, MinDurability, MaxDurability);

            return previous - Durability;
        }

        public float IncreaseDurability(float value)
        {
            value = Mathf.Abs(value);

            var previous = Durability;

            Durability = Mathf.Clamp(Durability + value, MinDurability, MaxDurability);

            return Durability - previous;
        }

        public IObservable<ChangedDurabilityEvent> ObserveDecreaseDurability()
        {
            return durability.Pairwise()
                .Where(pair => pair.Current < pair.Previous)
                .Select(pair => new ChangedDurabilityEvent(pair.Previous, pair.Current));
        }

        public IObservable<ChangedDurabilityEvent> ObserveIncreaseDurability()
        {
            return durability.Pairwise()
                .Where(pair => pair.Current > pair.Previous)
                .Select(pair => new ChangedDurabilityEvent(pair.Previous, pair.Current));
        }
    }
}
