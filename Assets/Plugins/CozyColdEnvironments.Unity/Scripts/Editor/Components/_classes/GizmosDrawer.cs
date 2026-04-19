#if UNITY_EDITOR
using UnityEngine;

#nullable enable
namespace CCEnvs.Unity.Editr.Components
{
    public abstract class GizmosDrawer : MonoBehaviour
    {
        [SerializeField]
        protected Color color = Color.violet;

        [SerializeField]
        protected bool visibleIfSelected;

        private void OnDrawGizmos()
        {
            if (!visibleIfSelected)
            {
                Gizmos.color = color;
                Draw();
            }
        }

        private void OnDrawGizmosSelected()
        {
            if (visibleIfSelected)
            {
                Gizmos.color = color;
                Draw();
            }
        }

        protected abstract void Draw();
    }
}
#endif
