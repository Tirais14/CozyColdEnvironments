using System;
using UnityEngine;
using UTIRLib.Unity;

#nullable enable
#pragma warning disable IDE0044
namespace UTIRLib
{
    [Serializable]
    public sealed record GameModelSettings 
        :
        IGameObjectSettings<GameModel>,
        IEquatable<GameModelSettings>
    {
        [SerializeField]
        private GameObjectSettings bodySettings = null!;

        public float Scale => bodySettings.Scale;

        [field: SerializeField]
        public Mesh? BodyMesh { get; private set; } = null;

        public bool HasBodyMesh => BodyMesh != null;

        [field: SerializeField]
        public Material[]? BodyMaterials { get; private set; } = null;

        public bool HasBodyMaterials => BodyMaterials != null;

        /// <exception cref="ArgumentNullException"></exception>
        public void ApplyTo(GameModel component)
        {
            if (component == null)
                throw new ArgumentNullException(nameof(component));

            component.Body.SetMesh(BodyMesh);

            if (HasBodyMaterials)
                component.Body.SetMaterials(BodyMaterials!);
        }

        void IGameObjectSettings.ApplyTo(GameObject gameObject)
        {
            bodySettings.ApplyTo(gameObject);
        }
    }
}
