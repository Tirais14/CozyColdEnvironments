using System.Collections.Generic;
using UnityEngine;
using UTIRLib.Unity.Extensions;
using UTIRLib.Unity.TypeMatching;

#nullable enable
namespace UTIRLib.UI.MVVM
{
    public static class GameObjectExtensions
    {
        public static T[] GetAssignedViews<T>(this GameObject value)
            where T : IView
        {
            return value.GetAssignedObjects<T>();
        }

        public static T? GetAssignedView<T>(this GameObject value)
            where T : IView
        {
            return value.GetAssignedObject<T>();
        }

        public static T[] GetAssignedViewsInChildren<T>(this GameObject value,
                                                        bool includeInactive = false)
            where T : IView
        {
            return value.GetAssignedObjectsInChildren<T>(includeInactive);
        }

        public static T? GetAssignedViewInChildren<T>(this GameObject value,
                                                      bool includeInactive = false)
            where T : IView
        {
            return value.GetAssignedObjectInChildren<T>(includeInactive);
        }

        public static T[] GetAssignedViewModels<T>(this GameObject value)
            where T : IViewModel
        {
            IView[] views = value.GetAssignedObjects<IView>();

            return FindViewModels<T>(views);
        }

        public static T? GetAssignedViewModel<T>(this GameObject value)
            where T : IViewModel
        {
            IView[] views = value.GetAssignedObjects<IView>();

            return FindViewModel<T>(views);
        }

        /// <exception cref="ArgumentNullException"></exception>
        public static T[] GetAssignedViewModelsInChildren<T>(this GameObject value,
                                                             bool includeInactive = false)
            where T : IViewModel
        {
            IView[] views = value.GetAssignedObjectsInChildren<IView>(includeInactive);

            return FindViewModels<T>(views);
        }

        public static T? GetAssignedViewModelInChildren<T>(this GameObject value,
                                                           bool includeInactive = false)
            where T : IViewModel
        {
            IView[] views = value.GetAssignedObjectsInChildren<IView>(includeInactive);

            return FindViewModel<T>(views);
        }

        public static T[] GetAssignedModels<T>(this GameObject value)
        {
            var viewModels = value.GetAssignedViewModels<IViewModel>();

            return FindModels<T>(viewModels);
        }

        public static T? GetAssignedModel<T>(this GameObject value)
        {
            var viewModels = value.GetAssignedViewModels<IViewModel>();

            return FindModel<T>(viewModels);
        }

        public static T[] GetAssignedModelsInChildren<T>(this GameObject value,
                                         bool includeInactive = false)
        {
            var viewModels = value.GetAssignedViewModelsInChildren<IViewModel>(includeInactive);

            return FindModels<T>(viewModels);
        }

        public static T? GetAssignedModelInChildren<T>(this GameObject value,
                                                bool includeInactive = false)
        {
            var viewModels = value.GetAssignedViewModelsInChildren<IViewModel>(includeInactive);

            return FindModel<T>(viewModels);
        }

        internal static T[] FindViewModels<T>(IView[] views)
            where T : IViewModel
        {
            var viewModels = new List<T>();
            int count = views.Length;
            for (int i = 0; i < count; i++)
            {
                if (views[i].GetViewModel().Is<T>(out var viewModel))
                    viewModels.Add(viewModel);
            }

            return viewModels.ToArray();
        }

        internal static T? FindViewModel<T>(IView[] views)
            where T : IViewModel
        {
            int count = views.Length;
            for (int i = 0; i < count; i++)
            {
                if (views[i].GetViewModel().Is<T>(out var viewModel))
                    return viewModel;
            }

            return default;
        }

        internal static T[] FindModels<T>(IViewModel[] viewModels)
        {
            var models = new List<T>();
            int count = viewModels.Length;
            for (int i = 0; i < count; i++)
            {
                if (viewModels[i].GetModel().Is<T>(out var model))
                    models.Add(model);
            }

            return models.ToArray();
        }

        internal static T? FindModel<T>(IViewModel[] viewModels)
        {
            int count = viewModels.Length;
            for (int i = 0; i < count; i++)
            {
                if (viewModels[i].GetModel().Is<T>(out var model))
                    return model;
            }

            return default;
        }
    }
}
