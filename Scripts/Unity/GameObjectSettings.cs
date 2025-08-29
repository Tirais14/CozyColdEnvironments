using Newtonsoft.Json;
using System;
using UnityEngine;

#nullable enable
namespace CCEnvs.Unity
{
    [Serializable]
    public sealed record GameObjectSettings 
        : 
        IGameObjectSettings, 
        IEquatable<GameObjectSettings>
    {
        [Min(1E-06f)]
        [field: SerializeField]
        [JsonProperty("scale")]
        public Vector3 Scale { get; set; } = Vector3.one;

        /// <exception cref="ArgumentNullException"></exception>
        public void ApplyTo(GameObject gameObject)
        {
            if (gameObject == null)
                throw new ArgumentNullException(nameof(gameObject));

            gameObject.transform.localScale = Scale;
        }
    }
}
