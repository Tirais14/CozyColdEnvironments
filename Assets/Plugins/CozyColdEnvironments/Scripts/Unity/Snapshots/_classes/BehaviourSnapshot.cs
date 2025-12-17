using System;
using UnityEngine;

#nullable enable
namespace CCEnvs.Unity.Snapshots
{
    [Serializable]
    public class BehaviourSnapshot<T> : ComponentSnapshot<T>
        where T : Behaviour
    {
        public bool Enabled { get; set; }

        public BehaviourSnapshot()
        {
        }

        public BehaviourSnapshot(T target)
            :
            base(target)
        {
            Enabled = target.enabled;
        }

        public override T Restore(T target)
        {
            base.Restore(target);
            CC.Guard.IsNotNullTarget(target);

            target.enabled = Enabled;
            return target;
        }
    }
}
