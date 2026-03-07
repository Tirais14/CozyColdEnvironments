using System;
using CCEnvs.Attributes.Serialization;
using Newtonsoft.Json;
using UnityEngine;

#nullable enable
namespace CCEnvs.Unity.Snapshots
{
    [Serializable]
    public record RigidBodySnapshot<T> : ComponentSnapshot<T>
        where T : Rigidbody
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

        public RigidBodySnapshot(T target) : base(target)
        {
        }

        protected RigidBodySnapshot(ComponentSnapshot<T> original) : base(original)
        {
        }

        protected override void OnRestore(ref T target)
        {
            base.OnRestore(ref target);

            if (linearVelocity is not null && linearVelocity.TryRestore(default, out var lVelocity))
                target.linearVelocity = lVelocity;

            if (angularVelocity is not null && angularVelocity.TryRestore(default, out var aVelocity))
                target.angularVelocity = aVelocity;
        }

        protected override void OnCapture(T target)
        {
            base.OnCapture(target);

            LinearVelocity = new Vector3Snapshot(target.linearVelocity);
            AngularVelocity = new Vector3Snapshot(target.angularVelocity);
        }

        protected override void OnReset()
        {
            base.OnReset();

            LinearVelocity = null;
            AngularVelocity = null;
        }
    }

    [Serializable]
    [SerializationDescriptor("RigidBodySnapshot", "93662e04-2ce4-4ea1-8761-60efc7d50534")]
    public record RigidBodySnapshot : RigidBodySnapshot<Rigidbody>
    {
        public RigidBodySnapshot()
        {
        }

        public RigidBodySnapshot(Rigidbody target) : base(target)
        {
        }

        protected RigidBodySnapshot(RigidBodySnapshot<Rigidbody> original) : base(original)
        {
        }

        protected RigidBodySnapshot(ComponentSnapshot<Rigidbody> original) : base(original)
        {
        }
    }
}
