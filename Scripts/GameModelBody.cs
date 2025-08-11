using UnityEngine;
using UTIRLib.Unity.Extensions;

#nullable enable
#pragma warning disable IDE0044
#pragma warning disable IDE1006
namespace UTIRLib
{
    [RequireComponent(typeof(MeshRenderer))]
    public class GameModelBody : MonoX
    {
        private Collider? m_Collider;

        [GetBySelf]
        new public MeshRenderer renderer { get; private set; } = null!;

#nullable disable
        new public Collider collider {
            get
            {
                if (m_Collider == null)
                    m_Collider = GetComponent<Collider>();

                return m_Collider;
            }
        }
#nullable enable
        public bool HasCollider => m_Collider != null;

        new public GameObject gameObject { get; private set; } = null!;
        new public Transform transform { get; private set; } = null!;

        protected override void OnAwake()
        {
            base.OnAwake();

            gameObject = base.gameObject;
            transform = base.transform;
        }

        public static GameModelBody Create(Transform? parent = null)
        {
            var go = new GameObject("Body", typeof(MeshRenderer));

            go.transform.parent = parent;
            go.tag = TirLib.Tags.GAME_OBJECT_OVERRIDE;

            return go.AddComponent<GameModelBody>();
        }
        public static GameModelBody Create(GameObject? parent = null)
        {
            return Create(parent.IfNotNull(x => x.transform));
        }
    }
}
