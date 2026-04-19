#if UNITY_EDITOR
using UnityEngine;

#nullable enable
namespace CCEnvs.Unity.Editr.Components
{
    public sealed class GizmosSphere : GizmosDrawer
    {
        [SerializeField]
        private float radius = 0.5f;

        [SerializeField]
        private bool isWire; 

        protected override void Draw()
        {
            if (isWire)
                Gizmos.DrawWireSphere(transform.position, radius);
            else
                Gizmos.DrawSphere(transform.position, radius);
        }
    }
}
#endif
