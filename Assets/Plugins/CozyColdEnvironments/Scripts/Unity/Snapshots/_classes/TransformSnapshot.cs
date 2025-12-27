using CCEnvs.FuncLanguage;
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
        [SerializeField]
        protected Vector3Snapshot? m_Position;

        [SerializeField]
        protected Vector3Snapshot? m_LocalPosition;   

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

        public override Maybe<Transform> Restore(Transform? target)
        {
            base.Restore(target);

            if (target == null)
                return Maybe<Transform>.None;

            if (Position is not null)
                target.position = Position.Restore().Raw;

            if (LocalPosition is not null) 
                target.localPosition = LocalPosition.Restore().Raw;

            if (Rotation is not null)
                target.rotation = Rotation.Restore().Raw;

            return target;
        }
    }
}
