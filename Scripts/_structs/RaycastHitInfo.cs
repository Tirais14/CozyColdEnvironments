using UnityEngine;

#nullable enable
namespace UTIRLib
{
    public readonly struct RaycastHitInfo
    {
        public readonly RaycastHit hit;
        public readonly bool hasHit;

        public RaycastHitInfo(RaycastHit hit, bool hasHit)
        {
            this.hit = hit;
            this.hasHit = hasHit;
        }

        public static implicit operator RaycastHit(RaycastHitInfo info)
        {
            return info.hit;
        }
        public static implicit operator bool(RaycastHitInfo info)
        {
            return info.hasHit;
        }
    }
}
