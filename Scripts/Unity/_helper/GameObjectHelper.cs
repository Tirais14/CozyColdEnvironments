using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UTIRLib.Reflection;
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

        /// <returns>Removed components</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static Type[] RemoveComponents(RemoveComponentsArguments args)
        {
            if (args is null)
                throw new ArgumentNullException(nameof(args));

            Stack<Component> components = CreateStackByHardDependecies(args.Object);
            var results = new List<Type>(components.Count);
            Component component;
            var predicate = new LoopPredicate(() => components.Count > 0);
            while (predicate)
            {
                component = components.Pop();
                Type componentType = component.GetType();
                if (args.IsToRemoveType(componentType))
                {
                    results.Add(componentType);
                    Object.Destroy(component);
                }
            }

            return results.ToArray();
        }

#pragma warning disable S1854
        /// <exception cref="ArgumentNullException"></exception>
        public static Stack<Component> CreateStackByHardDependecies(
            GameObject gameObject)
        {
            if (gameObject == null)
                throw new ArgumentNullException(nameof(gameObject));

            Component[] components = gameObject.GetComponents<Component>();

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

            //Defines
            Dictionary<Type, Type[]> GetAllDependencies()
            {
                return (from x in components
                        group x by x.GetType() into groups //same as distinct, but for components
                        select groups.First() into x
                        select x.GetType() into t
                        select t)
                        .ToDictionary(x => x, x => ComponentHelper.CollectHardDependencyTypes(x));
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
                foreach (var item in toStack)
                {
                    addedComponentTypes.Add(item.GetType());
                    results.Push(item);
                }
            }
        }
#pragma warning restore S1854
    }
}
