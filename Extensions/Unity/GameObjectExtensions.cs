using System;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;
using UTIRLib.Extensions;

#nullable enable
namespace UTIRLib
{
    public static class GameObjectExtensions
    {
        public static GameObject? FindParent(this GameObject gameObject, string n)
        {
            if (gameObject.transform.TryFindParent(n, out Transform? child))
                return child.gameObject;

            return null;
        }

        public static GameObject? FindParent<T>(this GameObject gameObject, T name)
            where T : Enum
        {
            return gameObject.FindParent(name.ToString());
        }

        public static GameObject? Find(this GameObject gameObject, string n)
        {
            if (gameObject.transform.TryFind(n, out Transform? child))
                return child.gameObject;

            return null;
        }

        public static GameObject? Find<T>(this GameObject gameObject, T name)
            where T : Enum
        {
            return gameObject.Find(name.ToString());
        }

        public static bool TryFindParent(this GameObject gameobject,
                                         string n,
                                         [NotNullWhen(true)] out GameObject? result)
        {
            result = gameobject.FindParent(n);

            return result != null;
        }

        public static bool TryFindParent<T>(this GameObject gameobject,
                                            T name,
                                            [NotNullWhen(true)] out GameObject? result)
            where T : Enum
        {
            result = gameobject.FindParent(name);

            return result != null;
        }

        public static bool TryFind(this GameObject gameobject,
                                   string n,
                                   [NotNullWhen(true)] out GameObject? result)
        {
            result = gameobject.Find(n);

            return result != null;
        }

        public static bool TryFind<T>(this GameObject gameobject,
                                      T name,
                                      [NotNullWhen(true)] out GameObject? result)
            where T : Enum
        {
            result = gameobject.Find(name);

            return result != null;
        }
    }
}
