using Newtonsoft.Json;
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
        [JsonProperty("bodySettings")]
        private GameObjectSettings bodySettings = null!;

        [JsonIgnore]
        public float Scale => bodySettings.Scale;

        [field: SerializeField]
        [JsonProperty("overrideBodyMesh")]
        public Mesh? BodyMesh { get; private set; } = null;

        [JsonIgnore]
        public bool HasBodyMesh => BodyMesh != null;

        [field: SerializeField]
        [JsonProperty("overrideBodyMaterials")]
        public Material[]? BodyMaterials { get; private set; } = null;

        [JsonIgnore]
        public bool HasBodyMaterials => BodyMaterials != null;

        public GameModelSettings(GameObjectSettings bodySettings,
                                 Mesh? bodyMesh,
                                 Material[]? bodyMaterials)
        {
            this.bodySettings = bodySettings;
            BodyMesh = bodyMesh;
            BodyMaterials = bodyMaterials;
        }

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
