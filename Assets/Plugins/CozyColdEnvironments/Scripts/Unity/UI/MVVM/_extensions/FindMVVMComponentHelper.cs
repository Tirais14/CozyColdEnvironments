using CCEnvs.FuncLanguage;
using CCEnvs.Reflection;
using CCEnvs.Unity;
using CCEnvs.Unity.UI.MVVM;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;
using ZLinq;

#nullable enable
namespace CCEnvs.UI.MVVM
{
    public static class FindMVVMComponentHelper
    {
        #region View

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IEnumerable<IView> FindViewsRaw(
            this GameObject source,
            Type? type = null,
            bool includeInactive = false,
            FindMode findMode = FindMode.Self)
        {
            CC.Guard.IsNotNull(source, nameof(source));

            type ??= typeof(IView);

            if (type.IsNotType<IView>())
                return Array.Empty<IView>();

            return source.FindComponentsRaw(
                type: type,
                includeInactive: includeInactive,
                findMode: findMode)
                .Cast<IView>();
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IEnumerable<IView> FindViewsRaw(
            this Component source,
            Type? type = null,
            bool includeInactive = false,
            FindMode findMode = FindMode.Self)
        {
            return source.gameObject.FindViewsRaw(
                type: type,
                includeInactive: includeInactive,
                findMode: findMode);
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IEnumerable<T> FindViewsRaw<T>(
            this GameObject source,
            bool includeInactive = false,
            FindMode findMode = FindMode.Self)

            where T : IView
        {
            return source.FindViewsRaw(
                type: typeof(T),
                includeInactive: includeInactive,
                findMode: findMode)
                .Cast<T>();
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IEnumerable<T> FindViewsRaw<T>(
            this Component source,
            bool includeInactive = false,
            FindMode findMode = FindMode.Self)

            where T : IView
        {
            return source.gameObject.FindViewsRaw<T>(
                includeInactive: includeInactive,
                findMode: findMode);
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Maybe<IView> FindViewRaw(
            this GameObject source,
            Type? type = null,
            bool includeInactive = false,
            FindMode findMode = FindMode.Self)
        {
            return source.FindViewsRaw(
                type: type,
                includeInactive: includeInactive,
                findMode: findMode)
                .FirstOrDefault()
                .Maybe();
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Maybe<IView> FindViewRaw(
            this Component source,
            Type? type = null,
            bool includeInactive = false,
            FindMode findMode = FindMode.Self)
        {
            return source.gameObject.FindViewRaw(
                type: type,
                includeInactive: includeInactive,
                findMode: findMode);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Maybe<T> FindViewRaw<T>(
            this GameObject source,
            bool includeInactive = false,
            FindMode findMode = FindMode.Self)

            where T : IView
        {
            return source.FindViewRaw(
                type: typeof(T),
                includeInactive: includeInactive,
                findMode: findMode)
                .Cast<T>()
                .RightTarget;
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Maybe<T> FindViewRaw<T>(
            this Component source,
            bool includeInactive = false,
            FindMode findMode = FindMode.Self)

            where T : IView
        {
            return source.gameObject.FindViewRaw<T>(
                includeInactive: includeInactive,
                findMode: findMode);
        }

        #endregion View

        #region ViewModel

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IEnumerable<IViewModel> FindViewModelsRaw(
            this GameObject source,
            Type? type = null,
            bool includeInactive = false,
            FindMode findMode = FindMode.Self)
        {
            bool typeIsNull = type is null;

            return source.FindViewsRaw(
                type: type,
                includeInactive: includeInactive,
                findMode: findMode)
                .Select(view => view.viewModel)
                .Where(viewModel => typeIsNull || viewModel.IsType(type!));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IEnumerable<T> FindViewModelsRaw<T>(
            this GameObject source,
            Type? type = null,
            bool includeInactive = false,
            FindMode findMode = FindMode.Self)

            where T : IViewModel
        {
            return source.FindViewModelsRaw(
                type: typeof(T),
                includeInactive: includeInactive,
                findMode: findMode)
                .Cast<T>();
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IEnumerable<IViewModel> FindViewModelsRaw(
            this Component source,
            Type? type = null,
            bool includeInactive = false,
            FindMode findMode = FindMode.Self)
        {
            return source.gameObject.FindViewModelsRaw(
                type: type,
                includeInactive: includeInactive,
                findMode: findMode);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IEnumerable<T> FindViewModelsRaw<T>(
            this Component source,
            Type? type = null,
            bool includeInactive = false,
            FindMode findMode = FindMode.Self)

            where T : IViewModel
        {
            return source.gameObject.FindViewModelsRaw(
                type: typeof(T),
                includeInactive: includeInactive,
                findMode: findMode)
                .Cast<T>();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Maybe<IViewModel> FindViewModelRaw(
            this GameObject source,
            Type? type = null,
            bool includeInactive = false,
            FindMode findMode = FindMode.Self)
        {
            return source.FindViewModelsRaw(
                type: type,
                includeInactive: includeInactive,
                findMode: findMode)
                .FirstOrDefault()
                .Maybe();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Maybe<T> FindViewModelRaw<T>(
            this GameObject source,
            bool includeInactive = false,
            FindMode findMode = FindMode.Self)

            where T : IViewModel
        {
            return source.FindViewModelRaw(
                type: typeof(T),
                includeInactive: includeInactive,
                findMode: findMode)
                .Cast<T>()
                .RightTarget;
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Maybe<IViewModel> FindViewModelRaw(
            this Component source,
            Type? type = null,
            bool includeInactive = false,
            FindMode findMode = FindMode.Self)

            where T : IViewModel
        {
            return source.gameObject.FindViewModelRaw(
                type: type,
                includeInactive: includeInactive,
                findMode: findMode);
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Maybe<T> FindViewModelRaw<T>(
            this Component source,
            bool includeInactive = false,
            FindMode findMode = FindMode.Self)

            where T : IViewModel
        {
            return source.gameObject.FindViewModelRaw<T>(
                includeInactive: includeInactive,
                findMode: findMode);
        }

        #endregion ViewModel

        #region Model

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IEnumerable<object> FindModelsRaw(
            this GameObject source,
            Type? type = null,
            bool includeInactive = false,
            FindMode findMode = FindMode.Self)
        {
            CC.Guard.IsNotNull(source, nameof(source));

            return from obj in source.FindComponentsRaw(
                type: type,
                includeInactive: includeInactive,
                findMode: findMode)
                   select (obj, type: obj.GetType()) into x
                   where x.type.IsType(type) || x.type.IsType<IView>()
                   select (x.obj, x.type, view: x.obj.AsOrDefault<IView>()) into x
                   select x.view.Map(it => it.viewModel.model).Access(x.obj);
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Maybe<object> FindModelRaw(
            this GameObject source,
            Type? type = null,
            bool includeInactive = false,
            FindMode findMode = FindMode.Self)
        {
            return source.FindModelsRaw(
                type: type,
                includeInactive: includeInactive,
                findMode: findMode)
                .FirstOrDefault();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Maybe<T> FindModelRaw<T>(
            this GameObject source,
            bool includeInactive = false,
            FindMode findMode = FindMode.Self)
        {
            return source.FindModelRaw(
                type: typeof(T),
                includeInactive: includeInactive,
                findMode: findMode)
                .Cast<T>()
                .RightTarget;
        }

        #endregion Model
    }
}
