using CCEnvs.Diagnostics;
using CCEnvs.FuncLanguage;
using CCEnvs.Reflection;
using CCEnvs.Unity.Diagnostics;
using CCEnvs.Unity.UI.MVVM;
using CommunityToolkit.Diagnostics;
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
    public record GameObjectSearch
    {
        [Flags]
        public enum Settings
        {
            None,
            Reusable,
            IncludeInactive = 2,
            ExcludeSelf = 4,
            ByFullName = 8,
            IgnoreCase = 16,
            /// <summary>
            /// Except in depth childrens from results
            /// </summary>
            NotRecursive = 32,
            Default = None
        }

        internal readonly static GameObjectSearch Instance = new GameObjectSearch().Reusable(false);
        private readonly static GameObjectSearch empty = new();

        public static GameObjectSearch Empty => new();

        /// <summary>
        /// May be null
        /// </summary>
        public Maybe<GameObject> Target { get; protected set; } = null!;
        public Settings settings { get; protected set; }
        public FindMode findMode { get; protected set; }
        public FindObjectsSortMode sortMode { get; protected set; }
        public Maybe<string> name { get; protected set; }
        /// <summary>
        /// <see cref="Settings.ByFullName"/>, <see cref="Settings.IgnoreCase"/> doesn' affect
        /// </summary>
        public Maybe<string> tag { get; protected set; }
        public int? layerMask { get; protected set; }

        public GameObjectSearch()
        {
            Reset();
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public GameObjectSearch From(GameObject gameObject)
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
        public GameObjectSearch From(Component component)
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
        public GameObjectSearch IncludeInactive(bool state = true)
        {
            if (state)
                settings |= Settings.IncludeInactive;
            else
                settings &= ~Settings.IncludeInactive;

            return this;
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public GameObjectSearch ExcludeSelf(bool state = true)
        {
            if (state)
                settings |= Settings.ExcludeSelf;
            else
                settings &= ~Settings.ExcludeSelf;

            return this;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public GameObjectSearch Reusable(bool state = true)
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
        public GameObjectSearch ByFullName(bool state = true)
        {
            if (state)
                settings |= Settings.ByFullName;
            else
                settings &= ~Settings.ByFullName;

            return this;
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public GameObjectSearch ByName(string? name = null)
        {
            this.name = name;

            return this;
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public GameObjectSearch ByTag(string? tag = null)
        {
            this.tag = tag;

            return this;
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public GameObjectSearch ByLayerMask(int? layerMask = null)
        {
            this.layerMask = layerMask;

            return this;
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public GameObjectSearch BySelf()
        {
            findMode = FindMode.Self;

            return this;
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public GameObjectSearch ByChildren()
        {
            findMode = FindMode.InChilds;

            return this;
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public GameObjectSearch ByParent()
        {
            findMode = FindMode.InParents;

            return this;
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public GameObjectSearch NotRecursive(bool state = true)
        {
            if (state)
                settings |= Settings.NotRecursive;
            else
                settings &= ~Settings.NotRecursive;

            return this;
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public GameObjectSearch SortByInstanceID(bool state = true)
        {
            if (state)
                sortMode = FindObjectsSortMode.InstanceID;
            else
                sortMode = FindObjectsSortMode.None;

            return this;
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public GameObjectSearch Reset()
        {
            Target = default!;
            settings = Settings.Default;
            findMode = FindMode.Self;
            sortMode = FindObjectsSortMode.None;
            name = Maybe<string>.None;
            tag = Maybe<string>.None;
            layerMask = default;

            return this;
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IEnumerable<object> Components(Type? type = null)
        {
            return Target.Match(
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

            return (Components(type).FirstOrDefault(), new ComponentNotFoundException(componentType: type, context: Target.Raw));
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
        public Result<IView> View(Type type)
        {
            return Component(type).Cast<IView>();
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
                   select view.viewModel into viewModel
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

            return (ViewModels(type).FirstOrDefault(), new ComponentNotFoundException(componentType: type, context: Target.Raw));
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
        public IEnumerable<object> Models(Type? type = null)
        {
            bool anyType = type is null;

            var results = from obj in Components()
                          select (obj, view: obj.AsOrDefault<IView>()) into x
                          select x.view.Map(y => y.model).GetValue(x.obj) into obj
                          where anyType || obj.IsInstanceOfType(type!)
                          select obj;

            return results;
        }

        /// <inheritdoc cref="Models"/>
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IEnumerable<T> Models<T>()
        {
            return Models(typeof(T)).Cast<T>();
        }

        /// <inheritdoc cref="Models"/>
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Result<object> Model(Type type)
        {
            Guard.IsNotNull(type, nameof(type));

            return (Models(type).FirstOrDefault(), new ComponentNotFoundException(componentType: type, context: Target.Raw));
        }

        /// <inheritdoc cref="Models"/>
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Result<T> Model<T>()
        {
            return Model(typeof(T)).Cast<T>();
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IEnumerable<Transform> Transforms() => Components<Transform>();

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Result<Transform> Transform()
        {
            return (Transforms().FirstOrDefault(), new ComponentNotFoundException(typeof(Transform), context: Target.Raw));
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IEnumerable<GameObject> ChildrenGameObjects() => ByChildren().GameObjects();

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IEnumerable<Transform> ChildrenTransforms() => ByChildren().Transforms();

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IEnumerable<GameObject> ParentGameObjects() => ByParent().GameObjects();

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IEnumerable<Transform> ParentTranforms() => ByParent().Transforms();

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
            return (GameObjects().FirstOrDefault(), new GameObjectNotFoundException(name.Raw));
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
                .Access()
                .AsOrDefault<Transform>()
                .GetValue(Target.GetValueUnsafe().transform);
        }

        protected virtual IEnumerable<object> ComponentsInternal(GameObject target, Type? type)
        {
            CC.Guard.IsNotNull(target, nameof(target));
            if (type == typeof(GameObject))
                throw new ArgumentException($"Type cannot be: {type.GetFullName()}.");

            bool anyType = type is null;
            type ??= typeof(Component);

            IEnumerable<Component> results;
            if (anyType || type.IsType<Component>())
            {
                results = findMode switch
                {
                    FindMode.Self => target.GetComponents(type),
                    FindMode.InChilds => target.GetComponentsInChildren(type, settings.IsFlagSetted(Settings.IncludeInactive)),
                    FindMode.InParents => target.GetComponentsInParent(type, settings.IsFlagSetted(Settings.IncludeInactive)),
                    _ => throw new InvalidOperationException(findMode.ToString())
                };
            }
            else
            {
                results = findMode switch
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

            if (settings.IsFlagSetted(Settings.NotRecursive)
                &&
                findMode.IsFlagSetted(FindMode.InChilds))
            {
                var goTransform = target.transform;

                results = from cmp in results
                          where cmp.gameObject != Target
                          where cmp.transform.parent == goTransform
                          select cmp;
            }

            if (layerMask.HasValue
                ||
                name.IsSome
                ||
                tag.IsSome)
            {
                results = from cmp in results
                          select (cmp, go: cmp.gameObject) into x
                          where name.Map(name =>
                          {
                              if (settings.IsFlagSetted(Settings.ByFullName))
                                  return x.go.name == name;
                              else
                                  return x.go.name.ContainsOrdinal(name, settings.IsFlagSetted(Settings.IgnoreCase));
                          }).Raw
                          where !layerMask.HasValue || x.go.layer == layerMask
                          where tag.Match(
                              some: tag => x.go.CompareTag(tag),
                              none: () => true)
                          .Raw
                          select x.cmp;

                results = results.ToArray();
            }

            if (settings.IsFlagSetted(Settings.ExcludeSelf))
                results = results.Where(cmp => cmp.gameObject != Target);

            return results;
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void ValidateInstance()
        {
            if (ReferenceEquals(this, Instance) && this != empty)
                this.PrintError("Singleton not reseted before another using.");
        }
    }

    public static class GameObjectSearchExtensions
    {
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static GameObjectSearch FindFor(this GameObject source)
        {
            CC.Guard.IsNotNull(source, nameof(source));

            return new GameObjectSearch().From(source);
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static GameObjectSearch FindFor(this Component source)
        {
            CC.Guard.IsNotNull(source, nameof(source));

            return new GameObjectSearch().From(source);
        }
    }
}

