using CCEnvs.Collections;
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
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.SceneManagement;
using ZLinq;

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
        public StringMatchSettings nameFilterSettings { get; set; }
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

        #region Setup

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
                nameFilterSettings &= ~StringMatchSettings.Partial;
            else
                nameFilterSettings |= StringMatchSettings.Partial;

            if (ignoreCase)
                nameFilterSettings |= StringMatchSettings.IgnoreCase;
            else
                nameFilterSettings &= ~StringMatchSettings.IgnoreCase;

            return this;
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public GameObjectQuery WithNameMatchSettings(StringMatchSettings value = StringMatchSettings.Default)
        {
            nameFilterSettings = value;
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
        public GameObjectQuery Reset()
        {
            Target = default!;
            settings = Settings.Default;
            nameFilterSettings = StringMatchSettings.Default;
            findMode = FindMode.Self;
            sortMode = FindObjectsSortMode.None;
            NameFilter = null;
            TagFilter = null;
            LayerMaskFilter = default;
            RequieredTypeFilter = null;
            GUID = null;

            return this;
        }

        #endregion Setup

        #region Components

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly ComponentsEnumerator Components(Type? type = null)
        {
            IList<Component> components = GetComponents(type);
            return new ComponentsEnumerator(this, components);
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly ComponentsEnumerator<T> Components<T>()
        {
            return Components(typeof(T)).Cast<T>();
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly Result<Component, object, (GameObjectQuery Query, Type ComponentType)> Component(Type type)
        {
            Guard.IsNotNull(type, nameof(type));

            var cmp = Components(type).FirstOrDefaultStruct<Component, ComponentsEnumerator>();

            if (cmp == null)
                return new Result<Component, object, (GameObjectQuery Query, Type ComponentType)>(static args => args.Query.GetException("Component not found", args.ComponentType), (this, type));

            return new Result<Component, object, (GameObjectQuery Query, Type ComponentType)>(cmp);
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly Result<T, object, GameObjectQuery> Component<T>()
        {
            var cmp = Components<T>().FirstOrDefaultStruct<T, ComponentsEnumerator<T>>();

            if (cmp == null)
                return new Result<T, object, GameObjectQuery>(static @this => @this.GetException("Component not found", typeof(T)), this);

            return new Result<T, object, GameObjectQuery>(cmp);
        }

        #endregion Components

        #region Views

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly ComponentsEnumerator<IView> Views(Type? type = null)
        {
            type ??= typeof(IView);

            return Components(type).Cast<IView>();
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly ComponentsEnumerator<T> Views<T>()
            where T : IView
        {
            return Components(typeof(T)).Cast<T>();
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly Result<IView, object, (GameObjectQuery Query, Type ViewType)> View(Type? type = null)
        {
            type ??= TypeofCache<IView>.Type;

            var view = Views(type).FirstOrDefaultStruct<IView, ComponentsEnumerator<IView>>();

            if (view.IsNull())
                return new Result<IView, object, (GameObjectQuery Query, Type ViewType)>(static args => args.Query.GetException("View not found", args.ViewType), (this, type));

            return new Result<IView, object, (GameObjectQuery Query, Type ViewType)>(view); 
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly Result<T, object, GameObjectQuery> View<T>()
            where T : IView
        {
            var view = Views<T>().FirstOrDefaultStruct<T, ComponentsEnumerator<T>>();

            if (view.IsNull())
                return new Result<T, object, GameObjectQuery>(static @this => @this.GetException("View not found", typeof(T)), this);

            return new Result<T, object, GameObjectQuery>(view);
        }

        #endregion Views

        #region ViewModels

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly ViewModelsEnumerator ViewModels(Type? type = null)
        {
            return Components(TypeofCache<IView>.Type).ViewModels(type);
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly ViewModelsEnumerator<T> ViewModels<T>()
            where T : IViewModel
        {
            return Components(TypeofCache<IView>.Type).ViewModels(typeof(T)).Cast<T>();
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly Result<IViewModel, object, (GameObjectQuery Query, Type ViewModelType)> ViewModel(Type? type)
        {
            type ??= TypeofCache<IViewModel>.Type;

            var viewModel = ViewModels(type).FirstOrDefaultStruct<IViewModel, ViewModelsEnumerator>();

            if (viewModel.IsNull())
                return new Result<IViewModel, object, (GameObjectQuery Query, Type ViewModelType)>(static args => args.Query.GetException("View model not found", args.ViewModelType), (this, type));

            return new Result<IViewModel, object, (GameObjectQuery Query, Type ViewModelType)>(viewModel);
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly Result<T, object, GameObjectQuery> ViewModel<T>()
            where T : IViewModel
        {
            var viewModel = ViewModels<T>().FirstOrDefaultStruct<T, ViewModelsEnumerator<T>>();

            if (viewModel.IsNull())
                return new Result<T, object, GameObjectQuery>(static @this => @this.GetException("View model not found", typeof(T)), this);

            return new Result<T, object, GameObjectQuery>(viewModel);
        }

        #endregion ViewModels

        #region Models

        /// <summary>
        /// Also include <see cref="Components(Type?)"/>
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly ModelsEnumerator Models(Type? type = null, bool includeComponents = true)
        {
            return Components().Models(type, includeComponents);
        }

        /// <inheritdoc cref="Models"/>
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly ModelsEnumerator<T> Models<T>(bool includeComponents = true)
        {
            return Components().Models(typeof(T), includeComponents).Cast<T>();
        }

        /// <inheritdoc cref="Models"/>
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly Result<object, object, (GameObjectQuery Query, Type ModelType)> Model(Type? type, bool includeComponents = true)
        {
            type ??= TypeofCache<object>.Type;

            var model = Models(type, includeComponents).FirstOrDefaultStruct<object, ModelsEnumerator>();

            if (model.IsNull())
                return new Result<object, object, (GameObjectQuery Query, Type ModelType)>(static args => args.Query.GetException("Model not found", args.ModelType), (this, type));

            return new Result<object, object, (GameObjectQuery Query, Type ModelType)>(model);
        }

        /// <inheritdoc cref="Models"/>
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly Result<T, object, GameObjectQuery> Model<T>(bool includeComponents = true)
        {
            var model = Models<T>(includeComponents).FirstOrDefaultStruct<T, ModelsEnumerator<T>>();

            if (model.IsNull())
                return new Result<T, object, GameObjectQuery>(static @this => @this.GetException("Model not found", typeof(T)), this);

            return new Result<T, object, GameObjectQuery>(model);
        }

        #endregion Models

        #region Transforms

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly ComponentsEnumerator<Transform> Transforms() => Components<Transform>();

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly Result<Transform, object, GameObjectQuery> Transform()
        {
            var transform = Transforms().FirstOrDefaultStruct<Transform, ComponentsEnumerator<Transform>>();

            if (transform == null)
                return new Result<Transform, object, GameObjectQuery>(exceptionFactory: (@this) => @this.GetException("Transform not found", TypeofCache<Transform>.Type), this);

            return new Result<Transform, object, GameObjectQuery>(transform);
        }


        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ComponentsEnumerator<Transform> ChildrenTransforms()
        {
            return FromChildrens().ExcludeSelf().Transforms();
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Result<Transform, object, GameObjectQuery> ChildrenTransform()
        {
            return FromChildrens().ExcludeSelf().Transform();
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ComponentsEnumerator<Transform> ParentTranforms()
        {
            return FromParents().ExcludeSelf().Transforms();
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Result<Transform, object, GameObjectQuery> ParentTransform()
        {
            return FromParents().ExcludeSelf().Transform();
        }

        #endregion Transforms

        #region GameObjects

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly GameObjectsEnumerator GameObjects()
        {
            return new GameObjectsEnumerator(Transforms());
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly Result<GameObject, object, GameObjectQuery> GameObject()
        {
            var go = GameObjects().FirstOrDefaultStruct<GameObject, GameObjectsEnumerator>();

            if (go == null)
                return new Result<GameObject, object, GameObjectQuery>(static @this => @this.GetException("Game object not found", typeof(GameObject)), this);

            return new Result<GameObject, object, GameObjectQuery>(go);
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public GameObjectsEnumerator ChildrenGameObjects()
        {
            return FromChildrens().ExcludeSelf().GameObjects();
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Result<GameObject, object, GameObjectQuery> ChildrenGameObject()
        {
            return FromChildrens().ExcludeSelf().GameObject();
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public GameObjectsEnumerator ParentGameObjects()
        {
            return FromParents().ExcludeSelf().GameObjects();
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Result<GameObject, object, GameObjectQuery> ParentGameObject()
        {
            return FromParents().ExcludeSelf().GameObject();
        }

        #endregion GameObjects

        #region Filters

        public readonly bool HasDepthLimiter(Transform target)
        {
            if (DepthLimiterType is null)
                return false;

            return target.HasComponent(DepthLimiterType);
        }

        public readonly bool IsMatchLayerMaskFilter(GameObject? go)
        {
            if (go == null)
                return false;

            return !LayerMaskFilter.HasValue
                   ||
                   (LayerMaskFilter.Value & 1 << go.layer) != 0;
        }

        public readonly bool IsMatchNameFilter(GameObject? go)
        {
            if (go == null)
                return false;

            return NameFilter is null
                   ||
                   go.name.Match(NameFilter, nameFilterSettings);
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

        public readonly bool HasRequieredType(GameObject? go)
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
                   HasRequieredType(go);
        }

        #endregion Filters

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly Transform? RootTransform()
        {
            CC.Guard.IsNotNullTarget(Target);

            var root = Target.transform.root;

            if (root == Target.transform)
                return Target.transform;

            if (root == null)
                return null;

            if (Target.GetComponentInParent<RootMarker>().IsNotNull(out var rootMarker))
                return rootMarker.transform;

            return root;
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly GameObjectQuery ShallowClone() => this;

        #region SearchAlgorytms

        private readonly IList<Component> CustomParentSearch(
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

            List<Component>? cmps = null;

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

                if (HasDepthLimiter(current))
                    return cmps ?? (IList<Component>)Array.Empty<Component>();

                bool foundAny = current.GetComponentsNonAlloc(type, ref cmps) != 0;

                if (firstComponentsOnBranch
                    &&
                    foundAny
                    )
                {
                    return cmps ?? (IList<Component>)Array.Empty<Component>();
                }

                current = current.parent;
            }

            return cmps ?? (IList<Component>)Array.Empty<Component>();
        }

        private readonly IList<Component> CustomBfsChildSearch(
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

            using var toProcess = QueuePool<Transform>.Shared.Get();
            enqueueChilds(target.transform, includeInactive, toProcess.Value);

            Transform child;

            while (toProcess.Value.TryDequeue(out child))
            {
                if (HasDepthLimiter(child))
                    continue;

                bool cmpsFound = child.GetComponentsNonAlloc(type, ref cmps) != 0;

                if (firstComponentsOnBranch && cmpsFound)
                    continue;

                enqueueChilds(child, includeInactive, toProcess.Value);
            }

            return cmps ?? (IList<Component>)Array.Empty<Component>();

            static void enqueueChilds(
                Transform transform,
                bool includeInactive,
                Queue<Transform> toProcess
                )
            {
                Transform child;
                int childCount = transform.childCount;

                for (int i = 0; i < childCount; i++)
                {
                    child = transform.GetChild(i);

                    if (!includeInactive && !child.gameObject.activeInHierarchy)
                        continue;

                    toProcess.Enqueue(child);
                }
            }
        }

        private readonly IList<Component> GetComponents(Type? type)
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

                return cmps ?? (IList<Component>)Array.Empty<Component>();
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

        #endregion SearchAlgorytms

        private readonly GameObjectQueryException GetException(
            string msg, 
            Type? seekingComponentType = null
            )
        {
            return new GameObjectQueryException(
                Target,
                settings,
                nameFilterSettings,
                findMode,
                message: msg,
                seekingComponentType: seekingComponentType,
                name: NameFilter,
                tag: TagFilter,
                layer: LayerMaskFilter ?? -1,
                componentFilter: RequieredTypeFilter
                );
        }

        #region Enumerators

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

            public bool IsInitialized { get; }

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
                IsInitialized = true;
            }

            public readonly ViewModelsEnumerator ViewModels(Type? viewModelType = null)
            {
                return new ViewModelsEnumerator(Cast<IView>(), viewModelType);
            }

            public readonly ModelsEnumerator Models(Type? modelType = null, bool includeComponents = false)
            {
                if (includeComponents)
                    return new ModelsEnumerator(ViewModels(), this, modelType);

                return new ModelsEnumerator(ViewModels(), modelType);
            }

            public bool MoveNext()
            {
                if (!IsInitialized)
                    return false;

                var loopFuse = LoopFuse.Create();

                while (++pointer < components.Count && loopFuse.MoveNext())
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

                    if (hasViewModelType && viewModel.IsNotInstanceOfType(viewModelType!))
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
            private ComponentsEnumerator componentsEtor;

            public object Current { get; private set; }

            public readonly ViewModelsEnumerator ViewModelsEtor => viewModelsEtor;

            public readonly ComponentsEnumerator ComponentsEtor => componentsEtor;

            public readonly GameObjectQuery Query => viewModelsEtor.Query;

            public ModelsEnumerator(
                in ViewModelsEnumerator viewModelsEtor,
                Type? modelType
                )
            {
                this.viewModelsEtor = viewModelsEtor;
                this.modelType = modelType;

                Current = default!;
                componentsEtor = default;
            }

            public ModelsEnumerator(
                in ViewModelsEnumerator viewModelsEtor,
                in ComponentsEnumerator componentsEtor,
                Type? modelType
                )
                :
                this(viewModelsEtor, modelType)
            {
                this.componentsEtor = componentsEtor;
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

                Component? cmp = null;

                while (loopFuse.MoveNext())
                {
                    if (viewModelsEtor.TryMoveNextStruct<ViewModelsEnumerator, IViewModel>(out var viewModel))
                        model = viewModel.Model;
                    else if (componentsEtor.TryMoveNextStruct<ComponentsEnumerator, Component>(out cmp))
                        model = cmp;
                    else break;

                    if (hasModelType && model.IsNotInstanceOfType(modelType!))
                        continue;

                    if (cmp != null || model.Is(out cmp))
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

            public readonly void Dispose()
            {
                viewModelsEtor.Dispose();
                componentsEtor.Dispose();
            }

            public readonly void Reset()
            {
                viewModelsEtor.Reset();
                componentsEtor.Reset();
            }

            public readonly IEnumerator<object> GetEnumerator() => this;
            readonly IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

            private readonly bool ProcessComponent(Component cmp)
            {
                var modelGO = cmp.gameObject;

                if (excludedGameObjects.Contains(modelGO)
                    ||
                    !Query.IsGameObjectMatch(modelGO))
                {
                    excludedGameObjects.Add(modelGO);
                    return false;
                }

                return true;
            }
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

        public struct GameObjectsEnumerator : IEnumerator<GameObject>, IEnumerable<GameObject>
        {
            private ComponentsEnumerator<Transform> transformsEtor;

            public GameObject Current { readonly get; private set; }

            public readonly ComponentsEnumerator<Transform> TransformEtor => transformsEtor;

            public readonly GameObjectQuery Query => transformsEtor.Query;

            readonly object IEnumerator.Current => Current;

            public GameObjectsEnumerator(ComponentsEnumerator<Transform> transformsEtor)
            {
                this.transformsEtor = transformsEtor;

                Current = null!;
            }

            public bool MoveNext()
            {
                if (!transformsEtor.TryMoveNextStruct<ComponentsEnumerator<Transform>, Transform>(out var transform))
                    return false;

                Current = transform.gameObject;
                return true;
            }

            public readonly void Dispose() => transformsEtor.Dispose();

            public readonly void Reset() => transformsEtor.Reset();

            public readonly IEnumerator<GameObject> GetEnumerator() => this;
            readonly IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }

        #endregion Enumerators
    }

    public static class GameObjectSearchExtensions
    {
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static GameObjectQuery QueryTo(this GameObject source)
        {
            CC.Guard.IsNotNull(source, nameof(source));

            return GameObjectQuery.Create().SetTarget(source);
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static GameObjectQuery QueryTo(this Component source)
        {
            CC.Guard.IsNotNull(source, nameof(source));

            return GameObjectQuery.Create().SetTarget(source);
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

