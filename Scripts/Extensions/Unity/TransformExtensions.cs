using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;

#nullable enable

namespace UTIRLib.Unity
{
    public static class TransformExtensions
    {
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
        public static Transform[] GetAllChilds(this Transform value)
        {
            if (value.childCount == 0)
                return Array.Empty<Transform>();

            var childs = new List<Transform>(value.childCount);

            LoopHelper.Collect(value.transform.GetChild(0), (child) =>
            {
                if (child.childCount == 0)
                    return LoopIteration<Transform[]>.Void();

                return LoopIteration.Complete(child.GetChilds());
            });

            return childs.ToArray();
        }

        public static Transform? FindParent(this Transform transform, string n)
        {
            LoopPredicate cyclePredicate = new(() => transform != null);
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