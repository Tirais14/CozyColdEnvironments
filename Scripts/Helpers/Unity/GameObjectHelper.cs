using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using UnityEngine;
using UTIRLib.Diagnostics;

#nullable enable

namespace UTIRLib.Utils
{
    public static class GameObjectHelper
    {
        public static object? GetAssignedObject(GameObject gameObject, Type targetType) =>
            GetAssignedObjectsInternal(gameObject, targetType, onlyFirst: true).FirstOrDefault();

        public static T? GetAssignedObject<T>(GameObject gameObject) =>
            (T?)GetAssignedObject(gameObject, typeof(T));

        public static object? GetAssignedObjectInChildren(GameObject gameObject, Type targetType) =>
            GetAssignedObjectsInternal(gameObject, targetType, FindMode.InChilds, onlyFirst: true).FirstOrDefault();

        public static T? GetAssignedObjectInChildren<T>(GameObject gameObject) =>
            (T?)GetAssignedObject(gameObject, typeof(T));

        public static object? GetAssignedObjectInParent(GameObject gameObject, Type targetType) =>
            GetAssignedObjectsInternal(gameObject, targetType, FindMode.InParents, onlyFirst: true).FirstOrDefault();

        public static T? GetAssignedObjectInParent<T>(GameObject gameObject) =>
            (T?)GetAssignedObject(gameObject, typeof(T));

        public static object[] GetAssignedObjects(GameObject gameObject, Type targetType) =>
            GetAssignedObjectsInternal(gameObject, targetType);

        public static T[] GetAssignedObjects<T>(GameObject gameObject) =>
            GetAssignedObjects(gameObject, typeof(T)).Cast<T>().ToArray();

        public static object[] GetAssignedObjectsInChildren(GameObject gameObject, Type targetType) =>
            GetAssignedObjectsInternal(gameObject, targetType, FindMode.InChilds);

        public static T[] GetAssignedObjectsInChildren<T>(GameObject gameObject) =>
            GetAssignedObjects(gameObject, typeof(T)).Cast<T>().ToArray();

        public static object[] GetAssignedObjectsInParent(GameObject gameObject, Type targetType) =>
            GetAssignedObjectsInternal(gameObject, targetType, FindMode.InParents);

        public static T[] GetAssignedObjectsInParent<T>(GameObject gameObject) =>
            GetAssignedObjects(gameObject, typeof(T)).Cast<T>().ToArray();

        public static bool TryGetAssignedObject(GameObject gameObject, Type targetType,
            [NotNullWhen(true)] out object? result)
        {
            result = GetAssignedObject(gameObject, targetType);

            return result.IsNotNull();
        }

        public static bool TryGetAssignedObject<T>(GameObject gameObject, [NotNullWhen(true)] out T? result)
        {
            result = GetAssignedObject<T>(gameObject);

            return result.IsNotNull();
        }

        public static bool TryGetAssignedObjectInChildren(GameObject gameObject, Type targetType,
            [NotNullWhen(true)] out object? result)
        {
            result = GetAssignedObjectInChildren(gameObject, targetType);

            return result.IsNotNull();
        }

        public static bool TryGetAssignedObjectInChildren<T>(GameObject gameObject, [NotNullWhen(true)] out T? result)
        {
            result = GetAssignedObjectInChildren<T>(gameObject);

            return result.IsNotNull();
        }

        public static bool TryGetAssignedObjectInParent(GameObject gameObject, Type targetType,
            [NotNullWhen(true)] out object? result)
        {
            result = GetAssignedObjectInParent(gameObject, targetType);

            return result.IsNotNull();
        }

        public static bool TryGetAssignedObjectInParent<T>(GameObject gameObject, [NotNullWhen(true)] out T? result)
        {
            result = GetAssignedObjectInParent<T>(gameObject);

            return result.IsNotNull();
        }

        public static bool TryGetAssignedObjects(GameObject gameObject, Type targetType, out object[] results)
        {
            results = GetAssignedObjects(gameObject, targetType);

            return results.Length > 0;
        }

        public static bool TryGetAssignedObjects<T>(GameObject gameObject, out T[] results)
        {
            results = GetAssignedObjects<T>(gameObject);

            return results.Length > 0;
        }

        public static bool TryGetAssignedObjectsInChildren(GameObject gameObject, Type targetType, out object[] results)
        {
            results = GetAssignedObjectsInChildren(gameObject, targetType);

            return results.Length > 0;
        }

        public static bool TryGetAssignedObjectsInChildren<T>(GameObject gameObject, out T[] results)
        {
            results = GetAssignedObjectsInChildren<T>(gameObject);

            return results.Length > 0;
        }

        public static bool TryGetAssignedObjectsInParent(GameObject gameObject, Type targetType, out object[] results)
        {
            results = GetAssignedObjectsInParent(gameObject, targetType);

            return results.Length > 0;
        }

        public static bool TryGetAssignedObjectsInParent<T>(GameObject gameObject, out T[] results)
        {
            results = GetAssignedObjectsInParent<T>(gameObject);

            return results.Length > 0;
        }

        private enum FindMode
        {
            Self,
            InChilds,
            InParents
        }

        private static object[] GetAssignedObjectsInternal(GameObject gameObject, Type targetType,
            FindMode findMode = FindMode.Self, bool onlyFirst = false)
        {
            Component[] gameObjectComponents = findMode switch {
                FindMode.InChilds => gameObject.GetComponentsInChildren(typeof(Component)),
                FindMode.InParents => gameObject.GetComponentsInParent(typeof(Component)),
                _ => gameObject.GetComponents(typeof(Component)),
            };

            List<object> results = new();
            int gameObjectComponentsCount = gameObjectComponents.Length;
            for (int i = 0; i < gameObjectComponentsCount; i++)
            {
                if (targetType.IsInstanceOfType(gameObjectComponents[i]))
                {
                    results.Add(gameObjectComponents[i]);
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