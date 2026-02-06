#nullable enable
using UnityEngine;

namespace CCEnvs.Unity
{
    public readonly struct CCRaycastHit
    {
        public readonly RaycastHit hit;
        public readonly bool hasHit;

        public CCRaycastHit(RaycastHit hit, bool hasHit)
        {
            this.hit = hit;
            this.hasHit = hasHit;
        }

        public static implicit operator RaycastHit(CCRaycastHit info)
        {
            return info.hit;
        }
        public static implicit operator bool(CCRaycastHit info)
        {
            return info.hasHit;
        }
    }
}
