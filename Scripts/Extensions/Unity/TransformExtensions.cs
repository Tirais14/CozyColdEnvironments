using System;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;

#nullable enable

namespace UTIRLib
{
    public static class TransformExtensions
    {
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