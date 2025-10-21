using CCEnvs.Unity.Extensions;
using System;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;

#nullable enable
namespace CCEnvs.Unity
{
    public static class FromComponent
    {
        public static object? GetAssignedObject(this Component value,
                                                Type targetType)
        {
            CC.Guard.NullArgument(value, nameof(value));

            return value.gameObject.GetAssignedObject(targetType);
        }

        public static T? GetAssignedObject<T>(this Component value)
        {
            CC.Guard.NullArgument(value, nameof(value));

            return value.gameObject.GetAssignedObject<T>();
        }

        public static object? GetAssignedObjectInChildren(this Component value,
                                                          Type targetType,
                                                          bool includeInactive = false)
        {
            CC.Guard.NullArgument(value, nameof(value));

            return value.gameObject.GetAssignedObjectInChildren(targetType, includeInactive);
        }

        public static T? GetAssignedObjectInChildren<T>(this Component value,
                                                        bool includeInactive = false)
        {
            CC.Guard.NullArgument(value, nameof(value));

            return value.gameObject.GetAssignedObjectInChildren<T>(includeInactive);
        }

        public static object? GetAssignedObjectInParent(this Component value,
                                                        Type targetType,
                                                        bool includeInactive = false)
        {
            CC.Guard.NullArgument(value, nameof(value));

            return value.gameObject.GetAssignedObjectInParent(targetType, includeInactive);
        }

        public static T? GetAssignedObjectInParent<T>(this Component value,
                                                      bool includeInactive = false)
        {
            CC.Guard.NullArgument(value, nameof(value));

            return value.gameObject.GetAssignedObjectInParent<T>(includeInactive);
        }

        public static object[] GetAssignedObjects(this Component value,
                                                  Type targetType)
        {
            CC.Guard.NullArgument(value, nameof(value));

            return value.gameObject.GetAssignedObjects(targetType);
        }

        public static T[] GetAssignedObjects<T>(this Component value)
        {
            CC.Guard.NullArgument(value, nameof(value));

            return value.gameObject.GetAssignedObjects<T>();
        }

        public static object[] GetAssignedObjectsInChildren(this Component value,
                                                            Type targetType,
                                                            bool includeInactive = false)
        {
            CC.Guard.NullArgument(value, nameof(value));

            return value.gameObject.GetAssignedObjectsInChildren(targetType, includeInactive);
        }

        public static T[] GetAssignedObjectsInChildren<T>(this Component value,
                                                          bool includeInactive = false)
        {
            CC.Guard.NullArgument(value, nameof(value));

            return value.gameObject.GetAssignedObjectsInChildren<T>(includeInactive);
        }

        public static object[] GetAssignedObjectsInParent(this Component value,
                                                          Type targetType,
                                                          bool includeInactive = false)
        {
            CC.Guard.NullArgument(value, nameof(value));

            return value.gameObject.GetAssignedObjectsInParent(targetType, includeInactive);
        }

        public static T[] GetAssignedObjectsInParent<T>(this Component value,
                                                        bool includeInactive = false)
        {
            CC.Guard.NullArgument(value, nameof(value));

            return value.gameObject.GetAssignedObjectsInParent<T>(includeInactive);
        }

        public static bool TryGetAssignedObject(this Component value,
                                                Type targetType,
                                                [NotNullWhen(true)] out object? result)
        {
            CC.Guard.NullArgument(value, nameof(value));

            return value.gameObject.TryGetAssignedObject(targetType, out result);
        }

        public static bool TryGetAssignedObject<T>(this Component value,
                                                   [NotNullWhen(true)] out T? result)
        {
            CC.Guard.NullArgument(value, nameof(value));

            return value.gameObject.TryGetAssignedObject(out result);
        }

        public static bool TryGetAssignedObjectInChildren(this Component value,
            Type targetType,
            bool includeInactive,
            [NotNullWhen(true)] out object? result)
        {
            CC.Guard.NullArgument(value, nameof(value));

            return value.gameObject.TryGetAssignedObjectInChildren(targetType,
                                                                   includeInactive,
                                                                   out result);
        }
        public static bool TryGetAssignedObjectInChildren(this Component value,
            Type targetType,
            [NotNullWhen(true)] out object? result)
        {
            CC.Guard.NullArgument(value, nameof(value));

            return value.gameObject.TryGetAssignedObjectInChildren(targetType,
                                                                   out result);
        }

        public static bool TryGetAssignedObjectInChildren<T>(this Component value,
            bool includeInactive,
            [NotNullWhen(true)] out T? result)
        {
            CC.Guard.NullArgument(value, nameof(value));

            return value.gameObject.TryGetAssignedObjectInChildren(includeInactive,
                                                                   out result);
        }
        public static bool TryGetAssignedObjectInChildren<T>(this Component value,
            [NotNullWhen(true)] out T? result)
        {
            CC.Guard.NullArgument(value, nameof(value));

            return value.gameObject.TryGetAssignedObjectInChildren(out result);
        }

        public static bool TryGetAssignedObjectInParent(this Component value,
            Type targetType,
            bool includeInactive,
            [NotNullWhen(true)] out object? result)
        {
            CC.Guard.NullArgument(value, nameof(value));

            return value.gameObject.TryGetAssignedObjectInParent(targetType,
                                                                 includeInactive,
                                                                 out result);
        }
        public static bool TryGetAssignedObjectInParent(this Component value,
            Type targetType,
            [NotNullWhen(true)] out object? result)
        {
            CC.Guard.NullArgument(value, nameof(value));

            return value.gameObject.TryGetAssignedObjectInParent(targetType,
                                                                 out result);
        }

        public static bool TryGetAssignedObjectInParent<T>(this Component value,
            bool includeInactive,
            [NotNullWhen(true)] out T? result)
        {
            CC.Guard.NullArgument(value, nameof(value));

            return value.gameObject.TryGetAssignedObjectInParent(includeInactive,
                                                                 out result);
        }
        public static bool TryGetAssignedObjectInParent<T>(this Component value,
            [NotNullWhen(true)] out T? result)
        {
            CC.Guard.NullArgument(value, nameof(value));

            return value.gameObject.TryGetAssignedObjectInParent(out result);
        }

        public static bool TryGetAssignedObjects(this Component value,
            Type targetType,
            out object[] results)
        {
            CC.Guard.NullArgument(value, nameof(value));

            return value.gameObject.TryGetAssignedObjects(targetType, out results);
        }

        public static bool TryGetAssignedObjects<T>(this Component value,
                                                    out T[] results)
        {
            CC.Guard.NullArgument(value, nameof(value));

            return value.gameObject.TryGetAssignedObjects(out results);
        }

        public static bool TryGetAssignedObjectsInChildren(this Component value,
                                                           Type targetType,
                                                           bool includeInactive,
                                                           out object[] results)
        {
            CC.Guard.NullArgument(value, nameof(value));

            return value.gameObject.TryGetAssignedObjectsInChildren(targetType,
                                                                    includeInactive,
                                                                    out results);
        }
        public static bool TryGetAssignedObjectsInChildren(this Component value,
                                                           Type targetType,
                                                           out object[] results)
        {
            CC.Guard.NullArgument(value, nameof(value));

            return value.gameObject.TryGetAssignedObjectsInChildren(targetType,
                                                                    out results);
        }


        public static bool TryGetAssignedObjectsInChildren<T>(this Component value,
                                                              bool includeInactive,
                                                              out T[] results)
        {
            CC.Guard.NullArgument(value, nameof(value));

            return value.gameObject.TryGetAssignedObjectsInChildren(includeInactive,
                                                                    out results);
        }
        public static bool TryGetAssignedObjectsInChildren<T>(this Component value,
                                                              out T[] results)
        {
            CC.Guard.NullArgument(value, nameof(value));

            return value.gameObject.TryGetAssignedObjectsInChildren(out results);
        }

        public static bool TryGetAssignedObjectsInParent(this Component value,
                                                         Type targetType,
                                                         bool includeInactive,
                                                         out object[] results)
        {
            CC.Guard.NullArgument(value, nameof(value));

            return value.gameObject.TryGetAssignedObjectsInParent(targetType,
                                                                  includeInactive,
                                                                  out results);
        }
        public static bool TryGetAssignedObjectsInParent(this Component value,
                                                         Type targetType,
                                                         out object[] results)
        {
            CC.Guard.NullArgument(value, nameof(value));

            return value.gameObject.TryGetAssignedObjectsInParent(targetType,
                                                                  out results);
        }

        public static bool TryGetAssignedObjectsInParent<T>(this Component value,
                                                            bool includeInactive,
                                                            out T[] results)
        {
            CC.Guard.NullArgument(value, nameof(value));

            return value.gameObject.TryGetAssignedObjectsInParent(includeInactive,
                                                                  out results);
        }
        public static bool TryGetAssignedObjectsInParent<T>(this Component value,
                                                            out T[] results)
        {
            CC.Guard.NullArgument(value, nameof(value));

            return value.gameObject.TryGetAssignedObjectsInParent(out results);
        }
    }
}
