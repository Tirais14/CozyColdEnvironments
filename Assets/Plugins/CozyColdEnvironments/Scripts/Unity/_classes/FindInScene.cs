using CCEnvs.FuncLanguage;
using CCEnvs.Reflection;
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
    public class FindInScene
    {
        public readonly static FindInScene Q = new();

        public static FindInScene Empty => new();

        private readonly static Type defaultType = typeof(Component);

        public FindObjectsInactive Inactive { get; protected set; } = FindObjectsInactive.Exclude;
        public FindObjectsSortMode SortMode { get; protected set; } = FindObjectsSortMode.None;

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public FindInScene IncludeInactive()
        {
            Inactive = FindObjectsInactive.Include;

            return this;
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public FindInScene SortByInstanceID()
        {
            SortMode = FindObjectsSortMode.InstanceID;

            return this;
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public FindInScene Reset()
        {
            Inactive = FindObjectsInactive.Exclude;
            SortMode = FindObjectsSortMode.None;

            return this;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IEnumerable<object> Components(Type? type = null)
        {
            type ??= defaultType;

            bool anyType = type == defaultType;

            IEnumerable<object> results;

            if (type.IsType(defaultType))
            {
                results = Object.FindObjectsByType(type, Inactive, SortMode)
                                .Where(cmp => anyType || cmp.IsType(type));
            }
            else
            {
                results = Object.FindObjectsByType(defaultType, Inactive, SortMode)
                                .Where(cmp => anyType || cmp.IsType(type));
            }

            Reset();

            return results;
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IEnumerable<T> Components<T>()
        {
            return Components(typeof(T)).Cast<T>();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Maybe<object> Component(Type type)
        {
            Guard.IsNotNull(type, nameof(type));

            if (type.IsType<Component>())
                return Object.FindAnyObjectByType(type, Inactive);

            return Components(type).FirstOrDefault();
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Maybe<T> Component<T>()
        {
            return Component(typeof(T)).Cast<T>().RightTarget;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IEnumerable<IView> Views(Type? type = null)
        {
            type ??= typeof(IView);

            if (type.IsNotType<IView>())
                return Array.Empty<IView>();

            return Components().Where(cmp => cmp.IsType(type)).Cast<IView>();
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
        public Maybe<IView> View(Type type)
        {
            Guard.IsNotNull(type, nameof(type));

            return Views(type).FirstOrDefault().Maybe();
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Maybe<T> View<T>()
            where T : IView
        {
            return View(typeof(T)).AsOrDefault<T>();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IEnumerable<IViewModel> ViewModels(Type? type = null)
        {
            return Views(type).Select(x => x.viewModel);
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
        public Maybe<IViewModel> ViewModel(Type type)
        {
            Guard.IsNotNull(type, nameof(type));

            return ViewModels(type).FirstOrDefault().Maybe();
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Maybe<T> ViewModel<T>()
            where T : IViewModel
        {
            return ViewModel(typeof(T)).AsOrDefault<T>();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IEnumerable<object> Models(Type? type = null)
        {
            bool anyType = type is null;

            return from cmp in Components()
                   select (cmp, view: cmp.AsOrDefault<IView>()) into x
                   select x.view.Map(y => y.model).Access(x.cmp) into cmp
                   where anyType || cmp.IsType(type!)
                   select cmp;
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IEnumerable<T> Models<T>()
        {
            return Models(typeof(T)).Cast<T>();
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Maybe<object> Model(Type type)
        {
            Guard.IsNotNull(type, nameof(type));

            return Models(type).FirstOrDefault().Maybe();
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Maybe<T> Model<T>()
        {
            return Model(typeof(T)).AsOrDefault<T>();
        }
    }
}
