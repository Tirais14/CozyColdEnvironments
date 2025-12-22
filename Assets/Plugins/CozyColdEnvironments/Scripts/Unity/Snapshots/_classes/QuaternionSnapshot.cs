using CCEnvs.FuncLanguage;
using CCEnvs.Snapshots;
using System;
using UnityEngine;

namespace CCEnvs.Unity.Snapshots
{
    [Serializable]
    public sealed class QuaternionSnapshot : Snapshot<Quaternion>
    {
        [field: SerializeField]
        public float X { get; private set; }

        [field: SerializeField]
        public float Y { get; private set; }

        [field: SerializeField]
        public float Z { get; private set; }

        [field: SerializeField]
        public float W { get; private set; }

        public override bool IgnoreTarget => true;

        public QuaternionSnapshot()
        {
        }

        public QuaternionSnapshot(Quaternion target) : base(target)
        {
            X = target.x;
            Y = target.y;
            Z = target.z;
            W = target.w;
        }

        public override Maybe<Quaternion> Restore(Quaternion target)
        {
            return new Quaternion(X, Y, Z, W);
        }
    }
}
