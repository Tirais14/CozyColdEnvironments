using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using UnityEngine;
using UTIRLib.Diagnostics;

#nullable enable

namespace UTIRLib.Unity
{
    public static class GameObjectGetComponentExtensions
    {
        /// <exception cref="ArgumentNullException"></exception>
        public static bool TryGetComponentInChildren(this GameObject value,
            Type type,
            bool includeInactive,
            [NotNullWhen(true)] out Component? result)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            result = value.GetComponentInChildren(type, includeInactive);

            return result != null;
        }

        public static bool TryGetComponentInChildren(this GameObject value,
            Type type,
            [NotNullWhen(true)] out Component? result)
        {
            return value.TryGetComponentInChildren(type,
                                                   includeInactive: false,
                                                   out result);
        }

        public static bool TryGetComponentInChildren<T>(this GameObject value,
            bool includeInactive,
            [NotNullWhen(true)] out T? result)
        {
            result = value.GetComponentInChildren<T>(includeInactive);

            return result != null;
        }

        public static bool TryGetComponentInChildren<T>(this GameObject value,
            [NotNullWhen(true)] out T? result)
        {
            return value.TryGetComponentInChildren(includeInactive: false, out result);
        }

        /// <exception cref="ArgumentNullException"></exception>
        public static bool TryGetComponentInParent(this GameObject value,
            Type type,
            bool includeInactive,
            [NotNullWhen(true)] out Component? result)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            result = value.GetComponentInParent(type, includeInactive);

            return result != null;
        }
        public static bool TryGetComponentInParent(this GameObject value,
            Type type,
            [NotNullWhen(true)] out Component? result)
        {
            return value.TryGetComponentInParent(type,
                                                 includeInactive: false,
                                                 out result);
        }

        public static bool TryGetComponentInParent<T>(this GameObject value,
            bool includeInactive,
            [NotNullWhen(true)] out T? result)
        {
            result = value.GetComponentInParent<T>(includeInactive);

            return result != null;
        }
        public static bool TryGetComponentInParent<T>(this GameObject value,
            [NotNullWhen(true)] out T? result)

        {
            return value.TryGetComponentInParent(includeInactive: false, out result);
        }
    }
}

namespace UTIRLib.Unity.Extensions
{
    public static class GameObjectGetComponentExtensions
    {
        public static object? GetAssignedObject(this GameObject gameObject,
                                                Type targetType)
        {
            return GetAssignedObjectsInternal(gameObject,
                                              targetType,
                                              onlyFirst: true).FirstOrDefault();
        }

        public static T? GetAssignedObject<T>(this GameObject gameObject)
        {
            return (T?)GetAssignedObject(gameObject, typeof(T));
        }

        public static object? GetAssignedObjectInChildren(this GameObject gameObject,
                                                          Type targetType)
        {
            return GetAssignedObjectsInternal(gameObject,
                                              targetType,
                                              FindMode.InChilds,
                                              onlyFirst: true).FirstOrDefault();
        }

        public static T? GetAssignedObjectInChildren<T>(this GameObject gameObject)
        {
            return (T?)GetAssignedObject(gameObject,
                                         typeof(T));
        }

        public static object? GetAssignedObjectInParent(this GameObject gameObject,
                                                        Type targetType)
        {
            return GetAssignedObjectsInternal(gameObject,
                                              targetType,
                                              FindMode.InParents,
                                              onlyFirst: true).FirstOrDefault();
        }

        public static T? GetAssignedObjectInParent<T>(this GameObject gameObject)
        {
            return (T?)GetAssignedObject(gameObject, typeof(T));
        }

        public static object[] GetAssignedObjects(this GameObject gameObject,
                                                  Type targetType)
        {
            return GetAssignedObjectsInternal(gameObject, targetType);
        }

        public static T[] GetAssignedObjects<T>(this GameObject gameObject)
        {
            return GetAssignedObjects(gameObject,
                                      typeof(T)).Cast<T>()
                                                .ToArray();
        }

        public static object[] GetAssignedObjectsInChildren(this GameObject gameObject,
                                                            Type targetType)
        {
            return GetAssignedObjectsInternal(gameObject,
                                              targetType,
                                              FindMode.InChilds);
        }

        public static T[] GetAssignedObjectsInChildren<T>(this GameObject gameObject)
        {
            return GetAssignedObjects(gameObject,
                                      typeof(T)).Cast<T>()
                                                .ToArray();
        }

        public static object[] GetAssignedObjectsInParent(this GameObject gameObject,
                                                          Type targetType)
        {
            return GetAssignedObjectsInternal(gameObject,
                                              targetType,
                                              FindMode.InParents);
        }

        public static T[] GetAssignedObjectsInParent<T>(this GameObject gameObject)
        {
            return GetAssignedObjects(gameObject,
                                      typeof(T)).Cast<T>()
                                                .ToArray();
        }

        public static bool TryGetAssignedObject(this GameObject gameObject,
                                                Type targetType,
                                                [NotNullWhen(true)] out object? result)
        {
            result = GetAssignedObject(gameObject, targetType);

            return result.IsNotNull();
        }

        public static bool TryGetAssignedObject<T>(this GameObject gameObject,
                                                   [NotNullWhen(true)] out T? result)
        {
            result = GetAssignedObject<T>(gameObject);

            return result.IsNotNull();
        }

        public static bool TryGetAssignedObjectInChildren(this GameObject gameObject,
            Type targetType,
            [NotNullWhen(true)] out object? result)
        {
            result = GetAssignedObjectInChildren(gameObject,
                                                 targetType);

            return result.IsNotNull();
        }

        public static bool TryGetAssignedObjectInChildren<T>(this GameObject gameObject,
            [NotNullWhen(true)] out T? result)
        {
            result = GetAssignedObjectInChildren<T>(gameObject);

            return result.IsNotNull();
        }

        public static bool TryGetAssignedObjectInParent(this GameObject gameObject,
            Type targetType,
            [NotNullWhen(true)] out object? result)
        {
            result = GetAssignedObjectInParent(gameObject,
                                               targetType);

            return result.IsNotNull();
        }

        public static bool TryGetAssignedObjectInParent<T>(this GameObject gameObject,
            [NotNullWhen(true)] out T? result)
        {
            result = GetAssignedObjectInParent<T>(gameObject);

            return result.IsNotNull();
        }

        public static bool TryGetAssignedObjects(this GameObject gameObject,
            Type targetType,
            out object[] results)
        {
            results = GetAssignedObjects(gameObject, targetType);

            return results.Length > 0;
        }

        public static bool TryGetAssignedObjects<T>(this GameObject gameObject,
                                                    out T[] results)
        {
            results = GetAssignedObjects<T>(gameObject);

            return results.Length > 0;
        }

        public static bool TryGetAssignedObjectsInChildren(this GameObject gameObject,
                                                           Type targetType,
                                                           out object[] results)
        {
            results = GetAssignedObjectsInChildren(gameObject, targetType);

            return results.Length > 0;
        }

        public static bool TryGetAssignedObjectsInChildren<T>(this GameObject gameObject,
                                                              out T[] results)
        {
            results = GetAssignedObjectsInChildren<T>(gameObject);

            return results.Length > 0;
        }

        public static bool TryGetAssignedObjectsInParent(this GameObject gameObject,
                                                         Type targetType,
                                                         out object[] results)
        {
            results = GetAssignedObjectsInParent(gameObject, targetType);

            return results.Length > 0;
        }

        public static bool TryGetAssignedObjectsInParent<T>(this GameObject gameObject,
                                                            out T[] results)
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

        private static object[] GetAssignedObjectsInternal(this GameObject gameObject,
                                                           Type targetType,
                                                           FindMode findMode = FindMode.Self,
                                                           bool onlyFirst = false)
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
                        break;
                }
            }

            return results.ToArray();
        }
    }
}