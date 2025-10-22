using CCEnvs.TypeMatching;
using CCEnvs.Unity.Components;
using CCEnvs.Unity.Diagnostics;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using UnityEngine;

#nullable enable
namespace CCEnvs.Unity
{
    public static class GameObjectExtensions
    {
        public static GameModel ToGameModel(this GameObject value,
            bool throwIfNotFound = true)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            var model = value.GetComponent<GameModel>();

            if (throwIfNotFound && model == null)
                throw new GameObjectNotFoundException(typeof(GameModel));

            return model;
        }

        /// <exception cref="System.ArgumentNullException"></exception>
        public static void ApplySettings(this GameObject value, GameObjectSettings settings)
        {
            if (value == null)
                throw new System.ArgumentNullException(nameof(value));
            if (settings == null)
                throw new System.ArgumentNullException(nameof(settings));

            settings.ApplyTo(value);
        }

        public static GameObject[] GetChilds(this GameObject value)
        {
            return value.transform.GetChilds().Select(x => x.gameObject).ToArray();
        }

        /// <summary>
        /// Include nested childs
        /// </summary>
        public static GameObject[] GetAllChilds(this GameObject value, bool includeFirst = true)
        {
            return value.transform.GetAllChilds(includeFirst).Select(x => x.gameObject).ToArray();
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