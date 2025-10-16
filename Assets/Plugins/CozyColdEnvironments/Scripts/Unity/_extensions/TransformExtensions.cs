using System;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;

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

        public static Transform[] GetChilds(this Transform value)
        {
            if (value.childCount == 0)
                return Array.Empty<Transform>();

            int childCount = value.childCount;
            var childs = new Transform[childCount];
            for (int i = 0; i < childCount; i++)
                childs[i] = value.GetChild(i);

            return childs;
        }

        /// <summary>
        /// Include nested childs
        /// </summary>
        public static Transform[] GetAllChilds(this Transform value, bool includeFirst = true)
        {
            if (value.childCount == 0)
                return Array.Empty<Transform>();

            if (includeFirst)
                return Collector.Collect(value, (x) => x.GetChilds())
                             .ToArray();

            return Collector.Collect(value.GetChild(0), (x) => x.GetChilds())
                             .ToArray();
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