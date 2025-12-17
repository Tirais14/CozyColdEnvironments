using CCEnvs.Json.Converters;
using CommunityToolkit.Diagnostics;
using System;
using System.Text.Json.Serialization;
using UnityEngine;

#nullable enable
namespace CCEnvs.Unity.Snapshots
{
    [Serializable]
    [JsonConverter(typeof(SnapshotConverter))]
    public class TransformSnapshot : ComponentSnapshot<Transform>
    {
        public Vector3Snapshot? Position { get; set; }
        public QuaternionSnapshot? Rotation { get; set; }

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
