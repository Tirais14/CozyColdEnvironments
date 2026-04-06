using CCEnvs.Unity.Components;
using CCEnvs.Unity.Injections;
using UnityEngine;

#nullable enable
namespace CCEnvs.Unity.D3
{
    public sealed class ExternalSize : CCBehaviour
    {
        [SerializeField]
        private Vector3 _center;

        [SerializeField]
        private Vector3 _size;

        public Vector3 center {
            get => _center;
            set => _center = value;
        }

        public Vector3 size {
            get => _size;
            set => _size = value;
        }

        [field: GetBySelf(IsOptional = true)]
        public new Collider? collider { get; private set; }

        [field: GetBySelf(IsOptional = true)]
        public new MeshRenderer? renderer { get; private set; }

        public Bounds bounds => new(cTransform.TransformPoint(center), size);
        public Bounds localBounds => new(center, size);

#if UNITY_EDITOR
        [Header("Editor")]

        [SerializeField]
        private bool drawBounds = true;

        [SerializeField]
        private Color boundsColor = Color.lightCyan;

        [SerializeField]
        private bool recalculateCenterByColliderOrRenderer = true;

        [SerializeField]
        private bool recalculateSizeByColliderOrRenderer = true;

        private void OnDrawGizmos()
        {
            if (!drawBounds)
                return;

            if (recalculateCenterByColliderOrRenderer && !Application.isPlaying)
            {
                collider = GetComponent<Collider>();
                renderer = GetComponent<MeshRenderer>();

                TryRecalculateCenterByColliderOrRenderer();
            }

            if (recalculateSizeByColliderOrRenderer && !Application.isPlaying)
            {
                collider = GetComponent<Collider>();
                renderer = GetComponent<MeshRenderer>();

                TryRecalculateSizeByColliderOrRenderer();
            }

            var bounds = this.bounds;

            Gizmos.color = boundsColor;
            Gizmos.DrawSphere(bounds.center, 0.03f);
            Gizmos.DrawWireCube(bounds.center, bounds.size);
        }
#endif

        public bool TryRecalculateCenterByColliderOrRenderer()
        {
            if (collider != null)
            {
                center = cTransform.InverseTransformPoint(collider.bounds.center);
                return true;
            }
            else if (renderer != null)
            {
                center = cTransform.InverseTransformPoint(renderer.bounds.center);
                return true;
            }

            return false;
        }

        public bool TryRecalculateSizeByColliderOrRenderer()
        {
            if (collider != null)
            {
                size = collider.bounds.size;
                return true;
            }
            else if (renderer != null)
            {
                size = renderer.bounds.size;
                return true;
            }

            return false;
        }
    }
}
