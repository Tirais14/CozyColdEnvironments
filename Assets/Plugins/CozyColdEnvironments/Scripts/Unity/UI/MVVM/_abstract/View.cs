#nullable enable 
using CCEnvs.Unity.Components;
using CCEnvs.Unity.UI.Elements;
using System.Linq;
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
    public abstract class View<T> : Elements.Element, IView
        where T : IViewModel
    {
        private LazyCC<T> viewModelLazy = null!;

        protected T viewModel => viewModelLazy.Value;

        protected override void Awake()
        {
            base.Awake();

            viewModelLazy ??= new LazyCC<T>(CreateViewModel);
        }

        protected abstract T CreateViewModel();

        public IViewModel GetViewModel()
        {
            viewModelLazy ??= new LazyCC<T>(CreateViewModel);

            return viewModel;
        }
    }
}
