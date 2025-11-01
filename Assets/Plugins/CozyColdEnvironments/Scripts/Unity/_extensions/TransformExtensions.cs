using System;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;
using CCEnvs.Collections;
using System.Linq;
using CCEnvs.FuncLanguage;

#nullable enable

namespace CCEnvs.Unity
{
    public static class TransformExtensions
    {
        public static Vector3 Backward(this Transform value)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            return value.forward * -1;
        }

        public static Vector3 Left(this Transform value)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            return value.right * -1;
        }

        public static Vector3 Down(this Transform value)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            return value.up * -1;
        }

        /// <summary>
        /// Include nested childs
        /// </summary>
        public static ArraySegment<Transform> GetChilds(this Transform source, bool excludeSelf = false)
        {
            if (source.childCount == 0)
                return Array.Empty<Transform>();

            var results = source.GetComponentsInChildren<Transform>(includeInactive: true);

            if (excludeSelf)
            {
                if (results[0] == source)
                    return results.GetArraySegment(results.Length - 1, offset: 1);

                return results.Where(x => x != source).ToArray();
            }

            return results;
        }

        public static Transform? FindParent(this Transform transform, string n)
        {
            LoopFuse cyclePredicate = new(() => transform != null);
            do
            {
                transform = transform.parent;

                if (transform.name.Equals(n))
                    return transform;
            } while (cyclePredicate.Invoke());

            return null;
        }

        public static bool TryFindParent(this Transform transform, string n, [NotNullWhen(true)] out Transform? result)
        {
            result = transform.FindParent(n);

            return result != null;
        }

        public static bool TryFind(this Transform transform,
                                   string n,
                                   [NotNullWhen(true)] out Transform? result)
        {
            result = transform.Find(n);

            return result != null;
        }
    }
}