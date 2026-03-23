using CCEnvs.Diagnostics;
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
        protected Camera _camera;

        [SerializeField]
        protected QueryTriggerInteraction triggerInteraction = QueryTriggerInteraction.Ignore;

        private readonly ReactiveCommand<Collider?> onRaycast = new();

        private Collider? objCollider;

        private RaycastHit? lastHit;

        public override float MaxDistance {
            get => maxDistance;
            set => SetMaxDistance(value);
        }

        public override LayerMask LayerMask {
            get => layerMask.Deserialized ?? Physics.AllLayers;
            set => SetLayerMask(value); 
        }

        public override Camera camera {
            get => _camera;
            set => SetCamera(value);
        }

        public override QueryTriggerInteraction TriggerInteraction {
            get => triggerInteraction;
            set => SetTriggerInteraction(value);
        }

        public override Collider? ObjectCollider => objCollider;

        public override RaycastHit? LastHit => lastHit;

        protected override void Start()
        {
            base.Start();

            if (_camera == null)
            {
                if (CCDebug.Instance.IsEnabled)
                    this.PrintWarning("Camera not setted. Will be used main camera");

                _camera = Camera.main;
            }

            CC.Guard.IsNotMissing(_camera);
        }

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

            _camera = value;

            return this.CastTo<TSelf>();
        }

        public TSelf SetTriggerInteraction(QueryTriggerInteraction value)
        {
            triggerInteraction = value;
            return this.CastTo<TSelf>();
        }

        public override Observable<Collider?> ObserveRaycast() => onRaycast;

        public override Observable<T?> ObserveObjectComponent<T>()
            where T : class
        {
            return onRaycast.Select(col =>
            {
                if (col == null)
                    return default;

                return col.transform.GetComponent<T>();
            });
        }

        protected bool OnRaycast(RaycastHit? hit)
        {
            if (hit == null || hit.Value.collider == null)
            {
                lastHit = null;
                objCollider = null;
            }
            else
            {
                lastHit = hit;
                objCollider = hit.Value.collider;

            }

            onRaycast.Execute(objCollider);
            return objCollider != null;
        }
    }
}
