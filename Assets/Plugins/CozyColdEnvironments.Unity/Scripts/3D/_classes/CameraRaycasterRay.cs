using UnityEngine;

#nullable enable
namespace CCEnvs.Unity.D3
{

    public class CameraRaycasterRay : CameraRaycaster<CameraRaycasterRay>
    {
        public override bool TryRaycast(Vector2 screenPoint)
        {
            if (Physics.Raycast(
                camera.ScreenPointToRay(screenPoint),
                out var hit,
                maxDistance,
                LayerMask,
                triggerInteraction
                ))
            {
                return SetObject(hit);
            }

            return SetObject(null);
        }
    }
}