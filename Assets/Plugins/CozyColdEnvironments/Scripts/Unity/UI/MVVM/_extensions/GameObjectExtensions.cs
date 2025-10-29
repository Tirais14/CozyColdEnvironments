using CCEnvs.FuncLanguage;
using CCEnvs.Unity;
using CCEnvs.Unity.UI.MVVM;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;
using ZLinq;

#nullable enable
namespace CCEnvs.UI.MVVM
{
    public static class GameObjectExtensions
    {
        #region Views
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Maybe<T> GetAssignedView<T>(this GameObject source)
            where T : IView
        {
            CC.Guard.IsNotNull(source, nameof(source));

            return source.GetAssignedObject<T>();
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T[] GetAssignedViews<T>(this GameObject source)
            where T : IView
        {
            CC.Guard.IsNotNull(source, nameof(source));

            return source.GetAssignedObjects<T>();
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IView[] GetAssignedViews(this GameObject source)
        {
            return source.GetAssignedObjects<IView>();
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Maybe<T> GetAssignedViewInChildren<T>(this GameObject source, bool includeInactive = false)
            where T : IView
        {
            CC.Guard.IsNotNull(source, nameof(source));
            return source.GetAssignedObjectInChildren<T>(includeInactive);
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T[] GetAssignedViewsInChildren<T>(this GameObject source, bool includeInactive = false)
            where T : IView
        {
            CC.Guard.IsNotNull(source, nameof(source));
            return source.GetAssignedObjectsInChildren<T>(includeInactive);
        }
        [DebuggerStepThrough]

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IView[] GetAssignedViewsInChildren(this GameObject source, bool includeInactive = false)
        {
            return source.GetAssignedObjectsInChildren<IView>(includeInactive);
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Maybe<T> GetAssignedViewInParent<T>(this GameObject source, bool includeInactive = false)
            where T : IView
        {
            CC.Guard.IsNotNull(source, nameof(source));
            return source.GetAssignedObjectInParent<T>(includeInactive);
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T[] GetAssignedViewsInParent<T>(this GameObject source, bool includeInactive = false)
            where T : IView
        {
            CC.Guard.IsNotNull(source, nameof(source));
            return source.GetAssignedObjectsInParent<T>(includeInactive);
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IView[] GetAssignedViewsInParent(this GameObject source, bool includeInactive = false)
        {
            return source.GetAssignedObjectsInParent<IView>(includeInactive);
        }

        #endregion Views

        #region ViewModels

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Maybe<T> GetAssignedViewModel<T>(this GameObject source)
            where T : IViewModel
        {
            CC.Guard.IsNotNull(source, nameof(source));

            return source.GetAssignedViews().FilterViewModels<T>(single: true).As<Maybe<T>>();
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T[] GetAssignedViewModels<T>(this GameObject source)
            where T : IViewModel
        {
            CC.Guard.IsNotNull(source, nameof(source));

            return source.GetAssignedViews().FilterViewModels<T>(single: false).As<T[]>();
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IViewModel[] GetAssignedViewModels(this GameObject source)
        {
            return source.GetAssignedViewModels<IViewModel>();
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Maybe<T> GetAssignedViewModelInChildren<T>(this GameObject source, bool includeInactive = false)
            where T : IViewModel
        {
            CC.Guard.IsNotNull(source, nameof(source));
            return source.GetAssignedViewsInChildren(includeInactive).FilterViewModels<T>(single: true).As<Maybe<T>>();
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T[] GetAssignedViewModelsInChildren<T>(this GameObject source, bool includeInactive = false)
            where T : IViewModel
        {
            CC.Guard.IsNotNull(source, nameof(source));
            return source.GetAssignedViewsInChildren(includeInactive).FilterViewModels<T>(single: false).As<T[]>();
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IViewModel[] GetAssignedViewModelsInChildren(this GameObject source, bool includeInactive = false)
        {
            return source.GetAssignedViewModelsInChildren<IViewModel>(includeInactive);
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Maybe<T> GetAssignedViewModelInParent<T>(this GameObject source, bool includeInactive = false)
            where T : IViewModel
        {
            CC.Guard.IsNotNull(source, nameof(source));
            return source.GetAssignedViewsInParent(includeInactive).FilterViewModels<T>(single: true).As<Maybe<T>>();
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T[] GetAssignedViewModelsInParent<T>(this GameObject source, bool includeInactive = false)
            where T : IViewModel
        {
            CC.Guard.IsNotNull(source, nameof(source));
            return source.GetAssignedViewsInParent(includeInactive).FilterViewModels<T>(single: false).As<T[]>();
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IViewModel[] GetAssignedViewModelsInParent(this GameObject source, bool includeInactive = false)
        {
            return source.GetAssignedViewModelsInParent<IViewModel>(includeInactive);
        }

        #endregion ViewModels

        #region Models

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Maybe<T> GetAssignedModel<T>(this GameObject source)
        {
            CC.Guard.IsNotNull(source, nameof(source));

            return source.GetAssignedViewModels().FilterModels<T>(single: true).As<Maybe<T>>();
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T[] GetAssignedModels<T>(this GameObject source)
        {
            CC.Guard.IsNotNull(source, nameof(source));

            return source.GetAssignedViewModels().FilterModels<T>(single: false).As<T[]>();
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static object[] GetAssignedModels(this GameObject source)
        {
            return source.GetAssignedModels<object>();
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Maybe<T> GetAssignedModelInChildren<T>(this GameObject source, bool includeInactive = false)
        {
            CC.Guard.IsNotNull(source, nameof(source));
            return source.GetAssignedViewModelsInChildren(includeInactive).FilterModels<T>(single: true).As<Maybe<T>>();
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T[] GetAssignedModelsInChildren<T>(this GameObject source, bool includeInactive = false)
        {
            CC.Guard.IsNotNull(source, nameof(source));
            return source.GetAssignedViewModelsInChildren(includeInactive).FilterModels<T>(single: false).As<T[]>();
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static object[] GetAssignedModelsInChildren(this GameObject source, bool includeInactive = false)
        {
            return source.GetAssignedModelsInChildren<object>(includeInactive);
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Maybe<T> GetAssignedModelInParent<T>(this GameObject source, bool includeInactive = false)
        {
            CC.Guard.IsNotNull(source, nameof(source));
            return source.GetAssignedViewModelsInParent(includeInactive).FilterModels<T>(single: true).As<Maybe<T>>();
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T[] GetAssignedModelsInParent<T>(this GameObject source, bool includeInactive = false)
        {
            CC.Guard.IsNotNull(source, nameof(source));
            return source.GetAssignedViewModelsInParent(includeInactive).FilterModels<T>(single: false).As<T[]>();
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static object[] GetAssignedModelsInParent(this GameObject source, bool includeInactive = false)
        {
            return source.GetAssignedModelsInParent<object>(includeInactive);
        }

        #endregion Models

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static object FilterViewModels<T>(this IEnumerable<IView> views, bool single)
            where T : IViewModel
        {
            if (single)
                return views.FirstOrDefault(x => x is T).Maybe().Map(x => (T)x);

            return views.Where(x => x is T).Cast<T>();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static object FilterModels<T>(this IEnumerable<IViewModel> viewModels, bool single)
        {
            if (single)
                return viewModels.FirstOrDefault(x => x is T).Maybe().Map(x => (T)x).Access()!;

            return viewModels.Where(x => x is T).Cast<T>();
        }
    }
}
