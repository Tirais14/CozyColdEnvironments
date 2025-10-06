using CCEnvs.Linq;
using CCEnvs.Unity.UI;
using System;
using System.Linq;
using UnityEngine;

#nullable enable
namespace CCEnvs.UI.MVVM
{
    public static class ComponentExtensions
    {
        public static T[] GetAssignedViews<T>(this Component value)
            where T : IView
        {
            T[] views = value.gameObject.GetAssignedViews<T>();

            if (value is IView self)
                return views.Where(x => !x.Equals(self)).ToArray();

            return views;
        }

        public static T? GetAssignedView<T>(this Component value)
            where T : IView
        {
            if (value is IView self)
                return value.gameObject.GetAssignedViews<T>()
                                       .FirstOrDefault(x => !x.Equals(self));

            return value.gameObject.GetAssignedView<T>();
        }

        public static T[] GetAssignedViewsInChildren<T>(this Component value,
                                                        bool includeInactive = false)
            where T : IView
        {
            T[] views = value.gameObject.GetAssignedViewsInChildren<T>(includeInactive);

            if (value is IView self)
                return views.Where(x => !x.Equals(self)).ToArray();

            return views;
        }

        public static T? GetAssignedViewInChildren<T>(this Component value,
                                                      bool includeInactive = false)
            where T : IView
        {
            if (value is IView self)
                return value.gameObject.GetAssignedViewsInChildren<T>(includeInactive)
                                       .FirstOrDefault(x => !x.Equals(self));

            return value.gameObject.GetAssignedViewInChildren<T>(includeInactive);
        }

        public static T[] GetAssignedViewModels<T>(this Component value)
            where T : IViewModel
        {
            IView[] views = value.GetAssignedViews<IView>();

            return GameObjectExtensions.FindViewModels<T>(views);
        }

        public static T? GetAssignedViewModel<T>(this Component value)
            where T : IViewModel
        {
            IView[] views = value.GetAssignedViews<IView>();

            return GameObjectExtensions.FindViewModel<T>(views);
        }

        /// <exception cref="ArgumentNullException"></exception>
        public static T[] GetAssignedViewModelsInChildren<T>(this Component value,
                                                             bool includeInactive = false)
            where T : IViewModel
        {
            IView[] views = value.GetAssignedViewsInChildren<IView>(includeInactive);

            return GameObjectExtensions.FindViewModels<T>(views);
        }

        public static T? GetAssignedViewModelInChildren<T>(this Component value,
                                                           bool includeInactive = false)
            where T : IViewModel
        {
            IView[] views = value.GetAssignedViewsInChildren<IView>(includeInactive);

            return GameObjectExtensions.FindViewModel<T>(views);
        }

        public static T[] GetAssignedModels<T>(this Component value)
        {
            IViewModel[] viewModels = value.GetAssignedViewModels<IViewModel>();

            return GameObjectExtensions.FindModels<T>(viewModels);
        }

        public static T? GetAssignedModel<T>(this Component value)
        {
            IViewModel[] viewModels = value.GetAssignedViewModels<IViewModel>();

            return GameObjectExtensions.FindModel<T>(viewModels);
        }

        public static T[] GetAssignedModelsInChildren<T>(this Component value,
                                                         bool includeInactive = false)
        {
            IViewModel[] viewModels = value.GetAssignedViewModelsInChildren<IViewModel>(includeInactive);

            return GameObjectExtensions.FindModels<T>(viewModels);
        }

        public static T? GetAssignedModelInChildren<T>(this Component value,
                                                       bool includeInactive = false)
        {
            IViewModel[] viewModels = value.GetAssignedViewModelsInChildren<IViewModel>(includeInactive);

            return GameObjectExtensions.FindModel<T>(viewModels);
        }
    }
}
