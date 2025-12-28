using CCEnvs.FuncLanguage;
using UnityEngine;

#nullable enable
namespace CCEnvs.Unity.Snapshots
{
    public class RigidBodySnapshot : ComponentSnapshot<Rigidbody>
    {
        public Vector3Snapshot LinearVelocity { get; private set; }
        public Vector3Snapshot AngularVelocity { get; private set; }

        public RigidBodySnapshot()
        {
        }

        public RigidBodySnapshot(Rigidbody target) : base(target)
        {
            LinearVelocity = new Vector3Snapshot(target.linearVelocity);
            AngularVelocity = new Vector3Snapshot(target.angularVelocity);
        }

        public override Maybe<Rigidbody> Restore(Rigidbody? target)
        {
            base.Restore(target);

            if (target == null)
                return null;

            target.linearVelocity = LinearVelocity.Restore().Raw;
            target.angularVelocity = AngularVelocity.Restore().Raw;

            return target;
        }
    }
}
