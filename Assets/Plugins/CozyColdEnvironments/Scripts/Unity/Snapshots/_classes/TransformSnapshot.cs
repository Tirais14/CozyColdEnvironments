using CCEnvs.Snapshots;
using System.Text.Json;
using System.Text.Json.Serialization;
using UnityEngine;

#nullable enable
namespace CCEnvs.Unity.Snaphots
{
    public class TransformSnapshot : Snapshot<Transform>
    {
        [SerializeField]
        [JsonInclude]
		[JsonPropertyName("position")]
        private Vector3 position;

        [SerializeField]
        [JsonInclude]
		[JsonPropertyName("rotation")]
        private Quaternion rotation;

        public TransformSnapshot()
        {
        }

        public TransformSnapshot(Transform target) : base(target)
        {
            position = target.position;
            rotation = target.rotation;
        }

        public override Transform Restore(Transform target)
        {
            CC.Guard.IsNotNull(target, nameof(target));

            target.SetPositionAndRotation(position, rotation);
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
