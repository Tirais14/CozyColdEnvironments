using UnityEngine;

#nullable enable
namespace CCEnvs.Unity
{
    public static class ComponentExtensions
    {
        //public static void ConfiguredDestroy(this Component source)
        //{
        //    CC.Validate.ArgumentNull(source, nameof(source));

        //    if (source.GetComponents<Component>().Any(x => !x.GetType().Namespace.StartsWith("Unity")))
        //        Object.Destroy(source);

        //    Object.Destroy(source.gameObject);
        //}
    }
}

namespace CCEnvs.Unity.Special
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
