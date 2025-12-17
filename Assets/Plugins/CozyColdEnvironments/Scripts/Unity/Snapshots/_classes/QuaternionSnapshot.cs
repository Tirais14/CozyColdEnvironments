using CCEnvs.Snapshots;
using System;
using UnityEngine;

namespace CCEnvs.Unity.Snapshots
{
    [Serializable]
    public sealed class QuaternionSnapshot : Snapshot<Quaternion>
    {
        private float x { get; set; }
        public float y { get; set; }
        private float z { get; set; }
        private float w { get; set; }

        public QuaternionSnapshot()
        {
        }

        public QuaternionSnapshot(Quaternion target) : base(target)
        {
            x = target.x;
            y = target.y;
            z = target.z;
            w = target.w;
        }

        public override Quaternion Restore(Quaternion target)
        {
            return new Quaternion(x, y, z, w);
        }
    }
}
