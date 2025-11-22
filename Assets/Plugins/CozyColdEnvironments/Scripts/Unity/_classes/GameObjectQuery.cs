using CCEnvs.Diagnostics;
using CCEnvs.FuncLanguage;
using CCEnvs.Reflection;
using CCEnvs.Unity.UI;
using CommunityToolkit.Diagnostics;
using Cysharp.Threading.Tasks.Triggers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;
using Object = UnityEngine.Object;

#nullable enable
namespace CCEnvs.Unity
{
    public record GameObjectQuery
    {
        [Flags]
        public enum Settings
        {
            None,
            Reusable,
            IncludeInactive = 2,
            ExcludeSelf = 4,
            /// <summary>
            /// Except in depth childrens from results
            /// </summary>
            NotRecursive = 8,
            Default = None
        }

        public readonly static GameObjectQuery Instance = new();

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
        public int? layerMask { get; set; }
        public Maybe<Type> mustContainsType { get; set; }

        public GameObjectQuery()
        {
            Reset();
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public GameObjectQuery From(GameObject gameObject)
        {
            if (gameObject == null)
            {
                this.PrintError($"{nameof(gameObject)} is null.");
                return this;
            }

            Target = gameObject;
            return this;
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public GameObjectQuery From(Component component)
        {
            if (component == null)
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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public GameObjectQuery Reusable(bool state = true)
        {
            if (this == Instance)
                return this;

            if (state)
                settings |= Settings.Reusable;
            else
                settings &= ~Settings.Reusable;

            return this;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public GameObjectQuery ByFullName(bool state = true)
        {
            if (state)
                stringMatchSettings &= ~StringMatchSettings.Partial;
            else
                stringMatchSettings |= StringMatchSettings.Partial;

            return this;
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public GameObjectQuery ByName(string? name = null)
        {
            this.name = name;

            return this;
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public GameObjectQuery ByTag(string? tag = null)
        {
            this.tag = tag;

            return this;
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public GameObjectQuery ByLayerMask(int? layerMask = null)
        {
            this.layerMask = layerMask;

            return this;
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public GameObjectQuery BySelf()
        {
            findMode = FindMode.Self;

            return this;
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public GameObjectQuery ByChildren()
        {
            findMode = FindMode.InChilds;

            return this;
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public GameObjectQuery ByParent()
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
        public GameObjectQuery Reset()
        {
            Target = default!;
            settings = Settings.Default;
            findMode = FindMode.Self;
            sortMode = FindObjectsSortMode.None;
            name = Maybe<string>.None;
            tag = Maybe<string>.None;
            layerMask = default;
            mustContainsType = null;

            return this;
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public GameObjectQuery MustContainType(Type? componentType = null)
        {
            mustContainsType = componentType;

            return this;
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public GameObjectQuery MustContainType<T>()
        {
            return MustContainType(typeof(T));
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IEnumerable<object> Components(Type? type = null)
        {
            return Target.BiMap(
                some: target => ComponentsInternal(target, type),
                none: () =>
                {
                    var includeInactiveState = settings.IsFlagSetted(Settings.IncludeInactive)
                        ?
                        FindObjectsInactive.Include
                        :
                        FindObjectsInactive.Exclude;

                    return Object.FindObjectsByType<Transform>(includeInactiveState, sortMode)
                        .Select(transform => transform.gameObject)
                        .Select(go => ComponentsInternal(go, type))
                        .SelectMany(x => x);
                })
                .GetValueUnsafe();
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IEnumerable<T> Components<T>()
        {
            return Components(typeof(T)).Cast<T>();
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Result<object> Component(Type type)
        {
            Guard.IsNotNull(type, nameof(type));

            return (Components(type).FirstOrDefault(), new GameObjectAppealException(
                Target.Raw,
                settings,
                findMode,
                message: $"Component not found.",
                seekingComponentType: type,
                name: name.Raw,
                tag: tag.Raw,
                layer: layerMask ?? -1,
                componentFilter: mustContainsType.Raw)
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
            where T : IView
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

            return (ViewModels(type).FirstOrDefault(), new GameObjectAppealException(
                Target.Raw,
                settings,
                findMode,
                message: "View model not found.",
                seekingComponentType: type,
                name: name.Raw,
                tag: tag.Raw,
                layer: layerMask ?? -1,
                componentFilter: mustContainsType.Raw)
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

            return (Models(type, includeComponents).FirstOrDefault(), new GameObjectAppealException(
                Target.Raw,
                settings,
                findMode,
                message: "Model not found.",
                seekingComponentType: type,
                name: name.Raw,
                tag: tag.Raw,
                layer: layerMask ?? -1,
                componentFilter: mustContainsType.Raw));
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
            return (Transforms().FirstOrDefault(), new GameObjectAppealException(
                Target.Raw,
                settings,
                findMode,
                message: "Transform not found.",
                seekingComponentType: typeof(Transform),
                name: name.Raw,
                tag: tag.Raw,
                layer: layerMask ?? -1,
                componentFilter: mustContainsType.Raw)
                );
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IEnumerable<GameObject> ChildrenGameObjects() => ByChildren().ExcludeSelf().GameObjects();

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IEnumerable<Transform> ChildrenTransforms() => ByChildren().ExcludeSelf().Transforms();

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IEnumerable<GameObject> ParentGameObjects() => ByParent().ExcludeSelf().GameObjects();

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IEnumerable<Transform> ParentTranforms() => ByParent().ExcludeSelf().Transforms();

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
            return (GameObjects().FirstOrDefault(), new GameObjectAppealException(
                Target.Raw,
                settings,
                findMode,
                message: "Game object not found.",
                seekingComponentType: typeof(GameObject),
                name: name.Raw,
                tag: tag.Raw,
                layer: layerMask ?? -1,
                componentFilter: mustContainsType.Raw)
                );
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Either<Transform, RootMarker> RootRaw()
        {
            var marker = ByParent().IncludeInactive()
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
                .AsOrDefault<Transform>()
                .GetValue(Target.GetValueUnsafe().transform);
        }

        public bool ContainsComponent(object component)
        {
            return Components(component.GetType()).Contains(component);
        }

        public bool ContainsModel(object model, bool includeComponents = true)
        {
            return Models(model.GetType(), includeComponents: includeComponents).Contains(model);
        }

        public bool ContainsViewModel(object viewModel)
        {
            return ViewModels(viewModel.GetType()).Contains(viewModel);
        }

        public bool ContainsView(object view)
        {
            return Views(view.GetType()).Contains(view);
        }

        protected virtual IEnumerable<object> ComponentsInternal(GameObject target, Type? type)
        {
            CC.Guard.IsNotNull(target, nameof(target));
            if (type == typeof(GameObject))
                throw new ArgumentException($"Type cannot be: {type.GetFullName()}.");

            bool anyType = type is null;
            type ??= typeof(Component);

            IEnumerable<Component> components;
            if (anyType || type.IsType<Component>())
            {
                components = findMode switch
                {
                    FindMode.Self => target.GetComponents(type),
                    FindMode.InChilds => target.GetComponentsInChildren(type, settings.IsFlagSetted(Settings.IncludeInactive)),
                    FindMode.InParents => target.GetComponentsInParent(type, settings.IsFlagSetted(Settings.IncludeInactive)),
                    _ => throw new InvalidOperationException(findMode.ToString())
                };
            }
            else
            {
                components = findMode switch
                {
                    FindMode.Self => target.Components()
                                           .Where(cmp => anyType || cmp.IsInstanceOfType(type!)),

                    FindMode.InChilds => ByChildren().GameObjects()
                                                     .SelectMany(x => x.Components())
                                                     .Where(x => anyType || x.IsInstanceOfType(type!)),

                    FindMode.InParents => ByParent().GameObjects()
                                                    .SelectMany(x => x.Components())
                                                    .Where(x => anyType || x.IsInstanceOfType(type!)),

                    _ => throw new InvalidOperationException(findMode.ToString())
                };
            }

            if (mustContainsType.IsSome)
                components = components.Where(cmp =>
                    cmp.gameObject.QueryTo()
                    .Component(mustContainsType.GetValueUnsafe())
                    .Lax()
                    .IsSome
                    );

            if (settings.IsFlagSetted(Settings.NotRecursive)
                &&
                findMode.IsFlagSetted(FindMode.InChilds))
            {
                var goTransform = target.transform;

                components = from cmp in components
                             where cmp.gameObject != Target
                             where cmp.transform.parent == goTransform
                             select cmp;
            }

            if (layerMask.HasValue)
                components = components.Where(cmp => cmp.gameObject.layer == layerMask);

            if (name.IsSome)
                components = components.Where(cmp => cmp.gameObject.name.Match(name.GetValueUnsafe(), stringMatchSettings));

            if (tag.IsSome)
                components = components.Where(cmp => cmp.gameObject.CompareTag(tag.GetValueUnsafe()));

            if (settings.IsFlagSetted(Settings.ExcludeSelf))
                components = components.Where(cmp => cmp.gameObject != target);

            IEnumerable<object> results = components;
            var providerObjects = components.OfType<IObjectProvider>().Select(x => x.InternalObject).Where(x => anyType || x.IsInstanceOfType(type));
            results = results.Concat(providerObjects);

            return results.Where(x => x.IsNotNull());
        }
    }

    public static class GameObjectSearchExtensions
    {
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static GameObjectQuery QueryTo(this GameObject source)
        {
            CC.Guard.IsNotNull(source, nameof(source));

            return new GameObjectQuery().From(source);
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static GameObjectQuery QueryTo(this Component source)
        {
            CC.Guard.IsNotNull(source, nameof(source));

            return new GameObjectQuery().From(source);
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static GameObjectQuery QueryToBySingleton(this GameObject source)
        {
            CC.Guard.IsNotNull(source, nameof(source));

            return GameObjectQuery.Instance.Reset().From(source);
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static GameObjectQuery QueryToBySingleton(this Component source)
        {
            CC.Guard.IsNotNull(source, nameof(source));

            return GameObjectQuery.Instance.Reset().From(source);
        }
    }
}

