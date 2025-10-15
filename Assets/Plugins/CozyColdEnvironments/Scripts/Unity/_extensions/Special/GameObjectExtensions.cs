using CCEnvs.TypeMatching;
using System.Linq;
using UnityEngine;

#nullable enable
namespace CCEnvs.Unity.Special
{
    public static class GameObjectExtensions
    {
        /// <returns>Overrided or default <see cref="Transform"/></returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        public static Transform GetTransform(this GameObject value)
        {
            if (value == null)
                throw new System.ArgumentNullException(nameof(value));

            if (value.GetOverridedTransform().Is<Transform>(out Transform? result))
                return result;

            return value.transform;
        }

        /// <exception cref="System.ArgumentNullException"></exception>
        public static Transform? GetOverridedTransform(this GameObject value,
                                                       bool throwIfNotFound = false)
        {
            if (value == null)
                throw new System.ArgumentNullException(nameof(value));

            GameObject[] childs = value.GetAllChilds();

            if (childs.Select(x => x.transform)
                      .FirstOrDefault(x => x.CompareTag(UCC.Tags.TRANSFORM_OVERRIDE))
                .IsNot<Transform>(out Transform? result)
                &&
                throwIfNotFound
                )
                throw new System.Exception($"Not found {UCC.Tags.TRANSFORM_OVERRIDE} for {value.name}.");

            return result;
        }

        /// <returns>Overrided or default <see cref="GameObject"/></returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        public static GameObject GetGameObject(this GameObject value)
        {
            if (value == null)
                throw new System.ArgumentNullException(nameof(value));

            if (value.GetOverridedGameObject().Is<GameObject>(out GameObject? result))
                return result;

            return value;
        }

        /// <exception cref="System.ArgumentNullException"></exception>
        public static GameObject? GetOverridedGameObject(this GameObject value,
                                                         bool throwIfNotFound = false)
        {
            if (value == null)
                throw new System.ArgumentNullException(nameof(value));

            GameObject[] childs = value.GetAllChilds();

            if (childs.FirstOrDefault(x => x.CompareTag(UCC.Tags.TRANSFORM_OVERRIDE))
                .IsNot<GameObject>(out GameObject? result)
                &&
                throwIfNotFound
                )
                throw new System.Exception($"Not found {UCC.Tags.GAME_OBJECT_OVERRIDE} for {value.name}.");

            return result;
        }
    }
}
