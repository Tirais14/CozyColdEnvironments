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

            LoopHelper.Collect(value.transform.GetChild(0), (child, toProccess) =>
            {
                if (child.childCount > 1)
                {
                    Transform[] temp = child.GetChilds()[1..^1];
                    for (int i = 0; i < temp.Length; i++)
                        toProccess.Push(temp[i]);
                }

                return child.GetChild(0);
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

        public static Transform? FindParent<T>(this Transform transform, T name)
            where T : Enum
        {
            return transform.FindParent(name.ToString());
        }

        public static Transform? Find<T>(this Transform transform, T name)
            where T : Enum
        {
            return transform.Find(name.ToString());
        }

        public static bool TryFindParent(this Transform transform, string n, [NotNullWhen(true)] out Transform? result)
        {
            result = transform.FindParent(n);

            return result != null;
        }

        public static bool TryFindParent<T>(this Transform transform,
                                            T name,
                                            [NotNullWhen(true)] out Transform? result)
            where T : Enum
        {
            result = transform.FindParent(name);

            return result != null;
        }

        public static bool TryFind(this Transform transform,
                                   string n,
                                   [NotNullWhen(true)] out Transform? result)
        {
            result = transform.Find(n);

            return result != null;
        }

        public static bool TryFind<T>(this Transform transform,
                                      T name,
                                      [NotNullWhen(true)] out Transform? result)
            where T : Enum
        {
            result = transform.Find(name);

            return result != null;
        }
    }
}