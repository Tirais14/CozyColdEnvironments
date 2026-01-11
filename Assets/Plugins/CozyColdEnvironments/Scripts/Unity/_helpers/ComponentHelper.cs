using CCEnvs.Reflection;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using UnityEngine;
using System.Linq;
using CCEnvs.FuncLanguage;
using UnityEditor;
using CCEnvs.Collections;

#nullable enable
namespace CCEnvs.Unity
{
    public static class ComponentHelper
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

        /// <exception cref="ArgumentNullException"></exception>
        public static Component[] GetHardDependencies(Component component)
        {
            if (component == null)
                throw new ArgumentNullException(nameof(component));

            Type type = component.GetType();
            Type[] depTypes = CollectHardDependencyTypes(type);

            if (depTypes.IsEmpty())
                return Array.Empty<Component>();

            var deps = new Component[depTypes.Length];
            for (int i = 0; i < depTypes.Length; i++)
                deps[i] = component.GetComponent(depTypes[i]);

            return deps;
        }

        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public static Type[] CollectHardDependencyTypes(Type componentType)
        {
            if (componentType is null)
                throw new ArgumentNullException(nameof(componentType));
            if (componentType.IsNotType<Component>())
                throw new ArgumentException(nameof(componentType));

            IEnumerable<RequireComponent> attributes = componentType.GetCustomAttributes<RequireComponent>();
            if (attributes.IsNullOrEmpty())
                return Type.EmptyTypes;

            return (from x in attributes
                    select x.AsEnumerable() into types
                    from t in types
                    where t is not null
                    select t).ToArray();
        }

        public static Maybe<string> GetPersistentGuid(this Component source)
        {
            CC.Guard.IsNotNullSource(source);
            return source.gameObject.GetPersistentGuid();
        }

        public static Maybe<string> GetRuntimeId(this Component source)
        {
            CC.Guard.IsNotNullSource(source);
            return source.gameObject.GetRuntimeId();    
        }

        public static HierarchyPath GetHierarchyPath(this Component source)
        {
            CC.Guard.IsNotNullSource(source);
            return source.transform.GetHierarchyPath();
        }

        public static bool MatchHierarchyPath(this Component source, HierarchyPath hierarchyPath)
        {
            CC.Guard.IsNotNullSource(source);
            return source.transform.MatchHierarchyPath(hierarchyPath);
        }
    }
}
