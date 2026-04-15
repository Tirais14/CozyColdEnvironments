using CCEnvs.Collections;
using CCEnvs.FuncLanguage;
using CCEnvs.Linq;
using CCEnvs.Pools;
using CCEnvs.Reflection;
using CCEnvs.Unity.UI;
using CommunityToolkit.Diagnostics;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.SceneManagement;
using ZLinq;
using Object = UnityEngine.Object;

#nullable enable
namespace CCEnvs.Unity
{
    public struct GameObjectQuery : IShallowCloneable<GameObjectQuery>
    {
        [Flags]
        public enum Settings
        {
            None,
            IncludeInactive = 1,
            ExcludeSelf = 2,
            /// <summary>
            /// Except in depth childrens from results
            /// </summary>
            NotRecursive = 4,
            CacheResult = 8,
            FirstComponentsOnBranch = 16,
            Default = None
        }

        public static GameObjectQuery Scene => new();

        /// <summary>
        /// May be null
        /// </summary>
        public GameObject? Target { get; set; }

        public Settings settings { get; set; }
        public StringMatchSettings nameMatchSettings { get; set; }
        public FindMode findMode { get; set; }
        public FindObjectsSortMode sortMode { get; set; }

        public string? NameFilter { get; set; }
        /// <summary>
        /// <see cref="Settings.ByFullName"/>, <see cref="Settings.IgnoreCase"/> doesn' affect
        /// </summary>
        public string? TagFilter { get; set; }
        public string? GUID { get; set; }

        public int? LayerMaskFilter { get; set; }

        public Type? RequieredTypeFilter { get; set; }
        public Type? DepthLimiterType { get; set; }

        public static GameObjectQuery Create()
        {
            return new GameObjectQuery().Reset();
        }

        public bool FilterByDepthLimiter(Transform target)
        {
            if (DepthLimiterType is null)
                return true;

            return target.GetComponent(DepthLimiterType) != null;
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public GameObjectQuery SetTarget(GameObject gameObject)
        {
            if (gameObject.IsNull())
            {
                this.PrintError($"{nameof(gameObject)} is null.");
                return this;
            }

            Target = gameObject;
            return this;
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public GameObjectQuery SetTarget(Component component)
        {
            if (component.IsNull())
            {
                this.PrintError($"{nameof(component)} is null.");
                return this;
            }

            Target = component.gameObject;
            return this;
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public GameObjectQuery IncludeInactive(bool state = true)
        {
            if (state)
                settings |= Settings.IncludeInactive;
            else
                settings &= ~Settings.IncludeInactive;

            return this;
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public GameObjectQuery ExcludeSelf(bool state = true)
        {
            if (state)
                settings |= Settings.ExcludeSelf;
            else
                settings &= ~Settings.ExcludeSelf;

            return this;
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public GameObjectQuery WithName(string? name = null, bool ignoreCase = false, bool byFullName = false)
        {
            this.NameFilter = name;

            if (byFullName)
                nameMatchSettings &= ~StringMatchSettings.Partial;
            else
                nameMatchSettings |= StringMatchSettings.Partial;

            if (ignoreCase)
                nameMatchSettings |= StringMatchSettings.IgnoreCase;
            else
                nameMatchSettings &= ~StringMatchSettings.IgnoreCase;

            return this;
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public GameObjectQuery WithNameMatchSettings(StringMatchSettings value = StringMatchSettings.Default)
        {
            nameMatchSettings = value;
            return this;
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public GameObjectQuery WithTag(string? value = null)
        {
            this.TagFilter = value;

            return this;
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public GameObjectQuery WithLayerMask(int? value = null)
        {
            LayerMaskFilter = value;

            return this;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public GameObjectQuery WithGuid(string? value = null)
        {
            GUID = value;

            return this;
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public GameObjectQuery FromSelf()
        {
            findMode = FindMode.Self;

            return this;
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public GameObjectQuery FromChildrens()
        {
            findMode = FindMode.InChilds;

            return this;
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public GameObjectQuery FromParents()
        {
            findMode = FindMode.InParents;

            return this;
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public GameObjectQuery NotRecursive(bool state = true)
        {
            if (state)
                settings |= Settings.NotRecursive;
            else
                settings &= ~Settings.NotRecursive;

            return this;
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public GameObjectQuery FirstComponentsOnBranch(bool state = true)
        {
            if (state)
                settings |= Settings.FirstComponentsOnBranch;
            else
                settings &= ~Settings.FirstComponentsOnBranch;

            return this;
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public GameObjectQuery SortByInstanceID(bool state = true)
        {
            if (state)
                sortMode = FindObjectsSortMode.InstanceID;
            else
                sortMode = FindObjectsSortMode.None;

            return this;
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public GameObjectQuery WithDepthLimiter(Type? type = null)
        {
            DepthLimiterType = type;

            return this;
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public GameObjectQuery WithDepthLimiter<T>()
        {
            return WithDepthLimiter(typeof(T));
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public GameObjectQuery Reset()
        {
            Target = default!;
            settings = Settings.Default;
            findMode = FindMode.Self;
            sortMode = FindObjectsSortMode.None;
            NameFilter = null;
            TagFilter = null;
            LayerMaskFilter = default;
            RequieredTypeFilter = null;

            return this;
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public GameObjectQuery HasComponent(Type? componentType = null)
        {
            RequieredTypeFilter = componentType;

            return this;
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public GameObjectQuery HasComponent<T>()
        {
            return HasComponent(typeof(T));
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IEnumerable<Component> Components(Type? type = null)
        {
            if (Target.TryGetValue(out var target))
                return ComponentsInternal(target, type);

            var includeInactiveState = settings.HasFlagT(Settings.IncludeInactive)
                        ?
                        FindObjectsInactive.Include
                        :
                        FindObjectsInactive.Exclude;

            return Object.FindObjectsByType<Transform>(includeInactiveState, sortMode)
#if ZLINQ_PLUGIN
                .AsValueEnumerable()
#endif
                .Select(static transform => transform.gameObject)
                .Select(go => ComponentsInternal(go, type))
                .SelectMany(static x => x)
#if ZLINQ_PLUGIN
                .AsEnumerable()
#endif
                ;
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IEnumerable<T> Components<T>()
        {
            return Components(typeof(T)).Cast<T>();
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Result<Component> Component(Type type)
        {
            Guard.IsNotNull(type, nameof(type));

            var cmp = Components(type).FirstOrDefault();

            if (cmp == null)
                return ()

            return (Components(type).FirstOrDefault(), new GameObjectQueryException(
                Target.Raw,
                settings,
                findMode,
                message: $"Component not found.",
                seekingComponentType: type,
                name: NameFilter.Raw,
                tag: TagFilter.Raw,
                layer: LayerMaskFilter.GetValue(-1),
                componentFilter: RequieredTypeFilter.Raw)
                );
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Result<T> Component<T>()
        {
            return Component(typeof(T)).Cast<T>();
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IEnumerable<IView> Views(Type? type = null)
        {
            type ??= typeof(IView);

            return Components(type).Cast<IView>();
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IEnumerable<T> Views<T>()
        {
            return Views(typeof(T)).Cast<T>();
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Result<IView> View(Type? type = null)
        {
            return Component(type ?? typeof(IView)).Cast<IView>();
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Result<T> View<T>()
            where T : IView
        {
            return View(typeof(T)).Cast<T>();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IEnumerable<IViewModel> ViewModels(Type? type = null)
        {
            type ??= typeof(IViewModel);

            bool anyType = type is null;

            return from view in Views(type)
                   select view.ViewModel into viewModel
                   where viewModel.IsNotNull()
                   where anyType || viewModel.IsInstanceOfType(type!)
                   select viewModel;
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IEnumerable<T> ViewModels<T>()
            where T : IViewModel
        {
            return ViewModels(typeof(T)).Cast<T>();
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Result<IViewModel> ViewModel(Type type)
        {
            Guard.IsNotNull(type, nameof(type));

            return (ViewModels(type).FirstOrDefault(), new GameObjectQueryException(
                Target.Raw,
                settings,
                findMode,
                message: "View model not found.",
                seekingComponentType: type,
                name: NameFilter.Raw,
                tag: TagFilter.Raw,
                layer: LayerMaskFilter.GetValue(-1),
                componentFilter: RequieredTypeFilter.Raw)
                );
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Result<T> ViewModel<T>()
            where T : IViewModel
        {
            return ViewModel(typeof(T)).Cast<T>();
        }

        /// <summary>
        /// Also include <see cref="Components(Type?)"/>
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IEnumerable<object> Models(Type? type = null, bool includeComponents = true)
        {
            bool anyType = type is null;
            type ??= typeof(object);

            var cmps = Components();

            var models = from view in cmps.OfType<IView>()
                         where view.ViewModel.IsNotNull()
                         select view.Model into model
                         where model.IsNotNull()
                         where anyType || model.IsInstanceOfType(type)
                         select model;

            if (includeComponents)
            {
                models = cmps.Where(cmp => anyType || cmp.IsInstanceOfType(type))
                             .Concat(models);
            }

            return models;
        }

        /// <inheritdoc cref="Models"/>
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IEnumerable<T> Models<T>(bool includeComponents = true)
        {
            return Models(typeof(T), includeComponents).Cast<T>();
        }

        /// <inheritdoc cref="Models"/>
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Result<object> Model(Type type, bool includeComponents = true)
        {
            Guard.IsNotNull(type, nameof(type));

            return (Models(type, includeComponents).FirstOrDefault(), new GameObjectQueryException(
                Target.Raw,
                settings,
                findMode,
                message: "Model not found.",
                seekingComponentType: type,
                name: NameFilter.Raw,
                tag: TagFilter.Raw,
                layer: LayerMaskFilter.GetValue(-1),
                componentFilter: RequieredTypeFilter.Raw));
        }

        /// <inheritdoc cref="Models"/>
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Result<T> Model<T>(bool includeComponents = true)
        {
            return Model(typeof(T), includeComponents).Cast<T>();
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IEnumerable<Transform> Transforms() => Components<Transform>();

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Result<Transform> Transform()
        {
            var transform = Transforms().FirstOrDefault();

            if (transform == null)
                return (null, GetException("Transform not found", TypeofCache<Transform>.Type));

            return (transform, null);
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IEnumerable<GameObject> ChildrenGameObjects() => FromChildrens().ExcludeSelf().GameObjects();

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IEnumerable<Transform> ChildrenTransforms() => FromChildrens().ExcludeSelf().Transforms();

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IEnumerable<GameObject> ParentGameObjects() => FromParents().ExcludeSelf().GameObjects();

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IEnumerable<Transform> ParentTranforms() => FromParents().ExcludeSelf().Transforms();

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IEnumerable<GameObject> GameObjects()
        {
            return Transforms().Select(x => x.gameObject);
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Result<GameObject> GameObject()
        {
            return (GameObjects().FirstOrDefault(), new GameObjectQueryException(
                Target.Raw,
                settings,
                findMode,
                message: "Game object not found.",
                seekingComponentType: typeof(GameObject),
                name: NameFilter.Raw,
                tag: TagFilter.Raw,
                layer: LayerMaskFilter.GetValue(-1),
                componentFilter: RequieredTypeFilter.Raw)
                );
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Either<Transform, RootMarker> RootRaw()
        {
            var marker = FromParents().IncludeInactive()
                                   .Component<RootMarker>()
                                   .Lax();

            return (marker.Map(x => x.transform).GetValue(Target.GetValueUnsafe().transform.root), marker.Raw);
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Transform RootTransform()
        {
            return RootRaw()
                .Match(
                    Right: r => r.transform,
                    Left: l => l)
                .GetValue()
                .AsObsolete<Transform>()
                .GetValue(Target.GetValueUnsafe().transform);
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public GameObjectQuery ShallowClone()
        {
            throw new NotImplementedException();
        }

        private IList<Component> CustomParentSearch(
            GameObject target,
            Type? type
            )
        {
            bool includeInactive = settings.HasFlagT(Settings.IncludeInactive);
            bool firstComponentsOnBranch = settings.HasFlagT(Settings.FirstComponentsOnBranch);
            Transform current;

            if (!settings.HasFlagT(Settings.ExcludeSelf))
                current = target.transform;
            else
            {
                current = target.transform.parent;

                if (current.IsNull())
                    return Array.Empty<Component>();
            }

            var cmps = new List<Component>();

            while (current.IsNotNull())
            {
                if (!includeInactive
                    &&
                    !current.gameObject.activeSelf
                    )
                {
                    current = current.parent;
                    continue;
                }

                if (!FilterByDepthLimiter(current))
                    return cmps;

                bool foundAny = false;
                if (current.Q().IncludeInactive(includeInactive).Components(type).IsNotNull(out var t)
                    &&
                    t.IsNotEmpty())
                {
                    foundAny = true;
                    cmps.AddRange(t);
                }

                if (firstComponentsOnBranch
                    &&
                    foundAny
                    )
                    return cmps;

                current = current.parent;
            }

            return cmps;
        }

        private IList<Component> CustomBfsChildSearch(
            GameObject target,
            Type? type
            )
        {
            bool includeInactive = settings.HasFlagT(Settings.IncludeInactive);
            bool firstComponentsOnBranch = settings.HasFlagT(Settings.FirstComponentsOnBranch);
            bool excludeSelf = settings.HasFlagT(Settings.ExcludeSelf);

            List<Component>? cmps = null;

            if (target.transform.childCount == 0)
                return Array.Empty<Component>();

            if (!excludeSelf)
                target.GetComponentsNonAlloc(type, ref cmps);

            var toProcess = new Queue<Transform>();
            enqueueChilds(target.transform, includeInactive, toProcess);

            Transform child;

            while (toProcess.Count != 0)
            {
                child = toProcess.Dequeue();

                if (!FilterByDepthLimiter(child))
                    continue;

                bool cmpsFound = target.GetComponentsNonAlloc(type, ref cmps) != 0;

                if (firstComponentsOnBranch && cmpsFound)
                    continue;

                enqueueChilds(child, includeInactive, toProcess);
            }

            return cmps ?? (IList<Component>)Array.Empty<Component>();

            static void enqueueChilds(
                Transform transform,
                bool includeInactive,
                Queue<Transform> toProcess
                )
            {
                Transform child;

                foreach (var childUntyped in transform)
                {
                    child = (Transform)childUntyped;

                    if (!includeInactive && !child.gameObject.activeSelf)
                        continue;

                    toProcess.Enqueue(child);
                }
            }
        }

        private IList<Component> GetComponentsFrom(
            GameObject target,
            Type? type
            )
        {
            if (findMode == FindMode.Self)
            {
                List<Component>? cmps = null;

                if (Target == null)
                {
                    int sceneCount = SceneManager.sceneCount;
                    using var sceneRoots = new PooledList<GameObject>(null);

                    Scene scene;

                    for (int i = 0; i < sceneCount; i++)
                    {
                        scene = SceneManager.GetSceneAt(i);
                        scene.GetRootGameObjects(sceneRoots);
                    }

                    bool includeInactive = settings.HasFlagT(Settings.IncludeInactive);

                    for (int i = 0; i < sceneRoots.Value.Count; i++)
                    {
                        sceneRoots[i].GetComponentsInChildrenNonAlloc(
                            type,
                            ref cmps,
                            includeInactive
                            );
                    }
                }
                else
                    target.GetComponentsNonAlloc(type, ref cmps);

                return cmps ?? (IList<Component>)Array.Empty<Component>();
            }
            else if (findMode == FindMode.InChilds)
            {
                bool isNotRecursive = settings.HasFlagT(Settings.NotRecursive);
                bool excludeSelf = settings.HasFlagT(Settings.ExcludeSelf);

                if (isNotRecursive)
                {
                    List<Component>? cmps = null;

                    if (!excludeSelf)
                        target.GetComponentsNonAlloc(type, ref cmps);

                    foreach (var child in target.transform)
                        ((Transform)child).GetComponentsNonAlloc(type, ref cmps);

                    return cmps ?? (IList<Component>)Array.Empty<Component>();
                }
                else
                    return CustomBfsChildSearch(target, type);
            }
            else if (findMode == FindMode.InParents)
                return CustomParentSearch(target, type);

            throw CC.ThrowHelper.InvalidOperationException(findMode, nameof(findMode));
        }

        private ComponentsEnumerator ComponentsInternal(
            GameObject target,
            Type? type
            )
        {
            IList<Component> components = GetComponentsFrom(target, type);

            return new ComponentsEnumerator(this, components);
        }

        private ComponentsEnumerator<T> ComponentsInternal<T>(
            GameObject target,
            Type? type
            )
        {
            IList<Component> components = GetComponentsFrom(target, type);

            return new ComponentsEnumerator<T>(new ComponentsEnumerator(this, components));
        }

        private readonly GameObjectQueryException GetException(
            string msg, 
            Type? seekingComponentType = null
            )
        {
            return new GameObjectQueryException(
                Target,
                settings,
                findMode,
                message: msg,
                seekingComponentType: seekingComponentType,
                name: NameFilter,
                tag: TagFilter,
                layer: LayerMaskFilter ?? -1,
                componentFilter: RequieredTypeFilter
                );
        }

        public struct ComponentsEnumerator : IEnumerator<Component>
        {
            private readonly GameObjectQuery query;

            private readonly IList<Component> components;

            private readonly PooledObject<HashSet<GameObject>> excludedGameObjects;

            private int pointer;

            private GameObject currentGO;

            public Component Current { get; private set; }

            readonly object IEnumerator.Current => Current;

            public ComponentsEnumerator(
                in GameObjectQuery query,
                IList<Component> components
                )
            {
                CC.Guard.IsNotNull(components, nameof(components));

                this.query = query;
                this.components = components;

                Current = null!;
                currentGO = null!;
                pointer = -1;
                excludedGameObjects = HashSetPool<GameObject>.Shared.Get();
            }

            public bool MoveNext()
            {
                var loopFuse = LoopFuse.Create();

                while (++pointer >= components.Count && loopFuse.MoveNext())
                {
                    Current = components[pointer];
                    currentGO = Current.gameObject;

                    if (IsExcludedGameObject())
                        continue;

                    if (!IsMatchLayerMaskFilter()
                        ||
                        !IsMatchNameFilter()
                        ||
                        !IsMatchTagFilter()
                        ||
                        !IsMatchGUIDFilter()
                        ||
                        !HasRequiredType())
                    {
                        excludedGameObjects.Value.Add(currentGO);
                        continue;
                    }

                    return true;
                }

                return false;
            }

            public readonly void Dispose() => excludedGameObjects.Dispose();

            public void Reset()
            {
                pointer = -1;
                excludedGameObjects.Value.Clear();
            }

            public readonly ComponentsEnumerator<T> Convert<T>()
            {
                return new ComponentsEnumerator<T>(this);
            }

            private readonly bool IsExcludedGameObject()
            {
                return excludedGameObjects.Value.Contains(currentGO); 
            }

            private readonly bool IsMatchLayerMaskFilter()
            {
                return !query.LayerMaskFilter.HasValue
                       ||
                       (currentGO.layer & 1 << query.LayerMaskFilter.Value) != 0;
            }

            private readonly bool IsMatchNameFilter()
            {
                return query.NameFilter is null
                       ||
                       currentGO.name.Match(query.NameFilter, query.nameMatchSettings);
            }

            private readonly bool IsMatchTagFilter()
            {
                return query.TagFilter is null
                       ||
                       currentGO.CompareTag(query.TagFilter);
            }

            private readonly bool IsMatchGUIDFilter()
            {
                return query.GUID.IsNullOrWhiteSpace()
                       ||
                       currentGO.GetPersistentGuid() == query.GUID;
            }

            private readonly bool HasRequiredType()
            {
                return query.RequieredTypeFilter is null
                       ||
                       currentGO.HasComponent(query.RequieredTypeFilter);
            }
        }

        public struct ComponentsEnumerator<T> : IEnumerator<T>
        {
            private ComponentsEnumerator core;

            public T Current { get; private set; }

            readonly object IEnumerator.Current => Current!;

            public ComponentsEnumerator(ComponentsEnumerator core)
            {
                this.core = core;

                Current = default!;
            }

            public static implicit operator ComponentsEnumerator(ComponentsEnumerator<T> instance)
            {
                return instance.core;
            }

            public bool MoveNext()
            {
                if (!core.TryMoveNextStruct<ComponentsEnumerator, Component>(out var current))
                    return false;

                Current = current.CastTo<T>();
                return true;
            }

            public readonly void Dispose() => core.Dispose();

            public readonly void Reset() => core.Reset();
        }
    }

    public static class GameObjectSearchExtensions
    {
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static GameObjectQuery QueryTo(this GameObject source)
        {
            CC.Guard.IsNotNull(source, nameof(source));

            return new GameObjectQuery().SetTarget(source);
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static GameObjectQuery QueryTo(this Component source)
        {
            CC.Guard.IsNotNull(source, nameof(source));

            return new GameObjectQuery().SetTarget(source);
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static GameObjectQuery Q(this GameObject source)
        {
            return source.QueryTo();
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static GameObjectQuery Q(this Component source)
        {
            return source.QueryTo();
        }
    }
}

