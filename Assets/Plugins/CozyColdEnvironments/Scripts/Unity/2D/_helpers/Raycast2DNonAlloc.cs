using System;
using System.Collections.Generic;
using UnityEngine;

#nullable enable
namespace CCEnvs.Unity._2D
{
    public static class Raycast2DNonAlloc
    {
        public const int CAPACITY_DEFAULT = 10;

        public static RaycastHit2D[] RaycastHits { get; private set; } = new RaycastHit2D[CAPACITY_DEFAULT];
        public static List<RaycastHit2D> RaycastHitList { get; } = new(CAPACITY_DEFAULT);
        public static int RaycastHitsLimit {
            get => RaycastHits.Length;
            set
            {
                if (value <= 0)
                    value = 1;

                RaycastHits = new RaycastHit2D[value];
            }
        }
    }
}
