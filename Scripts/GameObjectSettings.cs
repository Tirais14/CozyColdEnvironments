using System;
using UnityEngine;

#nullable enable
#pragma warning disable S1244
#pragma warning disable S2328
namespace UTIRLib
{
    [Serializable]
    public sealed class GameObjectSettings : IEquatable<GameObjectSettings>
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

        public bool Equals(GameObjectSettings? other)
        {
            if (other is null)
                return false;

            return scale == other.scale;
        }
        public override bool Equals(object obj)
        {
            return obj is GameObjectSettings typed && Equals(typed);
        }

        public override int GetHashCode()
        {
            return scale.GetHashCode();
        }

        public static bool operator ==(GameObjectSettings? left, GameObjectSettings? right)
        {
            if (left is null && right is null)
                return true;
            if (left is null)
                return false;

            return left.Equals(right);
        }

        public static bool operator !=(GameObjectSettings? left, GameObjectSettings? right)
        {
            if (left is null && right is null)
                return false;
            if (left is null)
                return true;

            return !left.Equals(right);
        }
    }
}
