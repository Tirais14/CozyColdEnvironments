#nullable enable
using CCEnvs.Disposables;
using CCEnvs.Language;
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
                if (views[i].GetViewModel().Is<T>(out var typed))
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
                if (views[i].GetViewModel().Is<T>(out var typed))
                    return typed;

            }

            return default;
        }
    }
    public abstract class ViewModel<T> : DisposableContainer, IViewModel<T>
    {
        protected T model;

        public Ghost<GameObject?> gameObject { get; private set; }

        protected ViewModel(T model)
        {
            this.model = model;
        }
        protected ViewModel(T model, GameObject gameObject)
            :
            this(model)
        {
            this.gameObject = gameObject;

            new Ghost<T>(model).Map(x => x as IGameObjectBindable)
                .IfSome(x =>
                {
                    var xR = x!.AsReflected();
                    try
                    {
                        xR.Property(nameof(gameObject)).SetValue(new Ghost<GameObject?>(gameObject));
                    }
                    catch (System.Exception)
                    {
                        xR.Field("_gameObject").SetValue(new Ghost<GameObject?>(gameObject));
                    }
                });
        }

        public T GetModel() => model;
        object IViewModel.GetModel() => model!;
    }
}
