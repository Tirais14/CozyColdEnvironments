using CCEnvs.FuncLanguage;
using CCEnvs.Snapshots;
using System;
using UnityEngine;

namespace CCEnvs.Unity.Snapshots
{
    [Serializable]
    public sealed class Vector2Snapshot : Snapshot<Vector2>
    {
        [field: SerializeField]
        public float X { get; set; }

        [field: SerializeField]
        public float Y { get; set; }

        public override bool IgnoreTarget => true;

        public Vector2Snapshot()
        {
        }

        public Vector2Snapshot(Vector2 target) : base(target)
        {
            X = target.x;
            Y = target.y;
        }

        public override Maybe<Vector2> Restore(Vector2 target)
        {
            return new Vector2(X, Y);
        }
    }
}
