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
        private Rigidbody? m_RigidBody;

        [GetBySelf]
        public MeshRenderer meshRenderer { get; private set; } = null!;

        [GetBySelf]
        public MeshFilter meshFilter { get; private set; } = null!;

        new public Collider collider {
            get
            {
                if (m_Collider == null)
                    m_Collider = GetComponent<Collider>();

                return m_Collider;
            }
        }

        public bool HasCollider => collider != null;

        new public Rigidbody rigidbody {
            get
            {
                if (m_RigidBody == null)
                    m_RigidBody = GetComponent<Rigidbody>();

                return m_RigidBody;
            }
        }

        public bool HasRigidBody => rigidbody != null;

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
