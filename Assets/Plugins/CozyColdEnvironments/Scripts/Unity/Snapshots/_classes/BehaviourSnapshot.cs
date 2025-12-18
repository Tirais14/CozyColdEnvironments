using System;
using UnityEngine;

#nullable enable
namespace CCEnvs.Unity.Snapshots
{
    [Serializable]
    public class BehaviourSnapshot<T> : ComponentSnapshot<T>
        where T : Behaviour
    {
        [SerializeField]
        protected bool m_Enabled;

        public bool Enabled {
            get => m_Enabled; 
            protected set => m_Enabled = value;
        }

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
