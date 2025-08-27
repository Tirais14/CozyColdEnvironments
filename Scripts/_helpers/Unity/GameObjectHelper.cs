using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UTIRLib.Collections;
using UTIRLib.Reflection;
using UTIRLib.Unity.TypeMatching;
using Object = UnityEngine.Object;

#nullable enable
namespace UTIRLib.Unity
{
    public static class GameObjectHelper
    {
        public static void EnableMonoBehaviours(GameObject gameObject)
        {
            if (gameObject == null)
                throw new ArgumentNullException(nameof(gameObject));

            var monos = gameObject.GetComponents<MonoBehaviour>();
            for (int i = 0; i < monos.Length; i++)
                monos[i].enabled = true;
        }

        public static void DisableMonoBehaviours(GameObject gameObject)
        {
            if (gameObject == null)
                throw new ArgumentNullException(nameof(gameObject));

            var monos = gameObject.GetComponents<MonoBehaviour>();
            for (int i = 0; i < monos.Length; i++)
                monos[i].enabled = false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="gameObject"></param>
        /// <param name="exclude"></param>
        /// <returns>Removed components</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static Type[] RemoveMonoBehaviours(GameObject gameObject)
        {
            if (gameObject == null)
                throw new ArgumentNullException(nameof(gameObject));

            Stack<Component> components = CreateStackByHardDependecies(gameObject);
            var results = new List<Type>(components.Count);
            Component component;
            var predicate = new LoopPredicate(() => components.Count > 0);
            while (predicate)
            {
                component = components.Pop();
                if (component.Is<MonoBehaviour>())
                {
                    results.Add(component.GetType());
                    Object.Destroy(component);
                }
            }

            return results.ToArray();
        }

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

        public static Type[] CollectHardDependencyTypes(Type type)
        {
            IEnumerable<RequireComponent> attributes = type.GetCustomAttributes<RequireComponent>();
            if (attributes.IsNullOrEmpty())
                return Type.EmptyTypes;

            return (from x in attributes
                    select x.AsEnumerable() into types
                    from t in types
                    where t is not null
                    select t).ToArray();
        }

#pragma warning disable S1854
        public static Stack<Component> CreateStackByHardDependecies(
            GameObject go)
        {
            Component[] components = go.GetComponents<Component>();

            Dictionary<Type, Type[]> allDeps = GetAllDependencies();
            IEnumerable<Component> toStack = GetComponentsWithoutDependencies();

            var results = new Stack<Component>(components.Length);
            var addedComponentTypes = new HashSet<Type>(components.Length);

            AddToResults();

            var predicate = new LoopPredicate(() => results.Count < components.Length)
            {
                IterationsLimit = 1000
            };
            while (predicate)
            {
                toStack = GetComponentsByDependencies();
                AddToResults();
            }

            return results;

            Dictionary<Type, Type[]> GetAllDependencies()
            {
                return (from x in components
                        group x by x.GetType() into groups //same as distinct, but for components
                        select groups.First() into x
                        select x.GetType() into t
                        select t)
                        .ToDictionary(x => x, x => CollectHardDependencyTypes(x));
            }

            IEnumerable<Component> GetComponentsByDependencies()
            {
                return from x in components.Where(x => !results.Contains(x))
                       let deps = allDeps[x.GetType()]
                       where deps.All(dep => addedComponentTypes.Any(x => x.IsType(dep)))
                       select x;
            }

            IEnumerable<Component> GetComponentsWithoutDependencies()
            {
                return from x in components
                       let t = x.GetType()
                       where allDeps[t].IsEmpty()
                       select x;
            }

            void AddToResults()
            {
                foreach (var item in components)
                {
                    addedComponentTypes.Add(item.GetType());
                    results.Push(item);
                }
            }
        }
#pragma warning restore S1854
    }
}
