using System;
using UnityEngine;

#nullable enable
#pragma warning disable S1244
#pragma warning disable S2328
namespace UTIRLib
{
    [Serializable]
    public sealed record GameObjectSettings 
        : 
        IGameObjectSettings, 
        IEquatable<GameObjectSettings>
    {
        [Min(1E-06f)]
        [SerializeField]
        private float scale;

        public float Scale => scale;

        public GameObjectSettings(float scale)
        {
            this.scale = scale;
        }

        public GameObjectSettings() : this(scale: 1f)
        {
        }

        /// <exception cref="ArgumentNullException"></exception>
        public void ApplyTo(GameObject gameObject)
        {
            if (gameObject == null)
                throw new ArgumentNullException(nameof(gameObject));
            if (scale == 1f)
                return;

            gameObject.transform.localScale *= scale;
        }
    }
}
