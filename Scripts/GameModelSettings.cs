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

        [field: SerializeField]
        [JsonProperty("overrideBodyMesh")]
        public Mesh? BodyMesh { get; private set; } = null;

        [field: SerializeField]
        [JsonProperty("overrideBodyMaterials")]
        public Material[]? BodyMaterials { get; private set; } = null;

        [JsonIgnore]
        public float Scale => bodySettings.Scale;

        [JsonIgnore]
        public bool HasBodyMesh => BodyMesh != null;

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
        public void ApplyTo(GameModel gameModel)
        {
            if (gameModel == null)
                throw new ArgumentNullException(nameof(gameModel));

            if (HasBodyMesh)
                gameModel.Body.SetMesh(BodyMesh);

            if (HasBodyMaterials)
                gameModel.Body.SetMaterials(BodyMaterials!);

            bodySettings.ApplyTo(gameModel.gameObject);
        }

        void IGameObjectSettings.ApplyTo(GameObject gameObject)
        {
            bodySettings.ApplyTo(gameObject);
        }
    }
}
