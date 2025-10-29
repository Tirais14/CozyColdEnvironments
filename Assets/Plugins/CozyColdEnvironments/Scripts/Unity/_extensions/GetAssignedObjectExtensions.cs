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
        public static Maybe<object> GetAssignedObject(this GameObject value,
                                                Type targetType)
        {
            return GetAssignedObjectsInternal(value,
                                              targetType,
                                              includeInactive: false,
                                              FindMode.Self,
                                              single: true).FirstOrDefault();
        }

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

        public static Maybe<T> GetAssignedObjectInChildren<T>(this GameObject value,
                                                        bool includeInactive = false)
        {
            return (T?)value.GetAssignedObjectInChildren(typeof(T), includeInactive);
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static object? GetAssignedObjectInChildren(this Component value,
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

        public static object[] GetAssignedObjects(this GameObject gameObject,
                                          Type targetType)
        {
            return GetAssignedObjectsInternal(gameObject,
                                              targetType,
                                              includeInactive: false,
                                              FindMode.Self,
                                              single: false);
        }

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
            result = GetAssignedObject<T>(value).Access();

            return result.IsNotNull();
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryGetAssignedObject(this Component value,
                                                Type targetType,
                                                [NotNullWhen(true)] out object? result)
        {
            CC.Guard.IsNotNull(value, nameof(value));

            return value.gameObject.TryGetAssignedObject(targetType, out result);
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryGetAssignedObject<T>(this Component value,
                                                   [NotNullWhen(true)] out T? result)
        {
            CC.Guard.IsNotNull(value, nameof(value));

            return value.gameObject.TryGetAssignedObject(out result);
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

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryGetAssignedObjectInChildren(this Component value,
            Type targetType,
            bool includeInactive,
            [NotNullWhen(true)] out object? result)
        {
            CC.Guard.IsNotNull(value, nameof(value));

            return value.gameObject.TryGetAssignedObjectInChildren(targetType,
                                                                   includeInactive,
                                                                   out result);
        }
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryGetAssignedObjectInChildren(this Component value,
            Type targetType,
            [NotNullWhen(true)] out object? result)
        {
            CC.Guard.IsNotNull(value, nameof(value));

            return value.gameObject.TryGetAssignedObjectInChildren(targetType,
                                                                   out result);
        }

        public static bool TryGetAssignedObjectInChildren<T>(this GameObject value,
            bool includeInactive,
            [NotNullWhen(true)] out T? result)
        {
            result = value.GetAssignedObjectInChildren<T>(includeInactive).Access();

            return result.IsNotNull();
        }
        public static bool TryGetAssignedObjectInChildren<T>(this GameObject value,
            [NotNullWhen(true)] out T? result)
        {
            return value.TryGetAssignedObjectInChildren(includeInactive: false, out result);
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryGetAssignedObjectInChildren<T>(this Component value,
            bool includeInactive,
            [NotNullWhen(true)] out T? result)
        {
            CC.Guard.IsNotNull(value, nameof(value));

            return value.gameObject.TryGetAssignedObjectInChildren(includeInactive,
                                                                   out result);
        }
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryGetAssignedObjectInChildren<T>(this Component value,
            [NotNullWhen(true)] out T? result)
        {
            CC.Guard.IsNotNull(value, nameof(value));

            return value.gameObject.TryGetAssignedObjectInChildren(out result);
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

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryGetAssignedObjectInParent(this Component value,
            Type targetType,
            bool includeInactive,
            [NotNullWhen(true)] out object? result)
        {
            CC.Guard.IsNotNull(value, nameof(value));

            return value.gameObject.TryGetAssignedObjectInParent(targetType,
                                                                 includeInactive,
                                                                 out result);
        }
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryGetAssignedObjectInParent(this Component value,
            Type targetType,
            [NotNullWhen(true)] out object? result)
        {
            CC.Guard.IsNotNull(value, nameof(value));

            return value.gameObject.TryGetAssignedObjectInParent(targetType,
                                                                 out result);
        }

        public static bool TryGetAssignedObjectInParent<T>(this GameObject value,
            bool includeInactive,
            [NotNullWhen(true)] out T? result)
        {
            result = value.GetAssignedObjectInParent<T>(includeInactive).Access();

            return result.IsNotNull();
        }
        public static bool TryGetAssignedObjectInParent<T>(this GameObject value,
            [NotNullWhen(true)] out T? result)
        {
            return value.TryGetAssignedObjectInParent(includeInactive: false, out result);
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryGetAssignedObjectInParent<T>(this Component value,
            bool includeInactive,
            [NotNullWhen(true)] out T? result)
        {
            CC.Guard.IsNotNull(value, nameof(value));

            return value.gameObject.TryGetAssignedObjectInParent(includeInactive,
                                                                 out result);
        }
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryGetAssignedObjectInParent<T>(this Component value,
            [NotNullWhen(true)] out T? result)
        {
            CC.Guard.IsNotNull(value, nameof(value));

            return value.gameObject.TryGetAssignedObjectInParent(out result);
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

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryGetAssignedObjects(this Component value,
            Type targetType,
            out object[] results)
        {
            CC.Guard.IsNotNull(value, nameof(value));

            return value.gameObject.TryGetAssignedObjects(targetType, out results);
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryGetAssignedObjects<T>(this Component value,
                                                    out T[] results)
        {
            CC.Guard.IsNotNull(value, nameof(value));

            return value.gameObject.TryGetAssignedObjects(out results);
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

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryGetAssignedObjectsInChildren(this Component value,
                                                           Type targetType,
                                                           bool includeInactive,
                                                           out object[] results)
        {
            CC.Guard.IsNotNull(value, nameof(value));

            return value.gameObject.TryGetAssignedObjectsInChildren(targetType,
                                                                    includeInactive,
                                                                    out results);
        }
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryGetAssignedObjectsInChildren(this Component value,
                                                           Type targetType,
                                                           out object[] results)
        {
            CC.Guard.IsNotNull(value, nameof(value));

            return value.gameObject.TryGetAssignedObjectsInChildren(targetType,
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

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryGetAssignedObjectsInChildren<T>(this Component value,
                                                              bool includeInactive,
                                                              out T[] results)
        {
            CC.Guard.IsNotNull(value, nameof(value));

            return value.gameObject.TryGetAssignedObjectsInChildren(includeInactive,
                                                                    out results);
        }
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryGetAssignedObjectsInChildren<T>(this Component value,
                                                              out T[] results)
        {
            CC.Guard.IsNotNull(value, nameof(value));

            return value.gameObject.TryGetAssignedObjectsInChildren(out results);
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

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryGetAssignedObjectsInParent(this Component value,
                                                         Type targetType,
                                                         bool includeInactive,
                                                         out object[] results)
        {
            CC.Guard.IsNotNull(value, nameof(value));

            return value.gameObject.TryGetAssignedObjectsInParent(targetType,
                                                                  includeInactive,
                                                                  out results);
        }
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryGetAssignedObjectsInParent(this Component value,
                                                         Type targetType,
                                                         out object[] results)
        {
            CC.Guard.IsNotNull(value, nameof(value));

            return value.gameObject.TryGetAssignedObjectsInParent(targetType,
                                                                  out results);
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryGetAssignedObjectsInParent<T>(this Component value,
                                                            bool includeInactive,
                                                            out T[] results)
        {
            CC.Guard.IsNotNull(value, nameof(value));

            return value.gameObject.TryGetAssignedObjectsInParent(includeInactive,
                                                                  out results);
        }
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryGetAssignedObjectsInParent<T>(this Component value,
                                                            out T[] results)
        {
            CC.Guard.IsNotNull(value, nameof(value));

            return value.gameObject.TryGetAssignedObjectsInParent(out results);
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
