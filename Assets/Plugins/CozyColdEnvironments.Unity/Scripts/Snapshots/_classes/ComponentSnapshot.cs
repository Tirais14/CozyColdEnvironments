using System;
using CCEnvs.Attributes.Serialization;
using CCEnvs.Snapshots;
using Newtonsoft.Json;
using UnityEngine;

#nullable enable
namespace CCEnvs.Unity.Snapshots
{
    [Serializable]
    public record ComponentSnapshot<T> : Snapshot<T>
        where T : Component
    {
        [Header(nameof(Component))]
        [Space(8)]

        [JsonIgnore]
        [SerializeField]
        protected GameObjectExtraInfo? extraInfo;

        public GameObjectExtraInfo? ExtraInfo {
            get => extraInfo;
            set => extraInfo = value;
        }

        public ComponentSnapshot()
        {
        }

        public ComponentSnapshot(T target)
            :
            base(target)
        {
        }

        protected ComponentSnapshot(Snapshot<T> original) : base(original)
        {
        }

        public override bool CanRestore(T? target)
        {
            return target != null || ExtraInfo is not null;
        }

        protected override void OnRestore(ref T target)
        {
        }

        protected override T? CreateValue()
        {
            return ExtraInfo!.FindGameObject(includeInactive: true)
                .Map(static go => go.Q().Component<T>().Raw)
                .GetValue();
        }

        protected override void OnCapture(T target)
        {
            base.OnCapture(target);

            ExtraInfo = target.GetExtraInfo();
        }

        protected override void OnReset()
        {
            base.OnReset();

            extraInfo = null;
        }
    }

    [Serializable]
    [SerializationDescriptor("ComponentSnapshot", "ad92c12a-fa33-4c14-bbdc-0fd040a7d85a")]
    public record ComponentSnapshot : ComponentSnapshot<Component>
    {
        public ComponentSnapshot()
        {
        }

        public ComponentSnapshot(Component target) : base(target)
        {
        }

        protected ComponentSnapshot(ComponentSnapshot<Component> original) : base(original)
        {
        }
    }
}
