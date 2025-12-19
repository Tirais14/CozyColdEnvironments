using CCEnvs.Json.Converters;
using CommunityToolkit.Diagnostics;
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
        protected QuaternionSnapshot? m_Rotation;

        public Vector3Snapshot? Position {
            get => m_Position;
            protected set => m_Position = value;
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
            Rotation = new QuaternionSnapshot(target.rotation);
        }

        public override Transform Restore(Transform? target)
        {
            base.Restore(target);

            CC.Guard.IsNotNull(target, nameof(target));
            Guard.IsNotNull(Position);
            Guard.IsNotNull(Rotation);

            target.SetPositionAndRotation(Position.Restore(), Rotation.Restore());
            return target;
        }
    }
}
