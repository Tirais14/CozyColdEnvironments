using System;
using CCEnvs.Attributes.Serialization;
using CCEnvs.Snapshots;
using UnityEngine;

#nullable enable
namespace CCEnvs.Unity.Snapshots
{
    [Serializable]
    [SerializationDescriptor("GameObjectSnapshot", "e6eab1e7-05e5-4e7e-a7a5-e5e004d8029c")]
    public sealed record GameObjectSnapshot : Snapshot<GameObject>
    {
        [field: SerializeField]
        public string? Name { get; private set; }

        [field: SerializeField]
        public string? Tag { get; private set; }

        [field: SerializeField]
        public int? Layer { get; private set; }

        [field: SerializeField]
        public bool? ActiveSelf { get; private set; } = true;

        [field: SerializeField]
        public TransformSnapshot? Transform { get; private set; }

        [field: SerializeField]
        public GameObjectExtraInfo? ExtraInfo { get; private set; }

        public GameObjectSnapshot()
        {
        }

        public GameObjectSnapshot(GameObject target) : base(target)
        {

        }

        public GameObjectSnapshot(Snapshot<GameObject> original) : base(original)
        {
        }

        protected override GameObject? CreateValue()
        {
            return ExtraInfo!.FindGameObject().GetValue();
        }

        protected override void OnRestore(ref GameObject target)
        {
            if (Name is not null)
                target.name = Name;

            if (Tag is not null)
                target.tag = Tag;

            if (Layer.HasValue)
                target.layer = Layer.Value;

            if (ActiveSelf.HasValue)
                target.SetActive(ActiveSelf.Value);

            Transform?.TryRestore(target.transform, out _);
        }

        protected override void OnCapture(GameObject target)
        {
            base.OnCapture(target);

            Name = target.name;
            Tag = target.tag;
            Layer = target.layer;
            ActiveSelf = target.activeSelf;
            Transform = new TransformSnapshot(target.transform);

            ExtraInfo = target.GetExtraInfo();
        }
    }
}
