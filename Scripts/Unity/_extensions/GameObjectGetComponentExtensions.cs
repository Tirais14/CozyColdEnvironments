using CCEnvs.Diagnostics;
using CCEnvs.Reflection;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using UnityEngine;

#nullable enable

namespace CCEnvs.Unity
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

            return result.IsNotNull();
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

            return result.IsNotNull();
        }
        public static bool TryGetComponentInParent<T>(this GameObject value,
            [NotNullWhen(true)] out T? result)

        {
            return value.TryGetComponentInParent(includeInactive: false, out result);
        }
    }
}

namespace CCEnvs.Unity.Extensions
{
    public static class GameObjectGetComponentExtensions
    {
        public static object? GetAssignedObject(this GameObject value,
                                                Type targetType)
        {
            return GetAssignedObjectsInternal(value,
                                              targetType,
                                              includeInactive: false,
                                              FindMode.Self,
                                              onlyFirst: true).FirstOrDefault();
        }

        public static T? GetAssignedObject<T>(this GameObject value)
        {
            return (T?)value.GetAssignedObject(typeof(T));
        }

        public static object? GetAssignedObjectInChildren(this GameObject value,
                                                          Type targetType,
                                                          bool includeInactive = false)
        {
            return GetAssignedObjectsInternal(value,
                                              targetType,
                                              includeInactive,
                                              FindMode.InChilds,
                                              onlyFirst: true).FirstOrDefault();
        }

        public static T? GetAssignedObjectInChildren<T>(this GameObject value,
                                                        bool includeInactive = false)
        {
            return (T?)value.GetAssignedObjectInChildren(typeof(T), includeInactive);
        }

        public static object? GetAssignedObjectInParent(this GameObject value,
                                                        Type targetType,
                                                        bool includeInactive = false)
        {
            return GetAssignedObjectsInternal(value,
                                              targetType,
                                              includeInactive,
                                              FindMode.InParents,
                                              onlyFirst: true).FirstOrDefault();
        }

        public static T? GetAssignedObjectInParent<T>(this GameObject value,
                                                      bool includeInactive = false)
        {
            return (T?)value.GetAssignedObjectInParent(typeof(T), includeInactive);
        }

        public static object[] GetAssignedObjects(this GameObject gameObject,
                                                  Type targetType)
        {
            return GetAssignedObjectsInternal(gameObject,
                                              targetType,
                                              includeInactive: false,
                                              FindMode.Self,
                                              onlyFirst: false);
        }

        public static T[] GetAssignedObjects<T>(this GameObject value)
        {
            return value.GetAssignedObjects(typeof(T))
                        .Cast<T>()
                        .ToArray();
        }

        public static object[] GetAssignedObjectsInChildren(this GameObject value,
                                                            Type targetType,
                                                            bool includeInactive = false)
        {
            return GetAssignedObjectsInternal(value,
                                              targetType,
                                              includeInactive,
                                              FindMode.InChilds,
                                              onlyFirst: false);
        }

        public static T[] GetAssignedObjectsInChildren<T>(this GameObject value,
                                                          bool includeInactive = false)
        {
            return value.GetAssignedObjectsInChildren(typeof(T))
                        .Cast<T>()
                        .ToArray();
        }

        public static object[] GetAssignedObjectsInParent(this GameObject value,
                                                          Type targetType,
                                                          bool includeInactive = false)
        {
            return GetAssignedObjectsInternal(value,
                                              targetType,
                                              includeInactive,
                                              FindMode.InParents,
                                              onlyFirst: false);
        }

        public static T[] GetAssignedObjectsInParent<T>(this GameObject value,
                                                        bool includeInactive = false)
        {
            return value.GetAssignedObjectsInParent(typeof(T))
                        .Cast<T>()
                        .ToArray();
        }

        public static bool TryGetAssignedObject(this GameObject value,
                                                Type targetType,
                                                [NotNullWhen(true)] out object? result)
        {
            result = GetAssignedObject(value, targetType);

            return result.IsNotNull();
        }

        public static bool TryGetAssignedObject<T>(this GameObject value,
                                                   [NotNullWhen(true)] out T? result)
        {
            result = GetAssignedObject<T>(value);

            return result.IsNotNull();
        }

        public static bool TryGetAssignedObjectInChildren(this GameObject value,
            Type targetType,
            bool includeInactive,
            [NotNullWhen(true)] out object? result)
        {
            result = value.GetAssignedObjectInChildren(targetType, includeInactive);

            return result.IsNotNull();
        }
        public static bool TryGetAssignedObjectInChildren(this GameObject value,
            Type targetType,
            [NotNullWhen(true)] out object? result)
        {
            return value.TryGetAssignedObjectInChildren(targetType,
                                                        includeInactive: false,
                                                        out result);
        }

        public static bool TryGetAssignedObjectInChildren<T>(this GameObject value,
            bool includeInactive,
            [NotNullWhen(true)] out T? result)
        {
            result = value.GetAssignedObjectInChildren<T>(includeInactive);

            return result.IsNotNull();
        }
        public static bool TryGetAssignedObjectInChildren<T>(this GameObject value,
            [NotNullWhen(true)] out T? result)
        {
            return value.TryGetAssignedObjectInChildren(includeInactive: false, out result);
        }

        public static bool TryGetAssignedObjectInParent(this GameObject value,
            Type targetType,
            bool includeInactive,
            [NotNullWhen(true)] out object? result)
        {
            result = value.GetAssignedObjectInParent(targetType, includeInactive);

            return result.IsNotNull();
        }
        public static bool TryGetAssignedObjectInParent(this GameObject value,
            Type targetType,
            [NotNullWhen(true)] out object? result)
        {
            return value.TryGetAssignedObjectInParent(targetType,
                                                      includeInactive: false,
                                                      out result);
        }

        public static bool TryGetAssignedObjectInParent<T>(this GameObject value,
            bool includeInactive,
            [NotNullWhen(true)] out T? result)
        {
            result = value.GetAssignedObjectInParent<T>(includeInactive);

            return result.IsNotNull();
        }
        public static bool TryGetAssignedObjectInParent<T>(this GameObject value,
            [NotNullWhen(true)] out T? result)
        {
            return value.TryGetAssignedObjectInParent(includeInactive: false, out result);
        }

        public static bool TryGetAssignedObjects(this GameObject value,
            Type targetType,
            out object[] results)
        {
            results = value.GetAssignedObjects(targetType);

            return results.Length > 0;
        }

        public static bool TryGetAssignedObjects<T>(this GameObject value,
                                                    out T[] results)
        {
            results = value.GetAssignedObjects<T>();

            return results.Length > 0;
        }

        public static bool TryGetAssignedObjectsInChildren(this GameObject value,
                                                           Type targetType,
                                                           bool includeInactive,
                                                           out object[] results)
        {
            results = value.GetAssignedObjectsInChildren(targetType, includeInactive);

            return results.Length > 0;
        }
        public static bool TryGetAssignedObjectsInChildren(this GameObject value,
                                                           Type targetType,
                                                           out object[] results)
        {
            return value.TryGetAssignedObjectsInChildren(targetType,
                                                         includeInactive: false,
                                                         out results);
        }

        public static bool TryGetAssignedObjectsInChildren<T>(this GameObject value,
                                                              bool includeInactive,
                                                              out T[] results)
        {
            results = value.GetAssignedObjectsInChildren<T>(includeInactive);

            return results.Length > 0;
        }
        public static bool TryGetAssignedObjectsInChildren<T>(this GameObject value,
                                                              out T[] results)
        {
            return value.TryGetAssignedObjectsInChildren(includeInactive: false,
                                                         out results);
        }

        public static bool TryGetAssignedObjectsInParent(this GameObject value,
                                                         Type targetType,
                                                         bool includeInactive,
                                                         out object[] results)
        {
            results = value.GetAssignedObjectsInParent(targetType, includeInactive);

            return results.Length > 0;
        }
        public static bool TryGetAssignedObjectsInParent(this GameObject value,
                                                         Type targetType,
                                                         out object[] results)
        {
            return value.TryGetAssignedObjectsInParent(targetType,
                                                       includeInactive: false,
                                                       out results);
        }

        public static bool TryGetAssignedObjectsInParent<T>(this GameObject value,
                                                            bool includeInactive,
                                                            out T[] results)
        {
            results = value.GetAssignedObjectsInParent<T>(includeInactive);

            return results.Length > 0;
        }
        public static bool TryGetAssignedObjectsInParent<T>(this GameObject value,
                                                            out T[] results)
        {
            return value.TryGetAssignedObjectsInParent(includeInactive: false, out results);
        }

        private enum FindMode
        {
            Self,
            InChilds,
            InParents
        }

        private static object[] GetAssignedObjectsInternal(this GameObject gameObject,
                                                           Type targetType,
                                                           bool includeInactive,
                                                           FindMode findMode,
                                                           bool onlyFirst)
        {
            if (targetType.IsType<Component>())
            {
                if (onlyFirst)
                {
                    return findMode switch
                    {
                        FindMode.Self => new object[] { gameObject.GetComponent(targetType) },
                        FindMode.InChilds => new object[] { gameObject.GetComponentInChildren(targetType) },
                        FindMode.InParents => new object[] { gameObject.GetComponentInParent(targetType) },
                        _ => throw new InvalidOperationException(findMode.ToString()),
                    };
                }
                else
                {
                    return findMode switch
                    {
                        FindMode.Self => gameObject.GetComponents(targetType),
                        FindMode.InChilds => gameObject.GetComponentsInChildren(targetType),
                        FindMode.InParents => gameObject.GetComponentsInParent(targetType),
                        _ => throw new InvalidOperationException(findMode.ToString()),
                    };
                }
            }

            Component[] gameObjectComponents = findMode switch
            {
                FindMode.InChilds => gameObject.GetComponentsInChildren(typeof(Component),
                                                                        includeInactive),

                FindMode.InParents => gameObject.GetComponentsInParent(typeof(Component),
                                                                       includeInactive),

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