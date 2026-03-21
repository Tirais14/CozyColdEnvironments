using CCEnvs.Unity.Components;
using R3;
using UnityEngine;

#nullable enable
namespace CCEnvs.Unity.D3
{
    public abstract class ICameraRaycaster : CCBehaviour
    {
        public abstract float MaxDistance { get; }

        public abstract LayerMask LayerMask { get; }

        public abstract Camera Camera { get; }

        public abstract QueryTriggerInteraction TriggerInteraction { get; }

        public abstract Collider? ObjectCollider { get; }

        public abstract RaycastHit? LastHit { get; }

        public abstract bool TryRaycast(Vector2 screenPoint);

        public abstract Observable<Collider?> ObserveObjectCollider();

        public abstract Observable<T?> ObserveObjectComponent<T>()
            where T : class;
    }
}
