#nullable enable
using CCEnvs.Disposables;
using CCEnvs.FuncLanguage;
using CCEnvs.Reflection;
using CCEnvs.TypeMatching;
using System.Collections.Generic;
using UnityEngine;

namespace CCEnvs.Unity.UI.MVVM
{
    public static class ViewModel
    {
        public static T[] FindViewModelsByType<T>(
            FindObjectsInactive findObjectsInactive = FindObjectsInactive.Exclude,
            FindObjectsSortMode findObjectsSortMode = FindObjectsSortMode.None)
            where T : IViewModel
        {
            IView[] views = View.FindViewsByType<IView>(findObjectsInactive,
                                                              findObjectsSortMode);

            var viewModels = new List<T>();
            int count = views.Length;
            for (int i = 0; i < count; i++)
            {
                if (views[i].viewModel.Is<T>(out var typed))
                    viewModels.Add(typed);
            }

            return viewModels.ToArray();
        }

        public static T? FindAnyViewModelByType<T>(
            FindObjectsInactive findObjectsInactive = FindObjectsInactive.Exclude)
            where T : IViewModel
        {
            IView[] views = View.FindViewsByType<IView>(findObjectsInactive);

            int count = views.Length;
            for (int i = 0; i < count; i++)
            {
                if (views[i].viewModel.Is<T>(out var typed))
                    return typed;

            }

            return default;
        }
    }
    public abstract class ViewModel<T> : DisposableContainer, IViewModel<T>
    {
        public Maybe<GameObject?> gameObject { get; private set; }
        public T model { get; private set; }

        protected ViewModel(T model, GameObject gameObject)
        {
            this.model = model;
            this.gameObject = gameObject;

            new Maybe<T>(model).Map(x => x as IGameObjectBindable)
                .IfSome(x =>
                {
                    var xR = x!.AsReflected();
                    try
                    {
                        xR.Property(nameof(gameObject)).SetValue(new Maybe<GameObject?>(gameObject));
                    }
                    catch (System.Exception)
                    {
                        xR.Field("_gameObject").SetValue(new Maybe<GameObject?>(gameObject));
                    }
                });
        }
    }
}
