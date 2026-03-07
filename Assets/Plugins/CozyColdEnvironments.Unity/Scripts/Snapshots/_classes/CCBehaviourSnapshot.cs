using System;
using CCEnvs.Attributes.Serialization;
using CCEnvs.Unity.Components;

#nullable enable
namespace CCEnvs.Unity.Snapshots
{
    [Serializable]
    public record CCBehaviourSnapshot<T> : MonoBehaviourSnapshot<T>
        where T : CCBehaviour
    {
        public CCBehaviourSnapshot()
        {
        }

        public CCBehaviourSnapshot(T target) : base(target)
        {
        }

        protected CCBehaviourSnapshot(MonoBehaviourSnapshot<T> original) : base(original)
        {
        }
    }

    [Serializable]
    [SerializationDescriptor("CCBehaviourSnapshot", "b4e35dc5-ce46-4ebf-8883-9b6c9e041851")]
    public record CCBehaviourSnapshot : CCBehaviourSnapshot<CCBehaviour>
    {
        public CCBehaviourSnapshot()
        {
        }

        public CCBehaviourSnapshot(CCBehaviour target) : base(target)
        {
        }

        protected CCBehaviourSnapshot(CCBehaviourSnapshot<CCBehaviour> original) : base(original)
        {
        }
    }
}
