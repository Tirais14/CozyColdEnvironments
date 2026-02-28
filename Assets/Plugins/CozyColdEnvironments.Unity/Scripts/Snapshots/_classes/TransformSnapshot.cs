using CCEnvs.Attributes.Serialization;
using CCEnvs.Snapshots;
using Newtonsoft.Json;
using System;
using UnityEngine;

#nullable enable
namespace CCEnvs.Unity.Snapshots
{
    [Serializable]
    public record TransformSnapshot<T> : ComponentSnapshot<T>
        where T : Transform
    {
        [JsonIgnore]
        [SerializeField]
        protected Vector3Snapshot? m_Position;

        [JsonIgnore]
        [SerializeField]
        protected Vector3Snapshot? m_LocalPosition;

        [JsonIgnore]
        [SerializeField]
        protected QuaternionSnapshot? m_Rotation;

        public Vector3Snapshot? Position {
            get => m_Position;
            protected set => m_Position = value;
        }

        public Vector3Snapshot? LocalPosition {
            get => m_LocalPosition;
            protected set => m_LocalPosition = value;
        }

        public QuaternionSnapshot? Rotation {
            get => m_Rotation;
            protected set => m_Rotation = value;
        }

        public TransformSnapshot()
        {
        }

        public TransformSnapshot(T target) : base(target)
        {
        }

        protected TransformSnapshot(ComponentSnapshot<T> original) : base(original)
        {
        }

        protected TransformSnapshot(Snapshot<T> original) : base(original)
        {
        }

        protected override void OnRestore(ref T target)
        {
            base.OnRestore(ref target);

            if (Position is not null && Position.TryRestore(default, out var pos))
                target!.position = pos;

            if (LocalPosition is not null && LocalPosition.TryRestore(default, out var lPos))
                target!.localPosition = lPos;

            if (Rotation is not null && Rotation.TryRestore(default, out var rot))
                target!.rotation = rot;
        }

        protected override void OnCapture(T target)
        {
            base.OnCapture(target);

            Position = new Vector3Snapshot(target.position);
            LocalPosition = new Vector3Snapshot(target.localPosition);
            Rotation = new QuaternionSnapshot(target.rotation);
        }

        protected override void OnReset()
        {
            base.OnReset();

            Position = default;
            LocalPosition = default;
            Rotation = default;
        }
    }

    [Serializable]
    [TypeSerializationDescriptor("TransformSnapshot", "0a16247f-5bbd-4968-9cd3-706be8b12247")]
    public record TransformSnapshot : TransformSnapshot<Transform>
    {
        public TransformSnapshot()
        {
        }

        public TransformSnapshot(Transform target) : base(target)
        {
        }

        protected TransformSnapshot(TransformSnapshot<Transform> original) : base(original)
        {
        }

        protected TransformSnapshot(ComponentSnapshot<Transform> original) : base(original)
        {
        }

        protected TransformSnapshot(Snapshot<Transform> original) : base(original)
        {
        }
    }
}
