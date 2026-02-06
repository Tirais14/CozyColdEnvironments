using CCEnvs.FuncLanguage;
using System;
using System.Diagnostics.CodeAnalysis;
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

        public override bool TryRestore(T? target, [NotNullWhen(true)] out T? restored)
        {
            if (!base.TryRestore(target, out restored))
                return false;

            target!.enabled = Enabled;

            restored = target;
            return true;
        }
    }
}
