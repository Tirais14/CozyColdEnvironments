using CCEnvs.Collections;
using CCEnvs.Diagnostics;
using CCEnvs.FuncLanguage;
using CCEnvs.Reflection;
using CCEnvs.Unity.Components;
using CommunityToolkit.Diagnostics;
using Cysharp.Text;
using Humanizer;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;
using UnityEngine.Pool;
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
            Component component;
            var predicate = new LoopFuse(() => components.Count > 0);
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

            var predicate = new LoopFuse(() => results.Count < components.Length)
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
                foreach (Component item in toStack)
                {
                    addedComponentTypes.Add(item.GetType());
                    results.Push(item);
                }
            }
        }

        public static Maybe<string> GetPersistentGuid(this GameObject source)
        {
            CC.Guard.IsNotNullSource(source);

            return source.Q()
                .Component<PersistentGuid>()
                .Lax()
                .Map(x => x.Guid)
                .Where(x => x.IsNotNullOrWhiteSpace());
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

        public static Maybe<string> GetRuntimeId(this GameObject source)
        {
            CC.Guard.IsNotNullSource(source);

            return source.Q()
                .Component<RuntimeId>()
                .Lax()
                .Map(x => x.Id)
                .Where(x => x.IsNotNullOrWhiteSpace());
        }

        public static Maybe<GameObject> FindByRuntimeId(string id, bool includeInactive = false)
        {
            Guard.IsNotNullOrWhiteSpace(id);

            foreach (var cmp in GameObjectQuery.Scene.IncludeInactive(includeInactive).Components<RuntimeId>())
            {
                if (cmp.Id.EqualsOrdinal(id, ignoreCase: false))
                    return cmp.gameObject;
            }

            return Maybe<GameObject>.None;
        }

        public static string GetHierarchyPath(this GameObject source)
        {
            CC.Guard.IsNotNullSource(source);

            if (source.transform.parent == null)
                return source.name;

            var parents = Do.Collect(source.transform, (x) => x.parent);
            using var pathBuilder = ZString.CreateStringBuilder();
            pathBuilder.Grow(parents.Count);
            pathBuilder.AppendJoin("/", parents.Reverse().AsValueEnumerable().Select(x => x.name).AsEnumerable());

            return pathBuilder.ToString();
        }

        public static RuntimeId AddRuntimeIdComponent(this GameObject source, string id)
        {
            CC.Guard.IsNotNullSource(source);

            if (source.TryGetComponent<RuntimeId>(out var idCmp))
                throw new InvalidOperationException($"{nameof(RuntimeId).Humanize()} already exists.");

            idCmp = source.AddComponent<RuntimeId>();

            idCmp.Reflect()
                 .Cache()
                 .WithName(nameof(RuntimeId.Id))
                 .WithArguments(id)
                 .SetPropertyValue();

            return idCmp;
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

                    var attributes = Do.Collect(reqAttribute,
                        static reqAttribute =>
                        {
                            return reqAttribute.TypesToArray()
                                .Select(x => x.GetCustomAttribute<RequireComponent>())
                                .ToArray();
                        });

                    return attributes.Count;
                });

            using var _ = HashSetPool<Type>.Get(out var componentTypeSet);
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

        public static Maybe<GameObject> FindByHierarchyPath(string path, bool includeInactive = false)
        {
            Guard.IsNotNullOrEmpty(path, nameof(path));

            var pathParts = Regex.Split(path, @"[/\\]");
            var goOption = GameObjectQuery.Scene.IncludeInactive(includeInactive).WithName(pathParts[0]).GameObject().Lax();
            GameObject? go = goOption.GetValue();

            foreach (var pathPart in pathParts.Skip(1))
            {
                if (!goOption.TryGetValue(out go))
                    return Maybe<GameObject>.None;

                goOption = go.Q().WithName(pathPart).ChildrenGameObject().Lax();
            }

            return go;
        }
    }
}

