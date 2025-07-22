using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using UnityEngine;

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

        public static GameObject? FindParent<T>(this GameObject value, T name)
            where T : Enum
        {
            return value.FindParent(name.ToString());
        }

        public static GameObject? Find(this GameObject value, string n)
        {
            if (value.transform.TryFind(n, out Transform? child))
                return child.gameObject;

            return null;
        }

        public static GameObject? Find<T>(this GameObject value, T name)
            where T : Enum
        {
            return value.Find(name.ToString());
        }

        public static bool TryFindParent(this GameObject value,
                                         string n,
                                         [NotNullWhen(true)] out GameObject? result)
        {
            result = value.FindParent(n);

            return result != null;
        }

        public static bool TryFindParent<T>(this GameObject value,
                                            T name,
                                            [NotNullWhen(true)] out GameObject? result)
            where T : Enum
        {
            result = value.FindParent(name);

            return result != null;
        }

        public static bool TryFind(this GameObject value,
                                   string n,
                                   [NotNullWhen(true)] out GameObject? result)
        {
            result = value.Find(n);

            return result != null;
        }

        public static bool TryFind<T>(this GameObject value,
                                      T name,
                                      [NotNullWhen(true)] out GameObject? result)
            where T : Enum
        {
            result = value.Find(name);

            return result != null;
        }
    }
}
