using System;
using CCEnvs.Attributes.Serialization;
using UnityEngine;

#nullable enable
namespace CCEnvs.Unity.Snapshots
{
    [Serializable]
    public record MonoBehaviourSnapshot<T> : BehaviourSnapshot<T>
        where T : MonoBehaviour
    {
        public MonoBehaviourSnapshot()
        {
        }

        public MonoBehaviourSnapshot(T target) : base(target)
        {
        }
    }

    [Serializable]
    [SerializationDescriptor("MonoBehaviourSnapshot", "76287dcc-37e4-4995-8c32-552ebfc18426")]
    public record MonoBehaviourSnapshot : MonoBehaviourSnapshot<MonoBehaviour>
    {
        public MonoBehaviourSnapshot()
        {
        }

        public MonoBehaviourSnapshot(MonoBehaviour target) : base(target)
        {
        }

        protected MonoBehaviourSnapshot(MonoBehaviourSnapshot<MonoBehaviour> original) : base(original)
        {
        }
    }
}
