using UnityEngine;

#nullable enable
namespace CCEnvs.Unity
{
    public static class BoundsExtensions
    {
        public static bool Contains(this Bounds outer, Bounds inner, float epsilon = 0.001f)
        {
            return
                outer.min.x - epsilon <= inner.min.x && outer.max.x + epsilon >= inner.max.x
                &&
                outer.min.y - epsilon <= inner.min.y && outer.max.y + epsilon >= inner.max.y
                &&
                outer.min.z - epsilon <= inner.min.z && outer.max.z + epsilon >= inner.max.z;
        }
    }
}
