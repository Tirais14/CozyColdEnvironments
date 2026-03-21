using CCEnvs.Unity.EditorSerialization;
using R3;
using UnityEngine;

#nullable enable
namespace CCEnvs.Unity.D3
{
    public abstract class CameraRaycaster<TSelf> : ICameraRaycaster
        where TSelf : ICameraRaycaster
    {
        public const float MAX_DISTANCE_MIN = 0.01f;

        [Header("Base Settings")]
        [Space(6f)]

        [SerializeField, Min(MAX_DISTANCE_MIN)]
        protected float maxDistance = 10f;

        [SerializeField]
        protected SerializedNullable<LayerMask> layerMask;

        [SerializeField]
        protected new Camera camera;

        [SerializeField]
        protected QueryTriggerInteraction triggerInteraction = QueryTriggerInteraction.Ignore;

        private readonly ReactiveProperty<Collider?> objectCollider = new();

        private RaycastHit? lastHit;

        public override float MaxDistance => maxDistance;

        public override LayerMask LayerMask => layerMask.Deserialized ?? Physics.AllLayers;

        public override Camera Camera => camera;

        public override QueryTriggerInteraction TriggerInteraction => triggerInteraction;

        public override Collider? ObjectCollider => objectCollider.Value;

        public override RaycastHit? LastHit => lastHit;

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

        public override abstract bool TryRaycast(Vector2 screenPoint);

        public override Observable<Collider?> ObserveObjectCollider() => objectCollider;

        public override Observable<T?> ObserveObjectComponent<T>()
            where T : class
        {
            return objectCollider.Select(col =>
            {
                if (col == null)
                    return default;

                return col.transform.GetComponent<T>();
            });
        }

        protected bool SetObject(RaycastHit? hit)
        {
            if (hit == null || hit.Value.collider == null)
            {
                lastHit = null;
                objectCollider.Value = null;
            }
            else
            {
                lastHit = hit;
                objectCollider.Value = hit.Value.collider;
            }

            return objectCollider.Value != null;
        }
    }
}
