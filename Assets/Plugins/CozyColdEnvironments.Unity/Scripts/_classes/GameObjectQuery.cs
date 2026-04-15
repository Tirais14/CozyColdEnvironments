using CCEnvs.Collections;
using CCEnvs.FuncLanguage;
using CCEnvs.Linq;
using CCEnvs.Pools;
using CCEnvs.Reflection;
using CCEnvs.TypeMatching;
using CCEnvs.Unity.UI;
using CommunityToolkit.Diagnostics;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using UnityEditor.Search;
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
        public GameObjectQuery RequireComponent(Type? componentType = null)
        {
            RequieredTypeFilter = componentType;

            return this;
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public GameObjectQuery RequireComponent<T>()
        {
            return RequireComponent(typeof(T));
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ComponentsEnumerator Components(Type? type = null)
        {
            return ComponentsInternal(type);
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ComponentsEnumerator<T> Components<T>()
        {
            return ComponentsInternal(typeof(T)).Cast<T>();
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Result<Component, object, (GameObjectQuery Query, Type ComponentType)> Component(Type type)
        {
            Guard.IsNotNull(type, nameof(type));

            var cmp = Components(type).FirstOrDefault();

            if (cmp == null)
                return new Result<Component, object, (GameObjectQuery Query, Type ComponentType)>(static args => args.Query.GetException("Component not found", args.ComponentType), (this, type));

            return new Result<Component, object, (GameObjectQuery Query, Type ComponentType)>(cmp);
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Result<T, object, GameObjectQuery> Component<T>()
        {
            var cmp = Components(typeof(T)).FirstOrDefault();

            if (cmp == null)
                return new Result<T, object, GameObjectQuery>(static @this => @this.GetException("Component not found", typeof(T)), this);

            return new Result<T, object, GameObjectQuery>(cmp.CastTo<T>());
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ComponentsEnumerator<IViewModel> Views(Type? type = null)
        {
            type ??= typeof(IView);

            return Components(type).Convert<IViewModel>();
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ComponentsEnumerator<T> Views<T>()
        {
            return Views(typeof(T)).Cast<T>();
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Result<IView> View(Type? type = null)
        {
            return Component(type ?? TypeofCache<IView>.Type).Cast<IView>();
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Result<T> View<T>()
            where T : IView
        {
            return View(TypeofCache<T>.Type).Cast<T>();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IEnumerable<IViewModel> ViewModels(Type? type = null)
        {
            return Components(TypeofCache<IView>.Type).ViewModels(type);
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IEnumerable<T> ViewModels<T>()
            where T : IViewModel
        {
            return Components(TypeofCache<IView>.Type).ViewModels(TypeofCache<T>.Type).Cast<T>();
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
        public Result<Transform, object, GameObjectQuery> Transform()
        {
            var transform = Transforms().FirstOrDefault();

            if (transform == null)
                return new Result<Transform, object, GameObjectQuery>(exceptionFactory: (@this) => @this.GetException("Transform not found", TypeofCache<Transform>.Type), this);

            return new Result<Transform, object, GameObjectQuery>(transform);
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
        public readonly GameObjectQuery ShallowClone() => this;

        public readonly bool IsMatchLayerMaskFilter(GameObject? go)
        {
            if (go == null)
                return false;

            return !LayerMaskFilter.HasValue
                   ||
                   (go.layer & 1 << LayerMaskFilter.Value) != 0;
        }

        public readonly bool IsMatchNameFilter(GameObject? go)
        {
            if (go == null)
                return false;

            return NameFilter is null
                   ||
                   go.name.Match(NameFilter, nameMatchSettings);
        }

        public readonly bool IsMatchTagFilter(GameObject? go)
        {
            if (go == null)
                return false;

            return TagFilter is null
                   ||
                   go.CompareTag(TagFilter);
        }

        public readonly bool IsMatchGUIDFilter(GameObject? go)
        {
            if (go == null)
                return false;

            return GUID.IsNullOrWhiteSpace()
                   ||
                   go.GetPersistentGuid() == GUID;
        }

        public readonly bool HasRequiredType(GameObject? go)
        {
            if (go == null)
                return false;

            return RequieredTypeFilter is null
                   ||
                   go.HasComponent(RequieredTypeFilter);
        }

        public readonly bool IsGameObjectMatch(GameObject? go)
        {
            return IsMatchLayerMaskFilter(go)
                   &&
                   IsMatchNameFilter(go)
                   &&
                   IsMatchTagFilter(go)
                   &&
                   IsMatchGUIDFilter(go)
                   &&
                   HasRequiredType(go);
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

        private IList<Component> GetComponentsFrom(Type? type)
        {
            if (Target == null)
            {
                List<Component>? cmps = null;

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
            else if (findMode == FindMode.Self)
            {
                List<Component>? cmps = null;
                Target.GetComponentsNonAlloc(type, ref cmps);

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
                        Target.GetComponentsNonAlloc(type, ref cmps);

                    foreach (var child in Target.transform)
                        ((Transform)child).GetComponentsNonAlloc(type, ref cmps);

                    return cmps ?? (IList<Component>)Array.Empty<Component>();
                }
                else
                    return CustomBfsChildSearch(Target, type);
            }
            else if (findMode == FindMode.InParents)
                return CustomParentSearch(Target, type);

            throw CC.ThrowHelper.InvalidOperationException(findMode, nameof(findMode));
        }

        private ComponentsEnumerator ComponentsInternal(Type? type)
        {
            IList<Component> components = GetComponentsFrom(type);

            return new ComponentsEnumerator(this, components);
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

        public struct ComponentsEnumerator 
            :
            IEnumerator<Component>, 
            IEnumerable<Component>,
            IGameObjectQueryEnumerator
        {
            internal readonly PooledObject<HashSet<GameObject>> excludedGameObjects;

            private readonly IList<Component> components;

            private int pointer;

            private GameObject currentGO;

            public Component Current { get; private set; }

            public readonly GameObjectQuery Query { get; }

            readonly object IEnumerator.Current => Current;

            public ComponentsEnumerator(
                in GameObjectQuery query,
                IList<Component> components
                )
            {
                CC.Guard.IsNotNull(components, nameof(components));

                this.Query = query;
                this.components = components;

                Current = null!;
                currentGO = null!;
                pointer = -1;
                excludedGameObjects = HashSetPool<GameObject>.Shared.Get();
            }

            public readonly ViewModelsEnumerator ViewModels(Type? viewModelType = null)
            {
                return new ViewModelsEnumerator(Cast<IView>(), viewModelType);
            }

            public readonly ModelsEnumerator Models(Type? modelType = null)
            {
                return new ModelsEnumerator(ViewModels(), modelType);
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

                    if (!Query.IsGameObjectMatch(currentGO))
                    {
                        excludedGameObjects.Value.Add(currentGO);
                        continue;
                    }

                    return true;
                }

                return false;
            }

            public readonly void Dispose() => excludedGameObjects.Dispose();

            public readonly IEnumerator<Component> GetEnumerator() => this;
            readonly IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

            public void Reset()
            {
                pointer = -1;
                excludedGameObjects.Value.Clear();
            }

            public readonly ComponentsEnumerator<T> Cast<T>()
            {
                return new ComponentsEnumerator<T>(this);
            }

            internal readonly bool IsExcludedGameObject()
            {
                return excludedGameObjects.Value.Contains(currentGO); 
            }
        }

        public struct ComponentsEnumerator<T>
            :
            IEnumerator<T>,
            IEnumerable<T>
        {
            private ComponentsEnumerator componentEtor;

            public T Current { readonly get; private set; }

            public readonly ComponentsEnumerator ComponentsEtor => componentEtor;

            public readonly GameObjectQuery Query => componentEtor.Query;

            readonly object IEnumerator.Current => Current!;

            public ComponentsEnumerator(ComponentsEnumerator componentsEtor)
            {
                this.componentEtor = componentsEtor;

                Current = default!;
            }

            public static implicit operator ComponentsEnumerator(ComponentsEnumerator<T> instance)
            {
                return instance.componentEtor;
            }

            public bool MoveNext()
            {
                if (!componentEtor.TryMoveNextStruct<ComponentsEnumerator, Component>(out var cmp))
                    return false;

                Current = cmp.CastTo<T>();
                return true;
            }

            public readonly void Dispose() => componentEtor.Dispose();

            public readonly void Reset() => componentEtor.Reset();

            public readonly IEnumerator<T> GetEnumerator() => this;
            readonly IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }

        public struct ViewModelsEnumerator 
            :
            IEnumerator<IViewModel>,
            IEnumerable<IViewModel>,
            IGameObjectQueryEnumerator
        {
            private readonly Type? viewModelType;

            private ComponentsEnumerator<IView> viewsEtor;

            public IViewModel Current { readonly get; private set; }

            public readonly ComponentsEnumerator<IView> ViewsEtor => viewsEtor;

            public readonly GameObjectQuery Query => viewsEtor.Query;

            internal readonly HashSet<GameObject> excludedGameObjects => viewsEtor.ComponentsEtor.excludedGameObjects.Value;

            readonly object IEnumerator.Current => Current;

            public ViewModelsEnumerator(
                in ComponentsEnumerator<IView> viewsEtor, 
                Type? viewModelType
                )
            {
                this.viewsEtor = viewsEtor;
                this.viewModelType = viewModelType;

                Current = null!;
            }

            public readonly ViewModelsEnumerator<T> Cast<T>()
                where T : IViewModel
            {
                return new ViewModelsEnumerator<T>(this);
            }

            public bool MoveNext()
            {
                IViewModel? viewModel;

                var loopFuse = LoopFuse.Create();

                bool hasViewModelType = viewModelType is not null;

                while (viewsEtor.TryMoveNextStruct<ComponentsEnumerator<IView>, IView>(out var view)
                       &&
                       loopFuse.MoveNext())
                {
                    viewModel = view.ViewModel;

                    if (!hasViewModelType || viewModel.IsNotInstanceOfType(viewModelType!))
                        continue;

                    if (viewModel.Is<Component>(out var cmp))
                    {
                        var viewGO = cmp.gameObject;

                        if (excludedGameObjects.Contains(viewGO)
                            ||
                            !Query.IsGameObjectMatch(viewGO))
                        {
                            excludedGameObjects.Add(viewGO);
                            continue;
                        }
                    }

                    Current = viewModel!;
                    return true;
                }

                return false;
            }

            public readonly void Dispose() => viewsEtor.Dispose();

            public readonly void Reset() => viewsEtor.Reset();

            public readonly IEnumerator<IViewModel> GetEnumerator() => this;
            readonly IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }

        public struct ViewModelsEnumerator<T>
            :
            IEnumerator<T>,
            IEnumerable<T>

            where T : IViewModel
        {
            private ViewModelsEnumerator viewModelsEtor;

            public T Current { readonly get; private set; }

            public readonly ViewModelsEnumerator ViewModelsEtor => viewModelsEtor;

            public readonly GameObjectQuery Query => viewModelsEtor.Query;

            readonly object IEnumerator.Current => Current;

            public ViewModelsEnumerator(ViewModelsEnumerator viewModelsEtor)
            {
                this.viewModelsEtor = viewModelsEtor;

                Current = default!;
            }

            public static implicit operator ViewModelsEnumerator(ViewModelsEnumerator<T> instance)
            {
                return instance.viewModelsEtor;
            }

            public bool MoveNext()
            {
                if (!viewModelsEtor.TryMoveNextStruct<ViewModelsEnumerator, IViewModel>(out var viewModel))
                    return false;

                Current = (T)viewModel;
                return true;
            }

            public readonly void Dispose() => viewModelsEtor.Dispose();

            public readonly void Reset() => viewModelsEtor.Reset();

            public readonly IEnumerator<T> GetEnumerator() => this;
            readonly IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }

        public struct ModelsEnumerator 
            :
            IEnumerator<object>,
            IEnumerable<object>
        {
            internal readonly HashSet<GameObject> excludedGameObjects => ViewModelsEtor.excludedGameObjects;

            private readonly Type? modelType;

            private ViewModelsEnumerator viewModelsEtor;

            public object Current { get; private set; }

            public readonly ViewModelsEnumerator ViewModelsEtor => viewModelsEtor;

            public readonly GameObjectQuery Query => viewModelsEtor.Query;

            public ModelsEnumerator(
                in ViewModelsEnumerator viewModelsEtor,
                Type? modelType
                )
            {
                this.viewModelsEtor = viewModelsEtor;
                this.modelType = modelType;

                Current = default!;
            }

            public readonly ModelsEnumerator<T> Cast<T>()
            {
                return new ModelsEnumerator<T>(this);
            }

            public bool MoveNext()
            {
                var loopFuse = LoopFuse.Create();

                object? model;

                bool hasModelType = modelType is not null;

                while (viewModelsEtor.TryMoveNextStruct<ViewModelsEnumerator, IViewModel>(out var viewModel)
                       &&
                       loopFuse.MoveNext())
                {
                    model = viewModel.Model;

                    if (!hasModelType || model.IsNotInstanceOfType(modelType!))
                        continue;

                    if (model.Is<Component>(out var cmp))
                    {
                        var modelGO = cmp.gameObject;

                        if (excludedGameObjects.Contains(modelGO)
                            ||
                            !Query.IsGameObjectMatch(modelGO))
                        {
                            excludedGameObjects.Add(modelGO);
                            continue;
                        }
                    }

                    Current = model!;
                    return true;
                }

                return false;
            }

            public readonly void Dispose() => viewModelsEtor.Dispose();

            public readonly void Reset() => viewModelsEtor.Reset();

            public readonly IEnumerator<object> GetEnumerator() => this;
            readonly IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }

        public struct ModelsEnumerator<T>
            :
            IEnumerator<T>,
            IEnumerable<T>
        {
            private ModelsEnumerator modelsEtor;

            public T Current { readonly get; private set; }

            public readonly ModelsEnumerator ModelsEtor => modelsEtor;

            public readonly GameObjectQuery Query => modelsEtor.Query;

            readonly object IEnumerator.Current => Current!;

            public ModelsEnumerator(ModelsEnumerator modelsEtor)
            {
                this.modelsEtor = modelsEtor;

                Current = default!;
            }

            public static implicit operator ModelsEnumerator(ModelsEnumerator<T> instance)
            {
                return instance.modelsEtor;
            }

            public bool MoveNext()
            {
                if (!modelsEtor.TryMoveNextStruct<ModelsEnumerator, object>(out var model))
                    return false;

                Current = (T)model;
                return true;
            }

            public readonly void Dispose() => modelsEtor.Dispose();

            public readonly void Reset() => modelsEtor.Reset();

            public readonly IEnumerator<T> GetEnumerator() => this;
            readonly IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
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

