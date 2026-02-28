using CCEnvs.Attributes.Serialization;
using System;
using UnityEngine.UI;

#nullable enable
namespace CCEnvs.Unity.Snapshots
{
    [Serializable]
    [TypeSerializationDescriptor("SelectableSnapshot", "62ec9842-d738-43ef-9246-6a66a15d746d")]
    public record SelectableSnapshot<T> : UIBehaviourSnapshot<T>
        where T : Selectable
    {
        public SelectableSnapshot()
        {
        }

        public SelectableSnapshot(T target) : base(target)
        {
        }

        protected SelectableSnapshot(UIBehaviourSnapshot<T> original) : base(original)
        {
        }
    }
}
