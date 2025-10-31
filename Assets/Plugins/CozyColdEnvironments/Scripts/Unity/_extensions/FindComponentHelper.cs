using CCEnvs.Diagnostics;
using CCEnvs.FuncLanguage;
using CCEnvs.Reflection;
using CommunityToolkit.Diagnostics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;

#nullable enable
namespace CCEnvs.Unity
{
    public static class FindComponentHelper
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IEnumerable<object> FindComponentsRaw(this GameObject source,
                                                            Type? type = null,
                                                            bool includeInactive = false,
                                                            FindMode findMode = FindMode.Self)
        {
            Guard.IsNotNull(source, nameof(source));

            Component[] cmps = findMode switch
            {
                FindMode.InChilds => source.GetComponentsInChildren(typeof(Component),
                                                                    includeInactive),

                FindMode.InParents => source.GetComponentsInParent(typeof(Component),
                                                                   includeInactive),

                _ => source.GetComponents(typeof(Component)),
            };

            type ??= typeof(Component);

            return cmps.Where(cmp => cmp.GetType().IsType(type));
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IEnumerable<object> FindComponentsRaw(this Component source,
                                                            Type? type = null,
                                                            bool includeInactive = false,
                                                            FindMode findMode = FindMode.Self)
        {
            return source.gameObject.FindComponentsRaw(type,
                includeInactive,
                findMode);
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IEnumerable<T> FindComponentsRaw<T>(this GameObject source,
                                                               bool includeInactive = false,
                                                               FindMode findMode = FindMode.Self)
        {
            return source.FindComponentsRaw(typeof(T),
                includeInactive,
                findMode)
                .Select(x => x.AsOrDefault<T>().Target!)
                .Where(x => x.IsNotNull());
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IEnumerable<T> FindComponentsRaw<T>(this Component source,
                                                               bool includeInactive = false,
                                                               FindMode findMode = FindMode.Self)
        {
            return source.gameObject.FindComponentsRaw<T>(includeInactive, findMode);
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static object FindComponentRaw(this GameObject source,
                                              Type? type = null,
                                              bool includeInactive = false,
                                              FindMode findMode = FindMode.Self)
        {
            CC.Guard.IsNotNull(source, nameof(source));

            type ??= typeof(Component);

            if (type.IsType<Component>())
            {
                return findMode switch
                {
                    FindMode.Self => source.GetComponent(type),
                    FindMode.InChilds => source.GetComponentInChildren(type, includeInactive),
                    FindMode.InParents => source.GetComponentInParent(type, includeInactive),
                    _ => throw new InvalidOperationException(findMode.ToString())
                };
            }

            return source.FindComponentsRaw(type,
                includeInactive,
                findMode)
                .FirstOrDefault();
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static object FindComponentRaw(this Component source,
                                              Type? type = null,
                                              bool includeInactive = false,
                                              FindMode findMode = FindMode.Self)
        {
            return source.FindComponentsRaw(type,
                includeInactive,
                findMode)
                .FirstOrDefault();
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Maybe<T> FindComponentRaw<T>(this GameObject source,
                                                 bool includeInactive = false,
                                                 FindMode findMode = FindMode.Self)
        {
            return source.FindComponentsRaw<T>(includeInactive, findMode)
                         .FirstOrDefault()
                         .AsOrDefault<T>();
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Maybe<T> FindComponentRaw<T>(this Component source,
                                                 bool includeInactive = false,
                                                 FindMode findMode = FindMode.Self)
        {
            return source.gameObject.FindComponentRaw<T>(includeInactive, findMode);
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Maybe<T> FindComponent<T>(this GameObject source)
        {
            return source.FindComponentRaw<T>();
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Maybe<T> FindComponent<T>(this Component source)
        {
            return source.gameObject.FindComponent<T>();
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Maybe<T> FindComponentInChildren<T>(this GameObject source,
                                                          bool includeInactive = false)
        {
            return source.FindComponentRaw<T>(
                includeInactive: includeInactive,
                findMode: FindMode.InChilds
                );
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Maybe<T> FindComponentInChildren<T>(this Component source,
                                                          bool includeInactive = false)
        {
            return source.FindComponentRaw<T>(
                includeInactive: includeInactive,
                findMode: FindMode.InChilds
                );
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Maybe<T> FindComponentInParent<T>(this GameObject source,
                                                        bool includeInactive = false)
        {
            return source.FindComponentRaw<T>(
                includeInactive: includeInactive,
                findMode: FindMode.InParents
                );
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Maybe<T> FindComponentInParent<T>(this Component source,
                                                      bool includeInactive = false)
        {
            CC.Guard.IsNotNull(source, nameof(source));

            return source.gameObject.FindComponentInParent<T>(includeInactive);
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T[] FindComponents<T>(this GameObject source)
        {
            return source.FindComponentsRaw<T>().ToArray();
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T[] FindComponents<T>(this Component source)
        {
            return source.FindComponentsRaw<T>().ToArray();
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T[] FindComponentsInChildren<T>(this GameObject source,
                                                      bool includeInactive = false)
        {
            return source.FindComponentsRaw<T>(
                includeInactive: includeInactive, 
                findMode: FindMode.InChilds)
                .ToArray();
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T[] FindComponentsInChildren<T>(this Component source,
                                                      bool includeInactive = false)
        {
            return source.FindComponentsRaw<T>(
                includeInactive: includeInactive,
                findMode: FindMode.InChilds)
                .ToArray();
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T[] FindComponentsInParent<T>(this GameObject source,
                                                    bool includeInactive = false)
        {
            return source.FindComponentsRaw<T>(
                includeInactive: includeInactive,
                findMode: FindMode.InParents)
                .ToArray();
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T[] FindComponentsInParent<T>(this Component source,
                                                    bool includeInactive = false)
        {
            return source.FindComponentsRaw<T>(
                includeInactive: includeInactive,
                findMode: FindMode.InParents)
                .ToArray();
        }
    }
}
