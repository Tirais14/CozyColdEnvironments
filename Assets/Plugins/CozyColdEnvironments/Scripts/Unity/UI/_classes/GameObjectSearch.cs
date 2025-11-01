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
        private readonly static GameObjectSearch empty = new GameObjectSearch().Reset();

        public readonly static GameObjectSearch Instance = new GameObjectSearch().Reusable(false);

        public static GameObjectSearch Empty => new GameObjectSearch().Reset();

        public GameObject Source { get; protected set; } = null!;
        public bool includeInactive { get; protected set; }
        public bool excludeSelf { get; protected set; }
        public bool resusable { get; protected set; } = true;
        public Maybe<string> name { get; protected set; }
        public Maybe<string> tag { get; protected set; }
        public int? layerMask { get; protected set; }
        public FindMode findMode { get; protected set; } = FindMode.Self;

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
            includeInactive = state;

            return this;
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public GameObjectSearch ExcludeSelf(bool state = true)
        {
            excludeSelf = state;

            return this;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public GameObjectSearch Reusable(bool state = true)
        {
            if (this == Instance)
                return this;

            resusable = state;

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
            includeInactive = default;
            excludeSelf = default;
            name = default;
            tag = default;
            layerMask = default;
            findMode = FindMode.Self;

            return this;
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Maybe<object> Component(Type type)
        {
            Guard.IsNotNull(type, nameof(type));

            return Components(type).FirstOrDefault();
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Maybe<T> Component<T>()
        {
            return Component(typeof(T)).Cast<T>().RightTarget;
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public object ComponentStrict(Type type)
        {
            Guard.IsNotNull(type, nameof(type));

            return Component(type).IfNone(() => throw new ComponentNotFoundException(type, Source)).Target!;
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T ComponentStrict<T>()
        {
            return Component<T>().IfNone(() => throw new ComponentNotFoundException(typeof(T), Source)).Target!;
        }

        public IEnumerable<object> Components(Type? type = null)
        {
            Guard.IsNotNull(Source, nameof(Source));
            ValidateInstance();

            bool anyType = type is null;

            IEnumerable<Component> results;
            if (anyType || type!.IsType<Component>())
            {
                results = findMode switch
                {
                    FindMode.Self => Source.GetComponents(type),
                    FindMode.InChilds => Source.GetComponentsInChildren(type, includeInactive),
                    FindMode.InParents => Source.GetComponentsInParent(type, includeInactive),
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
                          where name.Map(name => x.go.name == name).Access(true)
                          where !layerMask.HasValue || x.go.layer == layerMask
                          where tag.Map(tag => x.go.CompareTag(tag)).Access(true)
                          select x.cmp;
            }

            if (resusable)
                Reset();

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
        public Maybe<IView> View(Type? type = null)
        {
            return Views(type).FirstOrDefault().Maybe();
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Maybe<T> View<T>()
            where T : IView
        {
            return Views<T>().FirstOrDefault();
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IView ViewStrict(Type? type = null)
        {
            return View(type).IfNone(() => throw new ComponentNotFoundException(type, Source)).Target!;
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T ViewStrict<T>()
            where T : IView
        {
            return ViewStrict(typeof(T)).As<T>();
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
        public Maybe<IViewModel> ViewModel(Type? type = null)
        {
            return ViewModels(type).FirstOrDefault().Maybe();
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Maybe<T> ViewModel<T>()
            where T : IViewModel
        {
            return ViewModels<T>().FirstOrDefault();
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IViewModel ViewModelStrict(Type? type = null)
        {
            return ViewModel(type).IfNone(() => throw new ComponentNotFoundException(type, Source)).Target!;
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T ViewModelStrict<T>()
            where T : IViewModel
        {
            return ViewModelStrict(typeof(T)).As<T>();
        }

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

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IEnumerable<T> Models<T>()
        {
            return Models(typeof(T)).Cast<T>();
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Maybe<object> Model(Type? type = null)
        {
            return Models(type).FirstOrDefault();
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Maybe<T> Model<T>()
        {
            return Models<T>().FirstOrDefault();
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public object ModelStrict(Type? type = null)
        {
            return Model(type).IfNone(() => throw new ComponentNotFoundException(type, Source)).Target!;
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T ModelStrict<T>()
        {
            return ModelStrict().As<T>();
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IEnumerable<Transform> Transforms() => Components<Transform>();

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IEnumerable<GameObject> GameObjects()
        {
            return Transforms().Select(x => x.gameObject);
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

