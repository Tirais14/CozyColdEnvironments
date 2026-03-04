using System;
using CCEnvs.Attributes.Serialization;
using CCEnvs.Unity.Snapshots;

#nullable enable
namespace Core.Playables
{
    [Serializable, SerializationDescriptor("PlayerSnapshot", "2663f353-f544-4b48-bd81-2b04e622527a")]
    public record PlayerSnapshot : CCBehaviourSnapshot<Player>
    {
        public float? Health { get; set; }
        public float? Drunkness { get; set; }

        public PlayerSnapshot()
        {
        }

        public PlayerSnapshot(Player target) : base(target)
        {
        }

        protected PlayerSnapshot(CCBehaviourSnapshot<Player> original) : base(original)
        {
        }

        protected PlayerSnapshot(MonoBehaviourSnapshot<Player> original) : base(original)
        {
        }

        protected override void OnReset()
        {
            base.OnReset();

            Health = default;
            Drunkness = default;
        }

        protected override void OnCapture(Player target)
        {
            base.OnCapture(target);

            Health = target.Health;
            Drunkness = target.Drunkness;
        }

        protected override void OnRestore(ref Player target)
        {
            base.OnRestore(ref target);

            if (Health.HasValue)
                target.Health = Health.Value;

            if (Drunkness.HasValue)
                target.Drunkness = Drunkness.Value;
        }
    }
}
