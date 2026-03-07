using System;
using CCEnvs.Attributes.Serialization;
using UnityEngine;

#nullable enable
namespace CCEnvs.Unity.Snapshots
{
    [Serializable]
    public record BehaviourSnapshot<T> : ComponentSnapshot<T>
        where T : Behaviour
    {
        [SerializeField]
        protected bool? m_Enabled;

        public bool? Enabled {
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
        }

        protected BehaviourSnapshot(ComponentSnapshot<T> original) : base(original)
        {
        }

        protected override void OnRestore(ref T target)
        {
            base.OnRestore(ref target);

            if (Enabled != null)
                target.enabled = Enabled.Value;
        }

        protected override void OnCapture(T target)
        {
            base.OnCapture(target);

            Enabled = target.enabled;
        }

        protected override void OnReset()
        {
            base.OnReset();

            Enabled = null;
        }
    }

    [Serializable]
    [SerializationDescriptor("BehaviourSnapshot", "affcf8e9-c4d7-4e78-8e56-0dd261fcb229")]
    public record BehaviourSnapshot : BehaviourSnapshot<Behaviour>
    {
        public BehaviourSnapshot()
        {
        }

        public BehaviourSnapshot(Behaviour target) : base(target)
        {
        }

        protected BehaviourSnapshot(BehaviourSnapshot<Behaviour> original) : base(original)
        {
        }
    }
}
