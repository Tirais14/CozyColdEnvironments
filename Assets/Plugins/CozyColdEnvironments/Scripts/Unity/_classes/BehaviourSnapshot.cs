using UnityEngine;

#nullable enable
namespace CCEnvs.Unity
{
    public class BehaviourSnapshot : ISnapshot
    {
        protected readonly Behaviour target;

        public virtual object Target => target;
        public bool Enabled { get; }

        public BehaviourSnapshot(Behaviour target)
        {
            CC.Guard.IsNotNull(target, nameof(target));

            this.target = target;
            Enabled = target.enabled;
        }

        public virtual void Restore()
        {
            if (target == null)
                return;

            target.enabled = Enabled;
        }
    }
}
