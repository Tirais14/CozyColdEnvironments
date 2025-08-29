#nullable enable
using UnityEngine;

namespace CCEnvs.Unity
{
    public readonly struct CCERaycastHit
    {
        public readonly RaycastHit hit;
        public readonly bool hasHit;

        public CCERaycastHit(RaycastHit hit, bool hasHit)
        {
            this.hit = hit;
            this.hasHit = hasHit;
        }

        public static implicit operator RaycastHit(CCERaycastHit info)
        {
            return info.hit;
        }
        public static implicit operator bool(CCERaycastHit info)
        {
            return info.hasHit;
        }
    }
}
