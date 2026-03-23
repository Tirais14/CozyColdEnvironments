using UnityEngine;

#nullable enable
namespace CCEnvs.Unity
{
    public static class BoundsExtensions
    {
        public static bool Contains(this Bounds outer, Bounds inner)
        {
            return
                outer.min.x <= inner.min.x && outer.max.x >= inner.max.x 
                &&
                outer.min.y <= inner.min.y && outer.max.y >= inner.max.y
                &&
                outer.min.z <= inner.min.z && outer.max.z >= inner.max.z;
        }
    }
}
