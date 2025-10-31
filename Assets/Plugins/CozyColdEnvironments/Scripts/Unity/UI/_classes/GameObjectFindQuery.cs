using CCEnvs.FuncLanguage;
using CCEnvs.Reflection;
using CCEnvs.Unity.Diagnostics;
using CCEnvs.Unity.UI.MVVM;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;
using static CCEnvs.Unity.FindComponentHelper;

#nullable enable
namespace CCEnvs.Unity
{
    public class GameObjectFindQuery : FindComponentQueryBase<GameObjectFindQuery>
    {
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Maybe<object> Component(Type? type = null)
        {
            return FindComponentRaw(
                Source,
                type: type,
                includeInactive: includeInactive,
                findMode: findMode);
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Maybe<T> Component<T>()
        {
            return FindComponentRaw<T>(
                Source,
                includeInactive: includeInactive,
                findMode: findMode);
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public object ComponentStrict(Type? type = null)
        {
            return Component(type).IfNone(() => throw new ComponentNotFoundException(type, Source)).Target!;
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T ComponentStrict<T>()
        {
            return Component<T>().IfNone(() => throw new ComponentNotFoundException(typeof(T), Source)).Target!;
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IEnumerable<object> Components(Type? type = null)
        {
            var results = FindComponentsRaw(
                Source,
                type: type,
                includeInactive: includeInactive,
                findMode: findMode);

            Reset();

            return results;
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IEnumerable<T> Components<T>()
        {
            var results = FindComponentsRaw<T>(
                Source,
                includeInactive: includeInactive,
                findMode: findMode);

            Reset();

            return results;
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
            type ??= typeof(object);

            bool anyType = type is null;

            var results = from obj in Components(type)
                          select (obj, type: obj.GetType()) into x
                          select (x.obj, x.type, view: x.AsOrDefault<IView>()) into x
                          select x.view.Map(y => y.viewModel.model).Access(x.obj) into obj
                          where anyType || obj.IsType(type!)
                          select obj;

            Reset();

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
    }

    public static class GameObjectFindQueryExtensions
    {
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static GameObjectFindQuery FindFor(this GameObject source)
        {
            CC.Guard.IsNotNull(source, nameof(source));

            return GameObjectFindQuery.Instance.From(source);
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static GameObjectFindQuery FindFor(this Component source)
        {
            CC.Guard.IsNotNull(source, nameof(source));

            return GameObjectFindQuery.Instance.From(source);
        }
    }
}
