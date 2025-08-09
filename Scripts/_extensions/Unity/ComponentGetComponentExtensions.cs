using System;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;

#nullable enable
namespace UTIRLib.Unity
{
    public static class ComponentGetComponentExtensions
    {
        public static bool TryGetComponentInChildren(this Component value,
            Type type,
            bool includeInactive,
            [NotNullWhen(true)] out Component? result)
        {
            return value.gameObject.TryGetComponentInChildren(type,
                                                              includeInactive,
                                                              out result);
        }
        public static bool TryGetComponentInChildren(this Component value,
            Type type,
            [NotNullWhen(true)] out Component? result)
        {
            return value.TryGetComponentInChildren(type,
                                                   includeInactive: false,
                                                   out result);
        }

        public static bool TryGetComponentInChildren<T>(this Component value,
            bool includeInactive,
            [NotNullWhen(true)] out T? result)
        {
            return value.gameObject.TryGetComponentInChildren(includeInactive, out result);
        }
        public static bool TryGetComponentInChildren<T>(this Component value,
            [NotNullWhen(true)] out T? result)
        {
            return value.TryGetComponentInChildren(includeInactive: false, out result);
        }

        public static bool TryGetComponentInParent(this Component value,
            Type type,
            bool includeInactive,
            [NotNullWhen(true)] out Component? result)
        {
            return value.gameObject.TryGetComponentInParent(type, includeInactive, out result);
        }
        public static bool TryGetComponentInParent(this Component value,
            Type type,
            [NotNullWhen(true)] out Component? result)
        {
            return value.TryGetComponentInParent(type, includeInactive: false, out result);
        }

        public static bool TryGetComponentInParent<T>(this Component value,
            bool includeInactive,
            [NotNullWhen(true)] out T? result)
        {
            return value.gameObject.TryGetComponentInParent(includeInactive, out result);
        }
        public static bool TryGetComponentInParent<T>(this Component value,
            [NotNullWhen(true)] out T? result)
        {
            return value.TryGetComponentInParent(includeInactive: false, out result);
        }
    }
}

namespace UTIRLib.Unity.Extensions
{
    public static class ComponentGetComponentExtensions
    {
        public static object? GetAssignedObject(this Component value,
                                                Type targetType)
        {
            return value.gameObject.GetAssignedObject(targetType);
        }

        public static T? GetAssignedObject<T>(this Component value)
        {
            return value.gameObject.GetAssignedObject<T>();
        }

        public static object? GetAssignedObjectInChildren(this Component value,
                                                          Type targetType,
                                                          bool includeInactive = false)
        {
            return value.gameObject.GetAssignedObjectInChildren(targetType, includeInactive);
        }

        public static T? GetAssignedObjectInChildren<T>(this Component value,
                                                        bool includeInactive = false)
        {
            return value.gameObject.GetAssignedObjectInChildren<T>(includeInactive);
        }

        public static object? GetAssignedObjectInParent(this Component value,
                                                        Type targetType,
                                                        bool includeInactive = false)
        {
            return value.gameObject.GetAssignedObjectInParent(targetType, includeInactive);
        }

        public static T? GetAssignedObjectInParent<T>(this Component value,
                                                      bool includeInactive = false)
        {
            return value.gameObject.GetAssignedObjectInParent<T>(includeInactive);
        }

        public static object[] GetAssignedObjects(this Component value,
                                                  Type targetType)
        {
            return value.gameObject.GetAssignedObjects(targetType);
        }

        public static T[] GetAssignedObjects<T>(this Component value)
        {
            return value.gameObject.GetAssignedObjects<T>();
        }

        public static object[] GetAssignedObjectsInChildren(this Component value,
                                                            Type targetType,
                                                            bool includeInactive = false)
        {
            return value.gameObject.GetAssignedObjectsInChildren(targetType, includeInactive);
        }

        public static T[] GetAssignedObjectsInChildren<T>(this Component value,
                                                          bool includeInactive = false)
        {
            return value.gameObject.GetAssignedObjectsInChildren<T>(includeInactive);
        }

        public static object[] GetAssignedObjectsInParent(this Component value,
                                                          Type targetType,
                                                          bool includeInactive = false)
        {
            return value.gameObject.GetAssignedObjectsInParent(targetType, includeInactive);
        }

        public static T[] GetAssignedObjectsInParent<T>(this Component value,
                                                        bool includeInactive = false)
        {
            return value.gameObject.GetAssignedObjectsInParent<T>(includeInactive);
        }

        public static bool TryGetAssignedObject(this Component value,
                                                Type targetType,
                                                [NotNullWhen(true)] out object? result)
        {
            return value.gameObject.TryGetAssignedObject(targetType, out result);
        }

        public static bool TryGetAssignedObject<T>(this Component value,
                                                   [NotNullWhen(true)] out T? result)
        {
            return value.gameObject.TryGetAssignedObject(out result);
        }

        public static bool TryGetAssignedObjectInChildren(this Component value,
            Type targetType,
            bool includeInactive,
            [NotNullWhen(true)] out object? result)
        {
            return value.gameObject.TryGetAssignedObjectInChildren(targetType,
                                                                   includeInactive,
                                                                   out result);
        }
        public static bool TryGetAssignedObjectInChildren(this Component value,
            Type targetType,
            [NotNullWhen(true)] out object? result)
        {
            return value.gameObject.TryGetAssignedObjectInChildren(targetType,
                                                                   out result);
        }

        public static bool TryGetAssignedObjectInChildren<T>(this Component value,
            bool includeInactive,
            [NotNullWhen(true)] out T? result)
        {
            return value.gameObject.TryGetAssignedObjectInChildren(includeInactive,
                                                                   out result);
        }
        public static bool TryGetAssignedObjectInChildren<T>(this Component value,
            [NotNullWhen(true)] out T? result)
        {
            return value.gameObject.TryGetAssignedObjectInChildren(out result);
        }

        public static bool TryGetAssignedObjectInParent(this Component value,
            Type targetType,
            bool includeInactive,
            [NotNullWhen(true)] out object? result)
        {
            return value.gameObject.TryGetAssignedObjectInParent(targetType,
                                                                 includeInactive,
                                                                 out result);
        }
        public static bool TryGetAssignedObjectInParent(this Component value,
            Type targetType,
            [NotNullWhen(true)] out object? result)
        {
            return value.gameObject.TryGetAssignedObjectInParent(targetType,
                                                                 out result);
        }

        public static bool TryGetAssignedObjectInParent<T>(this Component value,
            bool includeInactive,
            [NotNullWhen(true)] out T? result)
        {
            return value.gameObject.TryGetAssignedObjectInParent(includeInactive,
                                                                 out result);
        }
        public static bool TryGetAssignedObjectInParent<T>(this Component value,
            [NotNullWhen(true)] out T? result)
        {
            return value.gameObject.TryGetAssignedObjectInParent(out result);
        }

        public static bool TryGetAssignedObjects(this Component value,
            Type targetType,
            out object[] results)
        {
            return value.gameObject.TryGetAssignedObjects(targetType, out results);
        }

        public static bool TryGetAssignedObjects<T>(this Component value,
                                                    out T[] results)
        {
            return value.gameObject.TryGetAssignedObjects(out results);
        }

        public static bool TryGetAssignedObjectsInChildren(this Component value,
                                                           Type targetType,
                                                           bool includeInactive,
                                                           out object[] results)
        {
            return value.gameObject.TryGetAssignedObjectsInChildren(targetType,
                                                                    includeInactive,
                                                                    out results);
        }
        public static bool TryGetAssignedObjectsInChildren(this Component value,
                                                           Type targetType,
                                                           out object[] results)
        {
            return value.gameObject.TryGetAssignedObjectsInChildren(targetType,
                                                                    out results);
        }


        public static bool TryGetAssignedObjectsInChildren<T>(this Component value,
                                                              bool includeInactive,
                                                              out T[] results)
        {
            return value.gameObject.TryGetAssignedObjectsInChildren(includeInactive,
                                                                    out results);
        }
        public static bool TryGetAssignedObjectsInChildren<T>(this Component value,
                                                              out T[] results)
        {
            return value.gameObject.TryGetAssignedObjectsInChildren(out results);
        }

        public static bool TryGetAssignedObjectsInParent(this Component value,
                                                         Type targetType,
                                                         bool includeInactive,
                                                         out object[] results)
        {
            return value.gameObject.TryGetAssignedObjectsInParent(targetType,
                                                                  includeInactive,
                                                                  out results);
        }
        public static bool TryGetAssignedObjectsInParent(this Component value,
                                                         Type targetType,
                                                         out object[] results)
        {
            return value.gameObject.TryGetAssignedObjectsInParent(targetType,
                                                                  out results);
        }

        public static bool TryGetAssignedObjectsInParent<T>(this Component value,
                                                            bool includeInactive,
                                                            out T[] results)
        {
            return value.gameObject.TryGetAssignedObjectsInParent(includeInactive,
                                                                  out results);
        }
        public static bool TryGetAssignedObjectsInParent<T>(this Component value,
                                                            out T[] results)
        {
            return value.gameObject.TryGetAssignedObjectsInParent(out results);
        }
    }
}
