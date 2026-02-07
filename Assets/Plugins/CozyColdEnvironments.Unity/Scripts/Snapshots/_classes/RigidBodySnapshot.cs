using Newtonsoft.Json;
using System;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;

#nullable enable
namespace CCEnvs.Unity.Snapshots
{
    [Serializable]
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

        protected override void OnRestore(ref Rigidbody target)
        {
            base.OnRestore(ref target);

            if (linearVelocity is not null && linearVelocity.TryRestore(default, out var lVelocity))
                target.linearVelocity = lVelocity;

            if (angularVelocity is not null && angularVelocity.TryRestore(default, out var aVelocity))
                target.angularVelocity = aVelocity;
        }
    }
}
