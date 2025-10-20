#nullable enable 
using CCEnvs.Reflection.Data;
using CCEnvs.Unity.UI.Elements;
using System;
using System.Linq;
using UniRx;
using UnityEngine;

#pragma warning disable IDE1006
namespace CCEnvs.Unity.UI.MVVM
{
    public static class View
    {
        public static T[] FindViewsByType<T>(
            FindObjectsInactive findObjectsInactive = FindObjectsInactive.Exclude,
            FindObjectsSortMode findObjectsSortMode = FindObjectsSortMode.None)
            where T : IView
        {
            return SceneSearch.FindObjectsByType<T>(findObjectsInactive,
                                                    findObjectsSortMode);
        }

        public static T? FindAnyViewByType<T>(
            FindObjectsInactive findObjectsInactive = FindObjectsInactive.Exclude,
            FindObjectsSortMode findObjectsSortMode = FindObjectsSortMode.None)
            where T : IView
        {
            return SceneSearch.FindObjectsByType<T>(
                findObjectsInactive,
                findObjectsSortMode)
                .FirstOrDefault();
        }
    }
    /// <summary>
    /// <see cref="TModel"/> must have empty constructor by default
    /// </summary>
    /// <typeparam name="TViewModel"></typeparam>
    /// <typeparam name="TModel"></typeparam>
    public abstract class View<TViewModel, TModel> : Element, IView<TViewModel, TModel>
        where TViewModel : ViewModel<TModel>
    {
        private LazyCC<TViewModel> viewModelLazy = null!;

        protected TViewModel viewModel => viewModelLazy.Value;

        protected override void Awake()
        {
            base.Awake();

            viewModelLazy ??= new LazyCC<TViewModel>(CreateViewModel);
        }

        protected virtual TViewModel CreateViewModel()
        {
            TModel model = InstanceFactory.Create<TModel>(
                parameters: InstanceFactory.Parameters.CacheConstructor
                            |
                            InstanceFactory.Parameters.ThrowIfNotFound)!;

            if (model is IDisposable disposable)
                disposable.AddTo(this);

            return InstanceFactory.Create<TViewModel>(
                new ExplicitArguments(
                    new ExplicitArgument(model)),
                InstanceFactory.Parameters.CacheConstructor
                |
                InstanceFactory.Parameters.ThrowIfNotFound);
        }

        public TViewModel GetViewModel()
        {
            viewModelLazy ??= new LazyCC<TViewModel>(CreateViewModel);

            return viewModel;
        }

        public TModel GetModel() => GetViewModel().GetModel();
    }
}
