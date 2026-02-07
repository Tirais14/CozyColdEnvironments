using CCEnvs.Json.Converters;
using Newtonsoft.Json;
using System;
using UnityEngine;

#nullable enable
namespace CCEnvs.Unity.Snapshots
{
    [Serializable]
    [JsonConverter(typeof(SnapshotJsonConverter))]
    public class TransformSnapshot : ComponentSnapshot<Transform>
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

        public TransformSnapshot(Transform target) : base(target)
        {
            Position = new Vector3Snapshot(target.position);
            LocalPosition = new Vector3Snapshot(target.localPosition);
            Rotation = new QuaternionSnapshot(target.rotation);
        }

        protected override void OnRestore(ref Transform target)
        {
            base.OnRestore(ref target);

            if (Position is not null && Position.TryRestore(default, out var pos))
                target!.position = pos;

            if (LocalPosition is not null && LocalPosition.TryRestore(default, out var lPos))
                target!.localPosition = lPos;

            if (Rotation is not null && Rotation.TryRestore(default, out var rot))
                target!.rotation = rot;
        }
    }
}
