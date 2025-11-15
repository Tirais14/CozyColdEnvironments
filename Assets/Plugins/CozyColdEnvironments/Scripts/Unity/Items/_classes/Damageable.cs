using CCEnvs.Attributes;
using CCEnvs.Unity.Components;
using System;
using UniRx;
using UnityEngine;

#nullable enable
#pragma warning disable S1244
namespace CCEnvs.Unity.Items
{
    public class Damageable : CCBehaviour, IDamageable
    {
        [OptionalField]
        [SerializeField]
        [Tooltip("Keep deafult to use MaxDurability on Start.")]
        private ReactiveProperty<float> durability = new();

        public float Durability {
            get => durability.Value;
            private set => durability.Value = value;
        }

        [field: SerializeField]
        public float MinDurability { get; set; }

        [field: SerializeField]
        public float MaxDurability { get; set; }

        protected override void Start()
        {
            if (Durability == default)
                Durability = MaxDurability;
        }

        public float DecreaseDurability(float value)
        {
            value = Mathf.Abs(value);

            var previous = Durability;

            Durability = Mathf.Clamp(Durability - value, MinDurability, MaxDurability);

            return previous - Durability;
        }
        public float DecreaseDurabilityBy(IDamager damager)
        {
            CC.Guard.IsNotNull(damager, nameof(damager));

            return DecreaseDurability(damager.DamageValue);
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
            return durability.Where(_ => StartPassed)
                .Pairwise()
                .Where(pair => pair.Current < pair.Previous)
                .Select(pair => new ChangedDurabilityEvent(pair.Previous, pair.Current));
        }

        public IObservable<ChangedDurabilityEvent> ObserveIncreaseDurability()
        {
            return durability.Where(_ => StartPassed)
                .Pairwise()
                .Where(pair => pair.Current > pair.Previous)
                .Select(pair => new ChangedDurabilityEvent(pair.Previous, pair.Current));
        }

        public IObservable<float> ObserveOnMaxDurability()
        {
            return durability.Where(_ => StartPassed)
                .Where(x => x.NearlyEquals(MaxDurability));
        }

        public IObservable<float> ObserveOnMinDurability()
        {
            return durability.Where(_ => StartPassed)
                .Where(x => x.NearlyEquals(MinDurability));
        }
    }
}
