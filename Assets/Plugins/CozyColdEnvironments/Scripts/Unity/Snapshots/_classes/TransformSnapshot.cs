using CCEnvs.Json.Converters;
using CCEnvs.Snapshots;
using CommunityToolkit.Diagnostics;
using System;
using System.Text.Json.Serialization;
using UnityEngine;

#nullable enable
namespace CCEnvs.Unity.Snaphots
{
    [Serializable]
    [JsonConverter(typeof(SnapshotConverter))]
    public class TransformSnapshot : ComponentSnapshot<Transform>
    {
        [JsonInclude]
        [SerializeField]
        protected Vector3Snapshot? position;

        [JsonInclude]
        [SerializeField]
        protected QuaternionSnapshot? rotation;

        public TransformSnapshot()
        {
        }

        public TransformSnapshot(Transform target) : base(target)
        {
            position = new Vector3Snapshot(target.position);
            rotation = new QuaternionSnapshot(target.rotation);
        }

        [JsonConstructor]
        public TransformSnapshot(Vector3Snapshot? position, QuaternionSnapshot? rotation)
        {
            this.position = position;
            this.rotation = rotation;
        }

        public override Transform Restore(Transform? target)
        {
            base.Restore(target);

            CC.Guard.IsNotNull(target, nameof(target));
            Guard.IsNotNull(position);
            Guard.IsNotNull(rotation);

            target.SetPositionAndRotation(position.Restore(), rotation.Restore());
            return target;
        }
    }

    public static class TransformSnapshotExtensions
    {
        public static TransformSnapshot CaptureState(this Transform source)
        {
            CC.Guard.IsNotNull(source, nameof(source));
            return new TransformSnapshot(source);
        }
    }
}
