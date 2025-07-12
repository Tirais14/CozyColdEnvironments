using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UTIRLib.Linq;
using UTIRLib.UExtensions;
using Object = UnityEngine.Object;

#nullable enable

namespace UTIRLib.Utils
{
    public static class UnityObjectHelper
    {
        public static object? FindObjectByType(Type type,
                                               FindObjectsInactive findObjectsInactive,
                                               FindObjectsSortMode sortMode)
        {
            return FindObjectsByTypeInternal(type, findObjectsInactive, sortMode).FirstOrDefault();
        }

        public static object? FindObjectByType(Type type, FindObjectsSortMode sortMode)
        {
            return FindObjectsByTypeInternal(type, FindObjectsInactive.Exclude, sortMode).FirstOrDefault();
        }

        public static T? FindObjectByType<T>(FindObjectsInactive findObjectsInactive,
                                             FindObjectsSortMode sortMode)
        {
            return FindObjectsByTypeInternal(typeof(T), findObjectsInactive, sortMode).FirstOrDefault()
                                                                                      .IsQ<T>();
        }

        public static T? FindObjectByType<T>(FindObjectsSortMode sortMode)
        {
            return FindObjectsByTypeInternal(typeof(T), FindObjectsInactive.Exclude, sortMode).FirstOrDefault()
                                                                                              .IsQ<T>();
        }

        public static object[] FindObjectsByType(Type type,
                                                 FindObjectsInactive findObjectsInactive,
                                                 FindObjectsSortMode sortMode)
        {
            return FindObjectsByTypeInternal(type, findObjectsInactive, sortMode);
        }

        public static object[] FindObjectsByType(Type type, FindObjectsSortMode sortMode)
        {
            return FindObjectsByTypeInternal(type, FindObjectsInactive.Exclude, sortMode);
        }

        public static T[] FindObjectsByType<T>(FindObjectsInactive findObjectsInactive, FindObjectsSortMode sortMode)
        {
            return FindObjectsByTypeInternal(typeof(T), findObjectsInactive, sortMode).Cast<T>().ToArray();
        }

        public static T[] FindObjectsByType<T>(FindObjectsSortMode sortMode)
        {
            return FindObjectsByTypeInternal(typeof(T), FindObjectsInactive.Exclude, sortMode).Cast<T>().ToArray();
        }

        private static object[] FindObjectsByTypeInternal(Type type, FindObjectsInactive findObjectsInactive,
            FindObjectsSortMode sortMode, bool onlyFirst = false)
        {
            GameObject[] gameObjects = Object.FindObjectsByType<GameObject>(findObjectsInactive, sortMode);

            int gameObjectsCount = gameObjects.Length;
            List<object> results = new();
            for (int i = 0; i < gameObjectsCount; i++)
            {
                if (gameObjects[i].TryGetAssignedObject(type, out var result))
                {
                    results.Add(result);
                    if (onlyFirst)
                    {
                        break;
                    }
                }
            }

            return results.ToArray();
        }
    }
}