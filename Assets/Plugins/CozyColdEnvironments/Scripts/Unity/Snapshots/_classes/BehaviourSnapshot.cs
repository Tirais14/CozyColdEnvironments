using CCEnvs.FuncLanguage;
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

        public override Maybe<T> Restore(T? target)
        {
            base.Restore(target);

            if (target.IsNull())
                return Maybe<T>.None;

            target.enabled = Enabled;

            return target;
        }
    }
}
