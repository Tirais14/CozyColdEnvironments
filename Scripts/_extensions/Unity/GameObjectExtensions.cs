using System.Diagnostics.CodeAnalysis;
using System.Linq;
using UnityEngine;
using UTIRLib.Unity.TypeMatching;

#nullable enable
namespace UTIRLib.Unity
{
    public static class GameObjectExtensions
    {
        public static GameObject[] GetChilds(this GameObject value) 
        {
            return value.transform.GetChilds().Select(x => x.gameObject).ToArray();
        }

        /// <summary>
        /// Include nested childs
        /// </summary>
        public static GameObject[] GetAllChilds(this GameObject value)
        {
            return value.transform.GetAllChilds().Select(x => x.gameObject).ToArray();
        }

        public static GameObject? FindParent(this GameObject value, string n)
        {
            if (value.transform.TryFindParent(n, out Transform? child))
                return child.gameObject;

            return null;
        }

        public static GameObject? Find(this GameObject value, string n)
        {
            if (value.transform.TryFind(n, out Transform? child))
                return child.gameObject;

            return null;
        }

        public static bool TryFindParent(this GameObject value,
                                         string n,
                                         [NotNullWhen(true)] out GameObject? result)
        {
            result = value.FindParent(n);

            return result != null;
        }

        public static bool TryFind(this GameObject value,
                                   string n,
                                   [NotNullWhen(true)] out GameObject? result)
        {
            result = value.Find(n);

            return result != null;
        }
    }
}
namespace UTIRLib.Unity.Special
{
    public static class GameObjectExtensions 
    {
        /// <returns>Overrided or default <see cref="Transform"/></returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        public static Transform GetTransform(this GameObject value)
        {
            if (value == null)
                throw new System.ArgumentNullException(nameof(value));

            if (value.GetOverridedTransform().Is<Transform>(out var result))
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
                      .FirstOrDefault(x => x.CompareTag(TirLib.Tags.TRANSFORM_OVERRIDE))
                .IsNot<Transform>(out var result)
                &&
                throwIfNotFound
                )
                throw new System.Exception($"Not found {TirLib.Tags.TRANSFORM_OVERRIDE} for {value.name}.");

            return result;
        }

        /// <returns>Overrided or default <see cref="GameObject"/></returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        public static GameObject GetGameObject(this GameObject value)
        {
            if (value == null)
                throw new System.ArgumentNullException(nameof(value));

            if (value.GetOverridedGameObject().Is<GameObject>(out var result))
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

            if (childs.FirstOrDefault(x => x.CompareTag(TirLib.Tags.TRANSFORM_OVERRIDE))
                .IsNot<GameObject>(out var result)
                &&
                throwIfNotFound
                )
                throw new System.Exception($"Not found {TirLib.Tags.GAME_OBJECT_OVERRIDE} for {value.name}.");

            return result;
        }
    }
}