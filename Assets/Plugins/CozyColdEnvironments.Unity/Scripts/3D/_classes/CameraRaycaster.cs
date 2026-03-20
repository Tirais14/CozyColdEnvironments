using CCEnvs.Unity.Components;
using CCEnvs.Unity.EditorSerialization;
using R3;
using UnityEngine;

#nullable enable
namespace CCEnvs.Unity.D3
{
    public sealed class CameraRaycaster : CCBehaviour
    {
        private readonly ReactiveProperty<Collider?> objectCollider = new();

        [SerializeField]
        private float maxDistance = 10f;

        [SerializeField]
        private SerializedNullable<LayerMask> layerMask;

        [SerializeField]
        private new Camera camera;

        [SerializeField]
        private QueryTriggerInteraction triggerInteraction = QueryTriggerInteraction.Ignore;

        public LayerMask LayerMask => layerMask.Deserialized ?? Physics.AllLayers;

        public Collider? ObjectCollider => objectCollider.Value;    

        public bool TryRaycast(Vector2 screenPoint)
        {
            if (Physics.Raycast(
                camera.ScreenPointToRay(screenPoint),
                out var hit,
                maxDistance,
                LayerMask,
                triggerInteraction
                ))
            {
                objectCollider.Value = hit.collider;
                return true;
            }

            objectCollider.Value = null;
            return false;
        }

        public Observable<Collider?> ObserveObjectCollider() => objectCollider;

        public Observable<T?> ObserveObjectComponent<T>()
        {
            return objectCollider.Select(col =>
            {
                if (col == null)
                    return default;

                return col.transform.GetComponent<T>();
            });
        }
    }
}
