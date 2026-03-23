using UnityEngine;

#nullable enable
namespace CCEnvs.Unity.D3
{

    public class CameraRaycasterRay : CameraRaycaster<CameraRaycasterRay>
    {
        public override bool TryRaycast(Ray ray)
        {
            if (Physics.Raycast(
                ray,
                out var hit,
                maxDistance,
                LayerMask,
                triggerInteraction
                ))
            {
                return OnRaycast(hit);
            }

            return OnRaycast(null);
        }
    }
}