using CCEnvs.Unity.Components;
using R3;
using UnityEngine;

#nullable enable
namespace CCEnvs.Unity.D3
{
    public abstract class ICameraRaycaster : CCBehaviour
    {
        public abstract float MaxDistance { get; set; }

        public abstract LayerMask LayerMask { get; set; }

        public new abstract Camera camera { get; set; }

        public abstract QueryTriggerInteraction TriggerInteraction { get; set; }

        public abstract Collider? ObjectCollider { get; }

        public abstract RaycastHit? LastHit { get; }

        public abstract bool TryRaycast(Ray ray);
        public virtual bool TryRaycast(Vector2 screenPoint)
        {
            return TryRaycast(camera.ScreenPointToRay(screenPoint));
        }
        public virtual bool TryRaycast()
        {
            var ray = new Ray(
                 camera.transform.position,
                 camera.transform.forward
                );

            return TryRaycast(ray);
        }

        public abstract Observable<Collider?> ObserveRaycast();

        public abstract Observable<T?> ObserveObjectComponent<T>()
            where T : class;
    }
}
