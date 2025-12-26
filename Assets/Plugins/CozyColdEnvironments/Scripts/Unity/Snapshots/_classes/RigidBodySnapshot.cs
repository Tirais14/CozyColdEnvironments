using CCEnvs.FuncLanguage;
using UnityEngine;

#nullable enable
namespace CCEnvs.Unity.Snapshots
{
    public class RigidBodySnapshot : ComponentSnapshot<Rigidbody>
    {
        public Vector3 LinearVelocity { get; private set; }
        public Vector3 AngularVelocity { get; private set; }

        public RigidBodySnapshot()
        {
        }

        public RigidBodySnapshot(Rigidbody target) : base(target)
        {
            LinearVelocity = target.linearVelocity;
            AngularVelocity = target.angularVelocity;
        }

        public override Maybe<Rigidbody> Restore(Rigidbody? target)
        {
            base.Restore(target);

            if (target == null)
                return null;

            target.linearVelocity = LinearVelocity;
            target.angularVelocity = AngularVelocity;

            return target;
        }
    }
}
