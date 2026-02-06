using CCEnvs.Collections;
using CCEnvs.Diagnostics;
using CCEnvs.FuncLanguage;
using CCEnvs.Linq;
using CCEnvs.Reflection;
using CCEnvs.Unity.UI;
using CommunityToolkit.Diagnostics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;
using ZLinq;
using Object = UnityEngine.Object;

#nullable enable
namespace CCEnvs.Unity
{
    public record GameObjectQuery : IShallowCloneable<GameObjectQuery>
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
        public Maybe<GameObject> Target { get; set; } = null!;
        public Settings settings { get; set; }
        public StringMatchSettings stringMatchSettings { get; set; }
        public FindMode findMode { get; set; }
        public FindObjectsSortMode sortMode { get; set; }
        public Maybe<string> name { get; set; }
        /// <summary>
        /// <see cref="Settings.ByFullName"/>, <see cref="Settings.IgnoreCase"/> doesn' affect
        /// </summary>
        public Maybe<string> tag { get; set; }
        public Maybe<IntBitMask> layerMask { get; set; }
        public Maybe<Type> hasType { get; set; }
        public Maybe<Type> depthLimiter { get; set; }
        public Maybe<string> guid { get; set; }

        public GameObjectQuery()
        {
            Reset();
        }

        public static bool FilterByDepthLimiter(Transform target, Type limiterType, bool includeInactive)
        {
            return target.Q()
                .IncludeInactive(includeInactive)
                .Component(limiterType)
                .Lax().IsNone;
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public GameObjectQuery SetTarget(GameObject gameObject)
        {
            if (gameObject .IsNull())
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
            this.name = name;

            if (byFullName)
                stringMatchSettings &= ~StringMatchSettings.Partial;
            else
                stringMatchSettings |= StringMatchSettings.Partial;

            if (ignoreCase)
                stringMatchSettings |= StringMatchSettings.IgnoreCase;
            else
                stringMatchSettings &= ~StringMatchSettings.IgnoreCase;

            return this;
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public GameObjectQuery WithTag(string? tag = null)
        {
            this.tag = tag;

            return this;
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public GameObjectQuery WithLayerMask(int? layerMask = null)
        {
            if (layerMask is null)
                this.layerMask = Maybe<IntBitMask>.None;
            else
                this.layerMask = layerMask.Value.ToBitMask();

            return this;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public GameObjectQuery WithGuid(string? guid = null)
        {
            if (guid.IsNullOrWhiteSpace())
                this.guid = Maybe<string>.None;
            else
                this.guid = guid;

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
        public GameObjectQuery DepthLimiter(Type? type = null)
        {
            depthLimiter = type;

            return this;
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public GameObjectQuery DepthLimiter<T>()
        {
            return DepthLimiter(typeof(T));
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public GameObjectQuery Reset()
        {
            Target = default!;
            settings = Settings.Default;
            findMode = FindMode.Self;
            sortMode = FindObjectsSortMode.None;
            name = Maybe<string>.None;
            tag = Maybe<string>.None;
            layerMask = default;
            hasType = null;

            return this;
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public GameObjectQuery HasComponent(Type? componentType = null)
        {
            hasType = componentType;

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

            var includeInactiveState = settings.IsFlagSetted(Settings.IncludeInactive)
                        ?
                        FindObjectsInactive.Include
                        :
                        FindObjectsInactive.Exclude;

            return Object.FindObjectsByType<Transform>(includeInactiveState, sortMode)
                .AsValueEnumerable()
                .Select(static transform => transform.gameObject)
                .Select(go => ComponentsInternal(go, type))
                .SelectMany(static x => x)
                .AsEnumerable();
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

            return (Components(type).FirstOrDefault(), new GameObjectQueryException(
                Target.Raw,
                settings,
                findMode,
                message: $"Component not found.",
                seekingComponentType: type,
                name: name.Raw,
                tag: tag.Raw,
                layer: layerMask.GetValue(-1),
                componentFilter: hasType.Raw)
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
                   select view.viewModel.Raw into viewModel
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
                name: name.Raw,
                tag: tag.Raw,
                layer: layerMask.GetValue(-1),
                componentFilter: hasType.Raw)
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
                         select view.model into model
                         where model.IsSome
                         select model.GetValueUnsafe() into model
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
                name: name.Raw,
                tag: tag.Raw,
                layer: layerMask.GetValue(-1),
                componentFilter: hasType.Raw));
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
            return (Transforms().FirstOrDefault(), new GameObjectQueryException(
                Target.Raw,
                settings,
                findMode,
                message: "Transform not found.",
                seekingComponentType: typeof(Transform),
                name: name.Raw,
                tag: tag.Raw,
                layer: layerMask.GetValue(-1),
                componentFilter: hasType.Raw)
                );
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IEnumerable<GameObject> ChildrenGameObjects() => FromChildrens().ExcludeSelf().GameObjects();

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Result<GameObject> ChildrenGameObject()
        {
            return (ChildrenGameObjects().FirstOrDefault(), GetException("Game object not found", typeof(GameObject)));
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IEnumerable<Transform> ChildrenTransforms() => FromChildrens().ExcludeSelf().Transforms();

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Result<Transform> ChildrenTransform()
        {
            return (ChildrenTransforms().FirstOrDefault(), GetException("Transform not found", typeof(Transform)));
        }

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
                name: name.Raw,
                tag: tag.Raw,
                layer: layerMask.GetValue(-1),
                componentFilter: hasType.Raw)
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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Component AddComponent(Type type)
        {
            Guard.IsNotNull(type);

            if (type.IsNotType<Component>())
                throw new ArgumentException($"{nameof(type)} cannot be non derived from {typeof(Component)}.");

            if (Target.IsNone)
                throw new InvalidOperationException("Game object is none.");

            if (type.IsGenericType)
                throw new InvalidOperationException("Cannot add open generic component type to game object.");

            return Target.GetValueUnsafe().AddComponent(type);
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Component AddComponent<T>() where T : Component
        {
            return AddComponent(typeof(T)); 
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public GameObjectQuery ShallowClone()
        {
            return new GameObjectQuery(this);
        }

        protected virtual IEnumerable<Component> CustomParentSearch(
            GameObject target, 
            Type type)
        {
            bool includeInactive = settings.IsFlagSetted(Settings.IncludeInactive);
            bool hasDepthLimiter = depthLimiter.IsSome;
            bool firstComponentsOnBranch = settings.IsFlagSetted(Settings.FirstComponentsOnBranch);
            Transform current;

            if (!settings.IsFlagSetted(Settings.ExcludeSelf))
                current = target.transform;
            else
            {
                current = target.transform.parent;

                if (current.IsNull())
                    return Enumerable.Empty<Component>();
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

                if (hasDepthLimiter
                    &&
                    !FilterByDepthLimiter(current, depthLimiter.GetValueUnsafe(), includeInactive)
                    )
                    return cmps;

                bool foundAny = false;
                if (current.Q().IncludeInactive(includeInactive).Components(type).Let(out var t)
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

        protected virtual IEnumerable<Component> CustomBfsChildSearch(
            GameObject target,
            Type type
            )
        {
            bool includeInactive = settings.IsFlagSetted(Settings.IncludeInactive);
            var toProcess = new Queue<Transform>(getNextTransforms(target.transform, includeInactive));
            bool firstComponentsOnBranch = settings.IsFlagSetted(Settings.FirstComponentsOnBranch);
            bool hasDepthLimiter = depthLimiter.IsSome;

            List<Component> cmps;
            if (!settings.IsFlagSetted(Settings.ExcludeSelf))
            {
                cmps = new List<Component>();
                cmps.AddRange(target.Q().Components(type));
            }
            else if (target.transform.childCount == 0)
                return Enumerable.Empty<Component>();
            else
                cmps = new List<Component>();

            Transform child;
            while (toProcess.IsNotEmpty())
            {
                child = toProcess.Dequeue();

                if (hasDepthLimiter 
                    &&
                    !FilterByDepthLimiter(child, depthLimiter.GetValueUnsafe(), includeInactive)
                    )
                    continue;

                bool cmpsFound = addComponents(child, includeInactive, type, cmps);

                if (firstComponentsOnBranch && cmpsFound)
                    continue;

                foreach (var item in getNextTransforms(child, includeInactive))
                    toProcess.Enqueue(item);
            }

            return cmps;

            static IEnumerable<Transform> getNextTransforms(
                Transform tr,
                bool includeInactive)
            {
                foreach (var current in tr.AsValueEnumerable().Cast<Transform>())
                {
                    if (!includeInactive && !current.gameObject.activeSelf)
                        continue;

                    yield return current;
                }
            }

            static bool addComponents(
                Transform target,
                bool includeInactive,
                Type type,
                List<Component> cmps)
            {
                if (target.Q()
                    .IncludeInactive(includeInactive)
                    .Components(type)
                    .Let(out var t)
                    &&
                    t.IsNotEmpty())
                {
                    cmps.AddRange(t);
                    return true;
                }

                return false;
            }
        }

        protected virtual IEnumerable<Component> GetComponentsFrom(
            GameObject target,
            Type type,
            bool anyType
            )
        {
            bool isNotRecursive = settings.IsFlagSetted(Settings.NotRecursive);
            bool excludeSelf = settings.IsFlagSetted(Settings.ExcludeSelf);

            if (findMode == FindMode.Self)
            {
                return target.GetComponents<Component>()
                             .Where(cmp => anyType || cmp.IsInstanceOfType(type));
            }
            else if (findMode == FindMode.InChilds)
            {
                if (isNotRecursive)
                {
                    Transform targetTransform = target.transform;
                    var cmps = new List<Component>();
                    var childs = targetTransform.AsValueEnumerable().Cast<Transform>();

                    if (!excludeSelf)
                        childs.Prepend(targetTransform);

                    foreach (var child in childs)
                        cmps.AddRange(child.Q().Components(type));

                    return cmps;
                }
                else
                    return CustomBfsChildSearch(target, type);
            }
            else if (findMode == FindMode.InParents)
                return CustomParentSearch(target, type);

            throw CC.ThrowHelper.InvalidOperationException(findMode, nameof(findMode));
        }

        protected virtual IEnumerable<Component> ComponentsInternal(
            GameObject target,
            Type? type
            )
        {
            CC.Guard.IsNotNull(target, nameof(target));

            if (type == typeof(GameObject))
                throw new ArgumentException($"Type cannot be: {type.GetFullName()}.");

            bool anyType = type is null;
            type ??= typeof(Component);

            IEnumerable<Component> components = GetComponentsFrom(target, type, anyType);

            if (hasType.IsSome)
                components = components.Where(cmp =>
                    cmp.gameObject.QueryTo()
                    .Component(hasType.GetValueUnsafe())
                    .Lax()
                    .IsSome
                    );

            if (this.layerMask.TryGetValue(out var layerMask))
                components = components.Where(cmp => layerMask.ContainsFlag(cmp.gameObject.layer));

            if (this.name.TryGetValue(out var name))
                components = components.Where(cmp => cmp.gameObject.name.Match(name, stringMatchSettings));

            if (this.tag.TryGetValue(out var tag))
                components = components.Where(cmp => cmp.gameObject.CompareTag(tag));

            if (this.guid.TryGetValue(out var guid))
                components = components.Where(cmp => cmp.GetPersistentGuid().Has(guid));

            return components;
        }

        protected GameObjectQueryException GetException(string msg, Type? seekingComponentType = null)
        {
            return new GameObjectQueryException(
                Target.Raw,
                settings,
                findMode,
                message: msg,
                seekingComponentType: seekingComponentType,
                name: name.Raw,
                tag: tag.Raw,
                layer: layerMask.GetValue(-1),
                componentFilter: hasType.Raw);
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

