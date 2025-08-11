using UnityEngine;

#nullable enable
namespace UTIRLib.Unity.Special
{
    public static class ComponentExtensions
    {
        /// <returns>Overrided or default <see cref="Transform"/></returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        public static Transform GetTransform(this Component value)
        {
            if (value == null)
                throw new System.ArgumentNullException(nameof(value));

            return value.gameObject.GetTransform();
        }

        /// <exception cref="System.ArgumentNullException"></exception>
        public static Transform? GetOverridedTransform(this Component value,
                                                       bool throwIfNotFound = false)
        {
            if (value == null)
                throw new System.ArgumentNullException(nameof(value));

            return value.gameObject.GetOverridedTransform(throwIfNotFound);
        }

        /// <returns>Overrided or default <see cref="GameObject"/></returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        public static GameObject GetGameObject(this Component value)
        {
            if (value == null)
                throw new System.ArgumentNullException(nameof(value));

            return value.gameObject.GetGameObject();
        }

        /// <exception cref="System.ArgumentNullException"></exception>
        public static GameObject? GetOverridedGameObject(this Component value,
                                                         bool throwIfNotFound = false)
        {
            if (value == null)
                throw new System.ArgumentNullException(nameof(value));

            return value.gameObject.GetOverridedGameObject(throwIfNotFound);
        }
    }
}
