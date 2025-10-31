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
        public static IEnumerable<object> FindComponentsRaw(GameObject source,
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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IEnumerable<T> FindComponentsRaw<T>(GameObject source,
                                                          bool includeInactive = false,
                                                          FindMode findMode = FindMode.Self)
        {
            return FindComponentsRaw(
                source,
                type: typeof(T),
                includeInactive: includeInactive,
                findMode: findMode)
                .Select(x => x.AsOrDefault<T>().Target!)
                .Where(x => x.IsNotNull());
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Maybe<object> FindComponentRaw(GameObject source,
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

            return FindComponentsRaw(source,
                type: type,
                includeInactive: includeInactive,
                findMode: findMode)
                .FirstOrDefault();
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Maybe<T> FindComponentRaw<T>(GameObject source,
                                                 bool includeInactive = false,
                                                 FindMode findMode = FindMode.Self)
        {
            return FindComponentRaw(
                source,
                type: typeof(T),
                includeInactive: includeInactive,
                findMode: findMode)
                .Cast<T>()
                .RightTarget;
        }
    }
}
