using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using CozyColdEnvironments.Reflection;
using System.Linq;

#nullable enable
namespace CozyColdEnvironments.Unity
{
    public static class ComponentHelper
    {
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
    }
}
