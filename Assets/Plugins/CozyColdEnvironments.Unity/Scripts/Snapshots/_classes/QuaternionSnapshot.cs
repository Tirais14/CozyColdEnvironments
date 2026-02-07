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

        protected override void OnRestore(ref Quaternion target)
        {
            target.x = X;
            target.y = Y;
            target.z = Z;
            target.w = W;
        }
    }
}
