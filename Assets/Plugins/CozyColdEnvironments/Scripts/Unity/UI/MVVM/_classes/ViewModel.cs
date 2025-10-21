#nullable enable
using CCEnvs.Disposables;
using CCEnvs.Language;
using CCEnvs.Reflection;
using CCEnvs.TypeMatching;
using System.Collections.Generic;

#if UNITY_2017_1_OR_NEWER
using UnityEngine;
#endif

namespace CCEnvs.Unity.UI.MVVM
{
    public static class ViewModel
    {
#if UNITY_2017_1_OR_NEWER
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
#endif //UNITY_2017_1_OR_NEWER
    }
    public abstract class ViewModel<T> : DisposableContainer, IViewModel<T>
    {
        protected T model;

#if UNITY_2017_1_OR_NEWER
        public Ghost<GameObject?> gameObject { get; private set; }
#endif

        protected ViewModel(T model)
        {
            this.model = model;
        }
#if UNITY_2017_1_OR_NEWER
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
#endif

        public T GetModel() => model;
        object IViewModel.GetModel() => model!;
    }
}
