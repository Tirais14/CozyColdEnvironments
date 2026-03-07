using System;
using CCEnvs.Attributes.Serialization;
using UnityEngine.UI;

#nullable enable
namespace CCEnvs.Unity.Snapshots
{
    [Serializable]
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

    [Serializable]
    [SerializationDescriptor("SelectableSnapshot", "62ec9842-d738-43ef-9246-6a66a15d746d")]
    public record SelectableSnapshot : SelectableSnapshot<Selectable>
    {
        public SelectableSnapshot()
        {
        }

        public SelectableSnapshot(Selectable target) : base(target)
        {
        }

        protected SelectableSnapshot(SelectableSnapshot<Selectable> original) : base(original)
        {
        }

        protected SelectableSnapshot(UIBehaviourSnapshot<Selectable> original) : base(original)
        {
        }
    }
}
