using CCEnvs.FuncLanguage;
using CCEnvs.Unity.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;

#nullable enable
namespace CCEnvs
{
    public static class SceneSearch
    {
        public static Maybe<object> FindObjectByType(Type type,
            FindObjectsInactive findObjectsInactive = FindObjectsInactive.Exclude,
            FindObjectsSortMode sortMode = FindObjectsSortMode.None)
        {
            return FindObjectsByTypeInternal(type,
                                             findObjectsInactive,
                                             sortMode).FirstOrDefault();
        }

        public static Maybe<object> FindObjectByType(Type type,
            FindObjectsSortMode sortMode = FindObjectsSortMode.None)
        {
            return FindObjectsByTypeInternal(type,
                                             FindObjectsInactive.Exclude,
                                             sortMode).FirstOrDefault();
        }

        public static Maybe<T> FindObjectByType<T>(FindObjectsInactive findObjectsInactive,
            FindObjectsSortMode sortMode = FindObjectsSortMode.None)
        {
            return FindObjectsByTypeInternal(typeof(T),
                                             findObjectsInactive,
                                             sortMode).FirstOrDefault()
                                                      .AsOrDefault<T>();
        }

        public static Maybe<T> FindObjectByType<T>(
            FindObjectsSortMode sortMode = FindObjectsSortMode.None)
        {
            return FindObjectsByTypeInternal(typeof(T),
                                             FindObjectsInactive.Exclude,
                                             sortMode).FirstOrDefault()
                                                                                              .AsOrDefault<T>();
        }

        public static object[] FindObjectsByType(Type type,
            FindObjectsInactive findObjectsInactive = FindObjectsInactive.Exclude,
            FindObjectsSortMode sortMode = FindObjectsSortMode.None)
        {
            return FindObjectsByTypeInternal(type, findObjectsInactive, sortMode);
        }

        public static object[] FindObjectsByType(Type type,
            FindObjectsSortMode sortMode = FindObjectsSortMode.None)
        {
            return FindObjectsByTypeInternal(type, FindObjectsInactive.Exclude, sortMode);
        }

        public static T[] FindObjectsByType<T>(
            FindObjectsInactive findObjectsInactive = FindObjectsInactive.Exclude,
            FindObjectsSortMode sortMode = FindObjectsSortMode.None)
        {
            return FindObjectsByTypeInternal(typeof(T),
                                             findObjectsInactive,
                                             sortMode).Cast<T>()
                                                      .ToArray();
        }

        public static T[] FindObjectsByType<T>(
            FindObjectsSortMode sortMode = FindObjectsSortMode.None)
        {
            return FindObjectsByTypeInternal(typeof(T),
                                             FindObjectsInactive.Exclude,
                                             sortMode).Cast<T>()
                                                      .ToArray();
        }

        private static object[] FindObjectsByTypeInternal(Type type,
            FindObjectsInactive findObjectsInactive,
            FindObjectsSortMode sortMode,
            bool onlyFirst = false)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            GameObject[] gameObjects =
                Object.FindObjectsByType<GameObject>(findObjectsInactive, sortMode);

            int gameObjectsCount = gameObjects.Length;
            List<object> results = new();
            for (int i = 0; i < gameObjectsCount; i++)
            {
                if (gameObjects[i].TryGetAssignedObjects(type, out object[]? result))
                {
                    results.AddRange(result);
                    if (onlyFirst)
                        break;
                }
            }

            return results.ToArray();
        }
    }
}