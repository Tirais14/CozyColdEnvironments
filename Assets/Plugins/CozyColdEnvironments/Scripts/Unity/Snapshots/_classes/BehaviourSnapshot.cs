using System;
using UnityEngine;

#nullable enable
namespace CCEnvs.Unity.Snaphots
{
    [Serializable]
    public class BehaviourSnapshot : ComponentSnapshot
    {
        [SerializeField]
        protected bool m_Enabled;

        public bool Enabled => m_Enabled;

        public BehaviourSnapshot()
        {
        }

        public BehaviourSnapshot(Behaviour target)
            :
            base(target)
        {
            m_Enabled = target.enabled;
        }

        public override void Restore(object target)
        {
            var beh = ValidateTarget<Behaviour>(target);

            beh.enabled = Enabled;
        }
    }
}
