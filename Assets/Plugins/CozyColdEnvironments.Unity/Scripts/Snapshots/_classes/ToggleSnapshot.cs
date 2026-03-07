using System;
using CCEnvs.Attributes.Serialization;
using UnityEngine;
using UnityEngine.UI;

#nullable enable
namespace CCEnvs.Unity.Snapshots
{
    [Serializable]
    public record ToggleSnapshot<T> : SelectableSnapshot<T>
        where T : Toggle
    {
        [SerializeField]
        protected bool? isOn;

        public bool? IsOn {
            get => isOn;
            set => isOn = value;
        }

        public ToggleSnapshot()
        {
        }

        public ToggleSnapshot(T target) : base(target)
        {
        }

        protected ToggleSnapshot(SelectableSnapshot<T> original) : base(original)
        {
        }

        protected ToggleSnapshot(UIBehaviourSnapshot<T> original) : base(original)
        {
        }

        protected override void OnRestore(ref T target)
        {
            base.OnRestore(ref target);

            if (isOn.HasValue)
                target.isOn = isOn.Value;
        }

        protected override void OnCapture(T target)
        {
            base.OnCapture(target);

            isOn = target.isOn;
        }

        protected override void OnReset()
        {
            base.OnReset();

            isOn = default;
        }
    }

    [Serializable]
    [SerializationDescriptor("ToggleSnapshot", "430233b6-9c52-4b81-9e04-c9553d0d288a")]
    public record ToggleSnapshot : ToggleSnapshot<Toggle>
    {
        public ToggleSnapshot()
        {
        }

        public ToggleSnapshot(Toggle target) : base(target)
        {
        }

        protected ToggleSnapshot(ToggleSnapshot<Toggle> original) : base(original)
        {
        }
    }
}
