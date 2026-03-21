using CCEnvs.Unity.EditorSerialization;
using CCEnvs.Unity.Injections;
using R3;
using UnityEngine;

#nullable enable
namespace CCEnvs.Unity.D3
{
    public abstract class CameraRaycaster<TSelf> : ICameraRaycaster
        where TSelf : ICameraRaycaster
    {
        public const float MAX_DISTANCE_MIN = 0.01f;

        protected readonly ReactiveProperty<Collider?> objectCollider = new();

        [Header("Base Settings")]
        [Space(6f)]

        [SerializeField, Min(MAX_DISTANCE_MIN)]
        protected float maxDistance = 10f;

        [SerializeField]
        protected SerializedNullable<LayerMask> layerMask;

        [SerializeField, GetBySelf]
        protected new Camera camera;

        [SerializeField]
        protected QueryTriggerInteraction triggerInteraction = QueryTriggerInteraction.Ignore;

        public float MaxDistance => maxDistance;

        public LayerMask LayerMask => layerMask.Deserialized ?? Physics.AllLayers;

        public Camera Camera => camera;

        public QueryTriggerInteraction TriggerInteraction => triggerInteraction;

        public Collider? ObjectCollider => objectCollider.Value;

        public TSelf SetMaxDistance(float value)
        {
            maxDistance = Mathf.Max(value, MAX_DISTANCE_MIN);
            return this.CastTo<TSelf>();
        }

        public TSelf SetLayerMask(LayerMask? value)
        {
            layerMask = new SerializedNullable<LayerMask>(value);
            return this.CastTo<TSelf>();
        }

        public TSelf SetCamera(Camera value)
        {
            CC.Guard.IsNotNull(value);

            camera = value;

            return this.CastTo<TSelf>();
        }

        public TSelf SetTriggerInteraction(QueryTriggerInteraction value)
        {
            triggerInteraction = value;
            return this.CastTo<TSelf>();
        }

        public abstract bool TryRaycast(Vector2 screenPoint);

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
