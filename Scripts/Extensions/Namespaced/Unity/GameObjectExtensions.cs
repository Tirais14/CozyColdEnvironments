using System;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;
using UTIRLib.Utils;

#nullable enable

namespace UTIRLib.UExtensions
{
    public static class GameObjectExtensions
    {
        public static object? GetAssignedObject(this GameObject gameObject, Type targetType)
        {
            return GameObjectHelper.GetAssignedObject(gameObject, targetType);
        }

        public static T? GetAssignedObject<T>(this GameObject gameObject)
        {
            return GameObjectHelper.GetAssignedObject<T>(gameObject);
        }

        public static object? GetAssignedObjectInChildren(this GameObject gameObject, Type targetType)
        {
            return GameObjectHelper.GetAssignedObjectInChildren(gameObject, targetType);
        }

        public static T? GetAssignedObjectInChildren<T>(this GameObject gameObject)
        {
            return GameObjectHelper.GetAssignedObjectInChildren<T>(gameObject);
        }

        public static object? GetAssignedObjectInParent(this GameObject gameObject, Type targetType)
        {
            return GameObjectHelper.GetAssignedObjectInParent(gameObject, targetType);
        }

        public static T? GetAssignedObjectInParent<T>(this GameObject gameObject)
        {
            return GameObjectHelper.GetAssignedObjectInParent<T>(gameObject);
        }

        public static object[] GetAssignedObjects(this GameObject gameObject, Type targetType)
        {
            return GameObjectHelper.GetAssignedObjects(gameObject, targetType);
        }

        public static T[] GetAssignedObjects<T>(this GameObject gameObject)
        {
            return GameObjectHelper.GetAssignedObjects<T>(gameObject);
        }

        public static object[] GetAssignedObjectsInChildren(this GameObject gameObject, Type targetType)
        {
            return GameObjectHelper.GetAssignedObjectsInChildren(gameObject, targetType);
        }

        public static T[] GetAssignedObjectsInChildren<T>(this GameObject gameObject)
        {
            return GameObjectHelper.GetAssignedObjectsInChildren<T>(gameObject);
        }

        public static object[] GetAssignedObjectsInParent(this GameObject gameObject, Type targetType)
        {
            return GameObjectHelper.GetAssignedObjectsInParent(gameObject, targetType);
        }

        public static T[] GetAssignedObjectsInParent<T>(this GameObject gameObject)
        {
            return GameObjectHelper.GetAssignedObjectsInParent<T>(gameObject);
        }

        public static bool TryGetAssignedObject(this GameObject gameObject,
                                                Type targetType,
                                                [NotNullWhen(true)] out object? result)
        {
            return GameObjectHelper.TryGetAssignedObject(gameObject, targetType, out result);
        }

        public static bool TryGetAssignedObject<T>(this GameObject gameObject, [NotNullWhen(true)] out T? result)
        {
            return GameObjectHelper.TryGetAssignedObject(gameObject, out result);
        }

        public static bool TryGetAssignedObjectInChildren(this GameObject gameObject,
                                                          Type targetType,
                                                          [NotNullWhen(true)] out object? result)
        {
            return GameObjectHelper.TryGetAssignedObjectInChildren(gameObject, targetType, out result);
        }

        public static bool TryGetAssignedObjectInChildren<T>(this GameObject gameObject,
                                                             [NotNullWhen(true)] out T? result)
        {
            return GameObjectHelper.TryGetAssignedObjectInChildren(gameObject, out result);
        }

        public static bool TryGetAssignedObjectInParent(this GameObject gameObject,
                                                        Type targetType,
                                                        [NotNullWhen(true)] out object? result)
        {
            return GameObjectHelper.TryGetAssignedObjectInParent(gameObject, targetType, out result);
        }

        public static bool TryGetAssignedObjectInParent<T>(this GameObject gameObject,
                                                           [NotNullWhen(true)] out T? result)
        {
            return GameObjectHelper.TryGetAssignedObjectInParent(gameObject, out result);
        }

        public static bool TryGetAssignedObjects(this GameObject gameObject,
                                                 Type targetType,
                                                 out object[] results)
        {
            return GameObjectHelper.TryGetAssignedObjects(gameObject, targetType, out results);
        }

        public static bool TryGetAssignedObjects<T>(this GameObject gameObject, out T[] results)
        {
            return GameObjectHelper.TryGetAssignedObjects(gameObject, out results);
        }

        public static bool TryGetAssignedObjectsInChildren(this GameObject gameObject,
                                                           Type targetType,
                                                           out object[] results)
        {
            return GameObjectHelper.TryGetAssignedObjectsInChildren(gameObject, targetType, out results);
        }

        public static bool TryGetAssignedObjectsInChildren<T>(this GameObject gameObject,
                                                              out T[] results)
        {
            return GameObjectHelper.TryGetAssignedObjectsInChildren(gameObject, out results);
        }

        public static bool TryGetAssignedObjectsInParent(this GameObject gameObject,
                                                         Type targetType,
                                                         out object[] results)
        {
            return GameObjectHelper.TryGetAssignedObjectsInParent(gameObject, targetType, out results);
        }

        public static bool TryGetAssignedObjectsInParent<T>(this GameObject gameObject, out T[] results)
        {
            return GameObjectHelper.TryGetAssignedObjectsInParent(gameObject, out results);
        }
    }
}