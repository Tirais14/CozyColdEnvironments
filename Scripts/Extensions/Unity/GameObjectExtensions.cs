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
