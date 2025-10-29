using CCEnvs.Unity.Injections;
using System;
using UnityEngine;

#nullable enable
#pragma warning disable IDE0044
#pragma warning disable IDE1006
namespace CCEnvs.Unity.Components
{
    [RequireComponent(typeof(MeshRenderer))]
    public class GameModelBody : CCBehaviour
    {
        private Collider? m_Collider;
        private Rigidbody? m_RigidBody;

        [field: GetBySelf]
        public MeshRenderer meshRenderer { get; private set; } = null!;

        [field: GetBySelf]
        public MeshFilter meshFilter { get; private set; } = null!;

        public new Collider collider {
            get
            {
                if (m_Collider == null)
                    m_Collider = GetComponent<Collider>();

                return m_Collider;
            }
        }

        public bool HasCollider => collider != null;

        public new Rigidbody rigidbody {
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
            go.tag = UCC.Tags.GAME_OBJECT_OVERRIDE;

            return go.AddComponent<GameModelBody>();
        }
        public static GameModelBody Create(GameObject? parent = null)
        {
            return Create(parent != null ? parent.transform : null);
        }

        public GameModelBody SetMesh(Mesh? mesh)
        {
            meshFilter.mesh = mesh;

            return this;
        }

        /// <exception cref="ArgumentNullException"></exception>
        public GameModelBody SetMaterial(Material material)
        {
            if (material == null)
                throw new ArgumentNullException(nameof(material));

            meshRenderer.material = material;

            return this;
        }

        /// <exception cref="ArgumentNullException"></exception>
        public GameModelBody SetMaterials(params Material[] materials)
        {
            if (materials is null)
                throw new ArgumentNullException(nameof(materials));

            meshRenderer.materials = materials;

            return this;
        }
    }
}
