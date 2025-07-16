using System;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;

#nullable enable

namespace UTIRLib.UExtensions
{
    public static class ComponentExtensions
    {
        public static object? GetAssignedObject(this Component component, Type targetType)
        {
            return component.gameObject.GetAssignedObject(targetType);
        }

        public static T? GetAssignedObject<T>(this Component component)
        {
            return component.gameObject.GetAssignedObject<T>();
        }

        public static object? GetAssignedObjectInChildren(this Component component, Type targetType)
        {
            return component.gameObject.GetAssignedObjectInChildren(targetType);
        }

        public static T? GetAssignedObjectInChildren<T>(this Component component)
        {
            return component.gameObject.GetAssignedObjectInChildren<T>();
        }

        public static object? GetAssignedObjectInParent(this Component component, Type targetType)
        {
            return component.gameObject.GetAssignedObjectInParent(targetType);
        }

        public static T? GetAssignedObjectInParent<T>(this Component component)
        {
            return component.gameObject.GetAssignedObjectInParent<T>();
        }

        public static object[] GetAssignedObjects(this Component component, Type targetType)
        {
            return component.gameObject.GetAssignedObjects(targetType);
        }

        public static T[] GetAssignedObjects<T>(this Component component)
        {
            return component.gameObject.GetAssignedObjects<T>();
        }

        public static object[] GetAssignedObjectsInChildren(this Component component, Type targetType)
        {
            return component.gameObject.GetAssignedObjectsInChildren(targetType);
        }

        public static T[] GetAssignedObjectsInChildren<T>(this Component component)
        {
            return component.gameObject.GetAssignedObjectsInChildren<T>();
        }

        public static object[] GetAssignedObjectsInParent(this Component component, Type targetType)
        {
            return component.gameObject.GetAssignedObjectsInParent(targetType);
        }

        public static T[] GetAssignedObjectsInParent<T>(this Component component)
        {
            return component.gameObject.GetAssignedObjectsInParent<T>();
        }

        public static bool TryGetAssignedObject(this Component component,
                                                Type targetType,
                                                [NotNullWhen(true)] out object? result)
        {
            return component.gameObject.TryGetAssignedObject(targetType, out result);
        }

        public static bool TryGetAssignedObject<T>(this Component component,
                                                   [NotNullWhen(true)] out T? result)
        {
            return component.gameObject.TryGetAssignedObject(out result);
        }

        public static bool TryGetAssignedObjectInChildren(this Component component,
                                                          Type targetType,
                                                          [NotNullWhen(true)] out object? result)
        {
            return component.gameObject.TryGetAssignedObjectInChildren(targetType, out result);
        }

        public static bool TryGetAssignedObjectInChildren<T>(this Component component,
                                                             [NotNullWhen(true)] out T? result)
        {
            return component.gameObject.TryGetAssignedObjectInChildren(out result);
        }

        public static bool TryGetAssignedObjectInParent(this Component component,
                                                        Type targetType,
                                                        [NotNullWhen(true)] out object? result)
        {
            return component.gameObject.TryGetAssignedObjectInParent(targetType, out result);
        }

        public static bool TryGetAssignedObjectInParent<T>(this Component component,
                                                           [NotNullWhen(true)] out T? result)
        {
            return component.gameObject.TryGetAssignedObjectInParent(out result);
        }

        public static bool TryGetAssignedObjects(this Component component,
                                                 Type targetType,
                                                 out object[] results)
        {
            return component.gameObject.TryGetAssignedObjects(targetType, out results);
        }

        public static bool TryGetAssignedObjects<T>(this Component component, out T[] results)
        {
            return component.gameObject.TryGetAssignedObjects(out results);
        }

        public static bool TryGetAssignedObjectsInChildren(this Component component, Type targetType, out object[] results)
        {
            return component.gameObject.TryGetAssignedObjectsInChildren(targetType, out results);
        }

        public static bool TryGetAssignedObjectsInChildren<T>(this Component component, out T[] results)
        {
            return component.gameObject.TryGetAssignedObjectsInChildren(out results);
        }

        public static bool TryGetAssignedObjectsInParent(this Component component, Type targetType, out object[] results)
        {
            return component.gameObject.TryGetAssignedObjectsInParent(targetType, out results);
        }

        public static bool TryGetAssignedObjectsInParent<T>(this Component component, out T[] results)
        {
            return component.gameObject.TryGetAssignedObjectsInParent(out results);
        }
    }
}