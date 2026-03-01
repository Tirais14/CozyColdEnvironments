using CCEnvs.Attributes.Serialization;
using System;
using UnityEngine.EventSystems;

#nullable enable
namespace CCEnvs.Unity.Snapshots
{
    [Serializable]
    public record UIBehaviourSnapshot<T> : MonoBehaviourSnapshot<T>
        where T : UIBehaviour
    {
        public UIBehaviourSnapshot()
        {
        }

        public UIBehaviourSnapshot(T target) : base(target)
        {
        }

        protected UIBehaviourSnapshot(MonoBehaviourSnapshot<T> original) : base(original)
        {
        }
    }

    [Serializable]
    [SerializationDescriptor("UIBehaviourSnapshot<>", "3fafb94c-22a5-4ef2-9ac3-3760846ecb23")]
    public record UIBehaviourSnapshot : UIBehaviourSnapshot<UIBehaviour>
    {
        public UIBehaviourSnapshot()
        {
        }

        public UIBehaviourSnapshot(UIBehaviour target) : base(target)
        {
        }

        protected UIBehaviourSnapshot(UIBehaviourSnapshot<UIBehaviour> original) : base(original)
        {
        }

        protected UIBehaviourSnapshot(MonoBehaviourSnapshot<UIBehaviour> original) : base(original)
        {
        }
    }
}
