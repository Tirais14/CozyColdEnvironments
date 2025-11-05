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
            Default = None
        }

        internal readonly static GameObjectSearch Instance = new GameObjectSearch().Reusable(false);
        private readonly static GameObjectSearch empty = new();

        public static GameObjectSearch Empty => new();

        /// <summary>
        /// May be null
        /// </summary>
        public GameObject Source { get; protected set; } = null!;
        public Settings settings { get; protected set; } = Settings.Default;
        public Maybe<string> name { get; protected set; }
        /// <summary>
        /// <see cref="Settings.ByFullName"/>, <see cref="Settings.IgnoreCase"/> doesn' affect
        /// </summary>
        public Maybe<string> tag { get; protected set; }
        public int? layerMask { get; protected set; }
        public FindMode findMode { get; protected set; } = FindMode.Self;

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

            Source = gameObject;
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

            Source = component.gameObject;
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
        public GameObjectSearch Name(string? name = null)
        {
            this.name = name;

            return this;
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public GameObjectSearch Tag(string? tag = null)
        {
            this.tag = tag;

            return this;
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public GameObjectSearch Layers(int? layerMask = null)
        {
            this.layerMask = layerMask;

            return this;
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public GameObjectSearch InSelf()
        {
            findMode = FindMode.Self;

            return this;
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public GameObjectSearch InChildren()
        {
            findMode = FindMode.InChilds;

            return this;
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public GameObjectSearch InParent()
        {
            findMode = FindMode.InParents;

            return this;
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public GameObjectSearch Reset()
        {
            Source = default!;
            settings = Settings.Default;
            name = Maybe<string>.None;
            tag = Maybe<string>.None;
            layerMask = default;
            findMode = FindMode.Self;

            return this;
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Result<object> Component(Type type)
        {
            Guard.IsNotNull(type, nameof(type));

            return (Components(type).FirstOrDefault(), new ComponentNotFoundException(componentType: type, context: Source));
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Result<T> Component<T>()
        {
            return Component(typeof(T)).Cast<T>();
        }

        public IEnumerable<object> Components(Type? type = null)
        {
            Guard.IsNotNull(Source, nameof(Source));

            bool anyType = type is null;
            type ??= typeof(Component);

            IEnumerable<Component> results;
            if (anyType || type!.IsType<Component>())
            {
                results = findMode switch
                {
                    FindMode.Self => Source.GetComponents(type),
                    FindMode.InChilds => Source.GetComponentsInChildren(type, settings.IsFlagSetted(Settings.IncludeInactive)),
                    FindMode.InParents => Source.GetComponentsInParent(type, settings.IsFlagSetted(Settings.IncludeInactive)),
                    _ => throw new InvalidOperationException(findMode.ToString())
                };
            }
            else
            {
                results = findMode switch
                {
                    FindMode.Self => Source.Components()
                                           .Where(cmp => anyType || cmp.IsType(type!)),

                    FindMode.InChilds => InChildren().GameObjects()
                                                     .SelectMany(x => x.Components())
                                                     .Where(x => anyType || x.IsType(type!)),

                    FindMode.InParents => InParent().GameObjects()
                                                    .SelectMany(x => x.Components())
                                                    .Where(x => anyType || x.IsType(type!)),

                    _ => throw new InvalidOperationException(findMode.ToString())
                };
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
                          where tag.Map(tag => x.go.CompareTag(tag)).Access(true)
                          select x.cmp;
            }

            //if (!settings.IsFlagSetted(Settings.Reusable))
            //    Reset();

            return results;
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IEnumerable<T> Components<T>()
        {
            return Components(typeof(T)).Cast<T>();
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
                   where anyType || viewModel.IsType(type!)
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

            return (ViewModels(type).FirstOrDefault(), new ComponentNotFoundException(componentType: type, context: Source));
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
                          select x.view.Map(y => y.model).Access(x.obj) into obj
                          where anyType || obj.IsType(type!)
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

            return (Models(type).FirstOrDefault(), new ComponentNotFoundException(componentType: type, context: Source));
        }

        /// <inheritdoc cref="Models"/>
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Maybe<T> Model<T>()
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
            return (Transforms().FirstOrDefault(), new ComponentNotFoundException(typeof(Transform), context: Source));
        }

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
            return (GameObjects().FirstOrDefault(), new ComponentNotFoundException(typeof(GameObject), context: Source));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Either<Transform, RootMarker> Root()
        {
            var marker = InParent().IncludeInactive()
                                   .Component<RootMarker>()
                                   .Lax();

            return (marker.Map(x => x.transform).Access(Source.transform.root), marker.Raw);
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

            return GameObjectSearch.Instance.Reset().From(source);
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static GameObjectSearch FindFor(this Component source)
        {
            CC.Guard.IsNotNull(source, nameof(source));

            return GameObjectSearch.Instance.Reset().From(source);
        }
    }
}

