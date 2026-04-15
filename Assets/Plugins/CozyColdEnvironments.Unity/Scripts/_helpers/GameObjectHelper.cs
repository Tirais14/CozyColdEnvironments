using CCEnvs.Collections;
using CCEnvs.FuncLanguage;
using CCEnvs.Pools;
using CCEnvs.Reflection;
using CCEnvs.Unity.Components;
using CommunityToolkit.Diagnostics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using UnityEditor;
using UnityEngine;
using UnityEngine.TextCore.Text;
using ZLinq;
using Object = UnityEngine.Object;

#nullable enable
namespace CCEnvs.Unity
{
    public static class GameObjectHelper
    {
        /// <exception cref="ArgumentNullException"></exception>
        public static bool TryGetComponentInChildren(this GameObject value,
            Type type,
            bool includeInactive,
            [NotNullWhen(true)] out Component? result)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            result = value.GetComponentInChildren(type, includeInactive);

            return result != null;
        }

        public static bool TryGetComponentInChildren(this GameObject value,
            Type type,
            [NotNullWhen(true)] out Component? result)
        {
            return value.TryGetComponentInChildren(type,
                                                   includeInactive: false,
                                                   out result);
        }

        public static bool TryGetComponentInChildren<T>(this GameObject value,
            bool includeInactive,
            [NotNullWhen(true)] out T? result)
        {
            result = value.GetComponentInChildren<T>(includeInactive);

            return result.IsNotNull();
        }

        public static bool TryGetComponentInChildren<T>(this GameObject value,
            [NotNullWhen(true)] out T? result)
        {
            return value.TryGetComponentInChildren(includeInactive: false, out result);
        }

        /// <exception cref="ArgumentNullException"></exception>
        public static bool TryGetComponentInParent(this GameObject value,
            Type type,
            bool includeInactive,
            [NotNullWhen(true)] out Component? result)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            result = value.GetComponentInParent(type, includeInactive);

            return result != null;
        }
        public static bool TryGetComponentInParent(this GameObject value,
            Type type,
            [NotNullWhen(true)] out Component? result)
        {
            return value.TryGetComponentInParent(type,
                                                 includeInactive: false,
                                                 out result);
        }

        public static bool TryGetComponentInParent<T>(this GameObject value,
            bool includeInactive,
            [NotNullWhen(true)] out T? result)
        {
            result = value.GetComponentInParent<T>(includeInactive);

            return result.IsNotNull();
        }
        public static bool TryGetComponentInParent<T>(this GameObject value,
            [NotNullWhen(true)] out T? result)

        {
            return value.TryGetComponentInParent(includeInactive: false, out result);
        }

        public static void EnableMonoBehaviours(GameObject gameObject)
        {
            if (gameObject == null)
                throw new ArgumentNullException(nameof(gameObject));

            MonoBehaviour[] monos = gameObject.GetComponents<MonoBehaviour>();
            for (int i = 0; i < monos.Length; i++)
                monos[i].enabled = true;
        }

        public static void DisableMonoBehaviours(GameObject gameObject)
        {
            if (gameObject == null)
                throw new ArgumentNullException(nameof(gameObject));

            MonoBehaviour[] monos = gameObject.GetComponents<MonoBehaviour>();
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

            var loopFuse = LoopFuse.Create();

            Component component;

            while (loopFuse.DebugMoveNext() && components.Count > 0)
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

        /// <exception cref="ArgumentNullException"></exception>
        public static Stack<Component> CreateStackByHardDependecies(
            GameObject gameObject)
        {
            if (gameObject == null)
                throw new ArgumentNullException(nameof(gameObject));

            Component[] components = gameObject.GetComponents<Component>();

            Dictionary<Type, Type[]> allDeps = getAllDependencies();
            IEnumerable<Component> toStack = getComponentsWithoutDependencies();

            var results = new Stack<Component>(components.Length);
            var addedComponentTypes = new HashSet<Type>(components.Length);

            addToResults();

            var loopFuse = LoopFuse.Create();

            while (loopFuse.MoveNext() && results.Count < components.Length)
            {
                toStack = getComponentsByDependencies();
                addToResults();
            }

            return results;

            //Defines
            Dictionary<Type, Type[]> getAllDependencies()
            {
                return (from x in components
                        group x by x.GetType() into groups //same as distinct, but for components
                        select groups.First() into x
                        select x.GetType() into t
                        select t)
                        .ToDictionary(x => x, x => ComponentHelper.CollectHardDependencyTypes(x));
            }

            IEnumerable<Component> getComponentsByDependencies()
            {
                return from x in components.Where(x => !results.Contains(x))
                       let deps = allDeps[x.GetType()]
                       where deps.All(dep => addedComponentTypes.Any(x => x.IsType(dep)))
                       select x;
            }

            IEnumerable<Component> getComponentsWithoutDependencies()
            {
                return from x in components
                       let t = x.GetType()
                       where allDeps[t].IsEmpty()
                       select x;
            }

            void addToResults()
            {
                foreach (Component item in toStack)
                {
                    addedComponentTypes.Add(item.GetType());
                    results.Push(item);
                }
            }
        }

        public static string? GetPersistentGuid(this GameObject source)
        {
            CC.Guard.IsNotNullSource(source);

            return source.Q()
                .Component<PersistentGuid>()
                .Lax()
                .Map(x => x.Guid)
                .Where(x => x.IsNotNullOrWhiteSpace())
                .GetValue();
        }

        public static Maybe<GameObject> FindByPersistenGuid(string guid, bool includeInactive = false)
        {
            Guard.IsNotNullOrWhiteSpace(guid);

            foreach (var cmp in GameObjectQuery.Scene.IncludeInactive(includeInactive).Components<PersistentGuid>())
            {
                if (cmp.Guid.IsNullOrWhiteSpace())
                    continue;

                if (cmp.Guid.EqualsOrdinal(guid, ignoreCase: false))
                    return cmp.gameObject;
            }

            return Maybe<GameObject>.None;
        }

        public static HierarchyPath GetHierarchyPath(this GameObject source)
        {
            CC.Guard.IsNotNullSource(source);
            return source.transform.GetHierarchyPath();
        }

        /// <summary>
        /// Trying to resolve all specified dependecies by <see cref="RequireComponent"/> and in result correctly deletes them. Otherwise will be printed exception and component will be skipped.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="componentTypes">keep empty to delete all</param>
        public static void RemoveComponents(this GameObject source, params Type[] componentTypes)
        {
            CC.Guard.IsNotNullSource(source);
            Guard.IsNotNull(componentTypes, nameof(componentTypes));

            var cmpInfos = source.GetComponents<Component>()
                .AsValueEnumerable()
                .Select(
                static cmp =>
                {
                    var type = cmp.GetType();
                    var requireCmpAttribute = type.GetCustomAttribute<RequireComponent>(inherit: true).Maybe();

                    return (cmp, type, requireCmpAttribute);
                })
                .OrderBy(
                static cmpInfo =>
                {
                    if (!cmpInfo.requireCmpAttribute.TryGetValue(out var reqAttribute))
                        return int.MaxValue;

                    var attributes = Loops.BreadthFirstSearch(reqAttribute,
                        static reqAttribute =>
                        {
                            return reqAttribute.TypesToArray()
                                .Select(x => x.GetCustomAttribute<RequireComponent>())
                                .ToArray();
                        });

                    return attributes.Count;
                });

            using var _ = UnityEngine.Pool.HashSetPool<Type>.Get(out var componentTypeSet);
            componentTypeSet.AddRange(componentTypes);

            foreach (var (cmp, type, _) in cmpInfos)
            {
                if (type.IsType<Transform>())
                    continue;

                if (componentTypes.IsNotEmpty() && !componentTypeSet.Contains(type))
                    continue;

                try
                {
                    Object.Destroy(cmp);
                }
                catch (Exception ex)
                {
                    type.PrintException(ex);
                }
            }
        }

        public static bool MatchHierarchyPath(this GameObject source, HierarchyPath hierarchyPath)
        {
            CC.Guard.IsNotNullSource(source);
            Guard.IsNotDefault(hierarchyPath, nameof(hierarchyPath));

            return source.transform.MatchHierarchyPath(hierarchyPath);
        }

        public static Maybe<GameObject> FindByHierarchyPath(HierarchyPath path, bool includeInactive = false)
        {
            Guard.IsNotDefault(path, nameof(path));

            var pathParts = path.Split();

            foreach (var transform in GameObjectQuery.Scene.WithName(pathParts[0]).Transforms())
            {
                if (transform.MatchHierarchyPath(path))
                    return transform.gameObject;
            }

            return Maybe<GameObject>.None;

            //var goOption = GameObjectQuery.Scene.IncludeInactive(includeInactive)
            //    .WithName(pathParts[0])
            //    .GameObject()
            //    .Lax();

            //GameObject? go = goOption.GetValue();

            //foreach (var pathPart in pathParts.Skip(1))
            //{
            //    if (!goOption.TryGetValue(out go))
            //        return Maybe<GameObject>.None;

            //    goOption = go.Q()
            //        .WithName(pathPart)
            //        .ChildrenGameObject()
            //        .Lax();
            //}

            //return go;
        }

        #region GetComponentsNonAlloc

        #region Self

        public static int GetComponentsNonAlloc(
            this GameObject source,
            Type? type,
            ref List<Component>? results
            )
        {
            CC.Guard.IsNotNullSource(source);

            bool hasResults = results is not null;
            bool capacityIncresed = false;

            int resultCount = hasResults ? results!.Count : 0;
            int cmpCount = source.GetComponentCount();

            Component cmp;

            for (int i = 0; i < cmpCount; i++)
            {
                cmp = source.GetComponentAtIndex(i);

                if (type != null && cmp.IsNotInstanceOfType(type))
                    continue;

                if (!hasResults)
                {
                    results ??= new List<Component>(cmpCount);
                    capacityIncresed = true;
                    hasResults = true;
                }
                else if (!capacityIncresed)
                {
                    results!.TryIncreaseCapacity(cmpCount);
                    capacityIncresed = true;
                }

                results!.Add(cmp);
            }

            return (hasResults ? results!.Count : 0) - resultCount;
        }

        [DebuggerHidden]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int GetComponentsNonAlloc<T>(
            this GameObject source,
            ref List<Component>? results
            )
        {
            return source.GetComponentsNonAlloc(TypeofCache<T>.Type, ref results);
        }

        public static int GetComponentsNonAlloc(
            this GameObject source,
            Type? type,
            List<Component> results
            )
        {
            CC.Guard.IsNotNullSource(source);
            Guard.IsNotNull(results);

            int resultCount = results.Count;
            int cmpCount = source.GetComponentCount();

            results.TryIncreaseCapacity(cmpCount);

            Component cmp;

            for (int i = 0; i < cmpCount; i++)
            {
                cmp = source.GetComponentAtIndex(i);

                if (type != null && cmp.IsNotInstanceOfType(type))
                    continue;

                results.Add(cmp);
            }

            return results.Count - resultCount;
        }

        [DebuggerHidden]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int GetComponentsNonAlloc<T>(
            this GameObject source,
            List<Component> results
            )
        {
            return source.GetComponentsNonAlloc(TypeofCache<T>.Type, results);
        }

        #endregion Self

        #region InChildren

        #region RefList

        public static int GetComponentsInChildrenNonAlloc(
            this GameObject source,
            Type? type,
            ref List<Component>? results,
            bool includeInactive = false
            )
        {
            CC.Guard.IsNotNullSource(source);

            return CollectComponentsInChildren(
                source.transform,
                type,
                ref results,
                includeInactive
                );
        }

        [DebuggerHidden]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int GetComponentsInChildrenNonAlloc<T>(
            this GameObject source,
            ref List<Component>? results,
            bool includeInactive = false
            )
        {
            return source.GetComponentsInChildrenNonAlloc(
                TypeofCache<T>.Type,
                ref results,
                includeInactive
                );
        }

        private static int CollectComponentsInChildren(
            Transform current,
            Type? type,
            ref List<Component>? results,
            bool includeInactive,
            int depth = 0
            )
        {
            if (!includeInactive && !current.gameObject.activeInHierarchy)
                return 0;

            int foundCount = 0;
            int childCount = current.childCount;

            if (depth >= 256)
            {
                using var toProcess = QueuePool<Transform>.Shared.Get();

                toProcess.Value.Enqueue(current);

                var loopFuse = LoopFuse.Create();

                Transform child;

                while (toProcess.Value.TryDequeue(out current) && loopFuse.MoveNext())
                {
                    childCount = current.childCount;

                    for (int i = 0; i < childCount; i++)
                    {
                        child = current.GetChild(i);

                        if (!includeInactive && !child.gameObject.activeInHierarchy)
                            continue;

                        foundCount += child.GetComponentsNonAlloc(type, ref results!);

                        toProcess.Value.Enqueue(child);
                    }
                }
            }
            else
            {
                foundCount += current.GetComponentsNonAlloc(type, ref results);

                for (int i = 0; i < childCount; i++)
                {
                    foundCount += CollectComponentsInChildren(
                        current.GetChild(i),
                        type,
                        ref results,
                        includeInactive,
                        depth + 1
                        );
                }
            }

            return foundCount;
        }

        #endregion RefList

        #region NonRefList

        public static int GetComponentsInChildrenNonAlloc(
            this GameObject source,
            Type? type,
            List<Component> results,
            bool includeInactive = false
            )
        {
            CC.Guard.IsNotNullSource(source);
            Guard.IsNotNull(results);

            int resultCount = results.Count;

            results.TryIncreaseCapacity(source.GetComponentCount());

            CollectComponentsInChildrenNonAlloc(
                source.transform,
                type,
                results,
                includeInactive
                );

            return results.Count - resultCount;
        }

        [DebuggerHidden]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int GetComponentsInChildrenNonAlloc<T>(
            this GameObject source,
            List<Component> results,
            bool includeInactive = false
            )
        {
            return source.GetComponentsInChildrenNonAlloc(
                TypeofCache<T>.Type,
                results,
                includeInactive
                );
        }

        private static int CollectComponentsInChildrenNonAlloc(
            Transform current,
            Type? type,
            List<Component> results,
            bool includeInactive = false,
            int depth = 0
            )
        {
            if (!includeInactive && !current.gameObject.activeInHierarchy)
                return 0;

            int foundCount = 0;
            int childCount = current.childCount;

            if (depth >= 256)
            {
                using var toProcess = QueuePool<Transform>.Shared.Get();

                toProcess.Value.Enqueue(current);

                var loopFuse = LoopFuse.Create();

                Transform child;

                while (toProcess.Value.TryDequeue(out current) && loopFuse.MoveNext())
                {
                    childCount = current.childCount;

                    for (int i = 0; i < childCount; i++)
                    {
                        child = current.GetChild(i);

                        if (!includeInactive && !child.gameObject.activeInHierarchy)
                            continue;

                        foundCount += child.GetComponentsNonAlloc(type, results);

                        toProcess.Value.Enqueue(child);
                    }
                }
            }
            else
            {
                foundCount += current.GetComponentsNonAlloc(type, results);

                for (int i = 0; i < childCount; i++)
                {
                    foundCount += CollectComponentsInChildrenNonAlloc(
                        current.GetChild(i),
                        type,
                        results,
                        includeInactive,
                        depth + 1
                        );
                }
            }

            return foundCount;
        }

        #endregion NonRefList

        #endregion InChildren

        #region InParent

        public static int GetComponentsInParentNonAlloc(
             this GameObject source,
             Type? type,
             ref List<Component>? results
            )
        {
            CC.Guard.IsNotNullSource(source);

            int foundCount = 0;

            Transform current = source.transform;
            while (current != null)
            {
                foundCount += current.GetComponentsNonAlloc(type, ref results);
                current = current.parent;
            }

            return foundCount;
        }

        [DebuggerHidden]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int GetComponentsInParentNonAlloc<T>(
            this GameObject source,
            ref List<Component>? results)
        {
            return source.GetComponentsInParentNonAlloc(TypeofCache<T>.Type, ref results);
        }

        public static int GetComponentsInParentNonAlloc(
            this GameObject source,
            Type? type,
            List<Component> results)
        {
            CC.Guard.IsNotNullSource(source);
            Guard.IsNotNull(results);

            int foundCount = 0;

            results.TryIncreaseCapacity(16); // Эвристика для цепи родителей

            Transform current = source.transform;
            while (current != null)
            {
                foundCount += current.GetComponentsNonAlloc(type, results);
                current = current.parent;
            }

            return foundCount;
        }

        [DebuggerHidden]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int GetComponentsInParentNonAlloc<T>(
            this GameObject source,
            List<Component> results)
        {
            return source.GetComponentsInParentNonAlloc(TypeofCache<T>.Type, results);
        }

        #endregion InParent

        #endregion GetComponentsNonAlloc

        public static bool HasComponent(this GameObject source, Type type)
        {
            CC.Guard.IsNotNullSource(source);
            Guard.IsNotNull(type);

            var cmpCount = source.GetComponentCount();

            if (source.GetComponentAtIndex(cmpCount-- - 1).IsInstanceOfType(type))
                return true;

            for (int i = 0; i < cmpCount; i++)
                if (source.GetComponentAtIndex(i).IsInstanceOfType(type))
                    return true;

            return false;
        }

        public static bool HasComponent<T>(this GameObject source)
        {
            CC.Guard.IsNotNullSource(source);

            return source.HasComponent(TypeofCache<T>.Type);
        }
    }
}

