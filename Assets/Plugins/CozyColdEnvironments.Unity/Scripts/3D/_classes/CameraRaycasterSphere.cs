using UnityEngine;

#nullable enable
namespace CCEnvs.Unity.D3
{
    public sealed class CameraRaycasterSphere : CameraRaycaster<CameraRaycasterSphere>
    {
        public const float RADIUS_MIN = 0.01f;

        [SerializeField, Min(RADIUS_MIN)]
        private float radius = 1f;

        public float Radius => radius;

        public CameraRaycasterSphere SetRadius(float value)
        {
            radius = Mathf.Max(value, RADIUS_MIN);
            return this;
        }

        public override bool TryRaycast(Ray ray)
        {
            if (Physics.SphereCast(
                ray,
                radius,
                out var hit,
                maxDistance,
                LayerMask,
                TriggerInteraction
                ))
            {
                return OnRaycast(hit);
            }

            return OnRaycast(null);
        }
    }
}
