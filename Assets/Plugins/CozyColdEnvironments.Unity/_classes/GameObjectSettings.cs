using System;
using UnityEngine;

#nullable enable
namespace CCEnvs.Unity
{
    [Obsolete("Use snapshots instead")]
    [Serializable]
    public sealed record GameObjectSettings
        :
        IGameObjectSettings,
        IEquatable<GameObjectSettings>
    {
        public static GameObjectSettings Default { get; } = new();

        [Min(1E-06f)]
        [field: SerializeField]
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
