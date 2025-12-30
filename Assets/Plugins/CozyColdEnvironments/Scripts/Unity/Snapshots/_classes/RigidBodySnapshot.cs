using Newtonsoft.Json;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;

#nullable enable
namespace CCEnvs.Unity.Snapshots
{
    public class RigidBodySnapshot : ComponentSnapshot<Rigidbody>
    {
        [JsonIgnore]
        [SerializeField]
        protected Vector3Snapshot? linearVelocity;

        [JsonIgnore]
        [SerializeField]
        protected Vector3Snapshot? angularVelocity;

        public Vector3Snapshot? LinearVelocity {
            get => linearVelocity; 
            protected set => linearVelocity = value;
        }
        public Vector3Snapshot? AngularVelocity {
            get => angularVelocity;
            protected set => angularVelocity = value;
        }

        public RigidBodySnapshot()
        {
        }

        public RigidBodySnapshot(Rigidbody target) : base(target)
        {
            LinearVelocity = new Vector3Snapshot(target.linearVelocity);
            AngularVelocity = new Vector3Snapshot(target.angularVelocity);
        }

        public override bool Restore(
            Rigidbody? target,
            [NotNullWhen(true)] out Rigidbody? restored)
        {
            if (!base.Restore(target, out restored))
                return false;

            if (LinearVelocity is not null && LinearVelocity.Restore(default, out var lVelocuty))
                target!.linearVelocity = lVelocuty;

            if (AngularVelocity is not null && AngularVelocity.Restore(default, out var aVelocity))
                target!.angularVelocity = aVelocity;

            restored = target!;
            return true;
        }
    }
}
