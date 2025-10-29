using CCEnvs.Diagnostics;
using CCEnvs.FuncLanguage;
using CCEnvs.Reflection;
using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;

#nullable enable
namespace CCEnvs.Unity
{
    public static class GetAssignedObjectExtensions
    {
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Maybe<object> GetAssignedObject(this GameObject value,
                                                Type targetType)
        {
            return GetAssignedObjectsInternal(value,
                                              targetType,
                                              includeInactive: false,
                                              FindMode.Self,
                                              single: true).FirstOrDefault();
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Maybe<T> GetAssignedObject<T>(this GameObject value)
        {
            return (T?)value.GetAssignedObject(typeof(T));
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Maybe<object> GetAssignedObject(this Component value,
                                                Type targetType)
        {
            CC.Guard.IsNotNull(value, nameof(value));

            return value.gameObject.GetAssignedObject(targetType);
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Maybe<T> GetAssignedObject<T>(this Component value)
        {
            CC.Guard.IsNotNull(value, nameof(value));

            return value.gameObject.GetAssignedObject<T>();
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Maybe<object> GetAssignedObjectInChildren(this GameObject value,
                                                  Type targetType,
                                                  bool includeInactive = false)
        {
            return GetAssignedObjectsInternal(value,
                                              targetType,
                                              includeInactive,
                                              FindMode.InChilds,
                                              single: true).FirstOrDefault();
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Maybe<T> GetAssignedObjectInChildren<T>(this GameObject value,
                                                        bool includeInactive = false)
        {
            return (T?)value.GetAssignedObjectInChildren(typeof(T), includeInactive);
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Maybe<object> GetAssignedObjectInChildren(this Component value,
                                                          Type targetType,
                                                          bool includeInactive = false)
        {
            CC.Guard.IsNotNull(value, nameof(value));

            return value.gameObject.GetAssignedObjectInChildren(targetType, includeInactive);
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Maybe<T> GetAssignedObjectInChildren<T>(this Component value,
                                                              bool includeInactive = false)
        {
            CC.Guard.IsNotNull(value, nameof(value));

            return value.gameObject.GetAssignedObjectInChildren<T>(includeInactive);
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Maybe<object> GetAssignedObjectInParent(this GameObject value,
                                                Type targetType,
                                                bool includeInactive = false)
        {
            return GetAssignedObjectsInternal(value,
                                              targetType,
                                              includeInactive,
                                              FindMode.InParents,
                                              single: true).FirstOrDefault();
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Maybe<T> GetAssignedObjectInParent<T>(this GameObject value,
                                                      bool includeInactive = false)
        {
            return (T?)value.GetAssignedObjectInParent(typeof(T), includeInactive);
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Maybe<object> GetAssignedObjectInParent(this Component value,
                                                        Type targetType,
                                                        bool includeInactive = false)
        {
            CC.Guard.IsNotNull(value, nameof(value));

            return value.gameObject.GetAssignedObjectInParent(targetType, includeInactive);
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Maybe<T> GetAssignedObjectInParent<T>(this Component value,
                                                      bool includeInactive = false)
        {
            CC.Guard.IsNotNull(value, nameof(value));

            return value.gameObject.GetAssignedObjectInParent<T>(includeInactive);
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static object[] GetAssignedObjects(this GameObject gameObject,
                                          Type targetType)
        {
            return GetAssignedObjectsInternal(gameObject,
                                              targetType,
                                              includeInactive: false,
                                              FindMode.Self,
                                              single: false);
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T[] GetAssignedObjects<T>(this GameObject value)
        {
            return value.GetAssignedObjects(typeof(T))
                        .Cast<T>()
                        .ToArray();
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static object[] GetAssignedObjects(this Component value,
                                                  Type targetType)
        {
            CC.Guard.IsNotNull(value, nameof(value));

            return value.gameObject.GetAssignedObjects(targetType);
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T[] GetAssignedObjects<T>(this Component value)
        {
            CC.Guard.IsNotNull(value, nameof(value));

            return value.gameObject.GetAssignedObjects<T>();
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static object[] GetAssignedObjectsInChildren(this GameObject value,
                                                    Type targetType,
                                                    bool includeInactive = false)
        {
            return GetAssignedObjectsInternal(value,
                                              targetType,
                                              includeInactive,
                                              FindMode.InChilds,
                                              single: false);
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T[] GetAssignedObjectsInChildren<T>(this GameObject value,
                                                          bool includeInactive = false)
        {
            return value.GetAssignedObjectsInChildren(typeof(T), includeInactive)
                        .Cast<T>()
                        .ToArray();
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static object[] GetAssignedObjectsInChildren(this Component value,
                                                            Type targetType,
                                                            bool includeInactive = false)
        {
            CC.Guard.IsNotNull(value, nameof(value));

            return value.gameObject.GetAssignedObjectsInChildren(targetType, includeInactive);
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T[] GetAssignedObjectsInChildren<T>(this Component value,
                                                          bool includeInactive = false)
        {
            CC.Guard.IsNotNull(value, nameof(value));

            return value.gameObject.GetAssignedObjectsInChildren<T>(includeInactive);
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static object[] GetAssignedObjectsInParent(this GameObject value,
                                                  Type targetType,
                                                  bool includeInactive = false)
        {
            return GetAssignedObjectsInternal(value,
                                              targetType,
                                              includeInactive,
                                              FindMode.InParents,
                                              single: false);
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T[] GetAssignedObjectsInParent<T>(this GameObject value,
                                                        bool includeInactive = false)
        {
            return value.GetAssignedObjectsInParent(typeof(T), includeInactive)
                        .Cast<T>()
                        .ToArray();
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static object[] GetAssignedObjectsInParent(this Component value,
                                                          Type targetType,
                                                          bool includeInactive = false)
        {
            CC.Guard.IsNotNull(value, nameof(value));

            return value.gameObject.GetAssignedObjectsInParent(targetType, includeInactive);
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T[] GetAssignedObjectsInParent<T>(this Component value,
                                                        bool includeInactive = false)
        {
            CC.Guard.IsNotNull(value, nameof(value));

            return value.gameObject.GetAssignedObjectsInParent<T>(includeInactive);
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
                                                           bool single)
        {
            Component[] cmps = findMode switch
            {
                FindMode.InChilds => gameObject.GetComponentsInChildren(typeof(Component),
                                                                        includeInactive),

                FindMode.InParents => gameObject.GetComponentsInParent(typeof(Component),
                                                                       includeInactive),

                _ => gameObject.GetComponents(typeof(Component)),
            };

            if (single)
            {
                var t = cmps.FirstOrDefault(cmp => cmp.GetType().IsType(targetType)).Maybe();

                return t.Map(x => Range.From<object>(x)).Access(Array.Empty<object>())!;
            }
            else
            {
                var t = cmps.Where(x => x.GetType().IsType(targetType)).ToArray();

                return t;
            }
        }
    }
}
