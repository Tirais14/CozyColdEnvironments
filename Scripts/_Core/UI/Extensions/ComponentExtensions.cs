using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UTIRLib.Linq;
using UTIRLib.Unity.Extensions;
using UTIRLib.Unity.TypeMatching;

#nullable enable
namespace UTIRLib.UI.MVVM
{
    public static class ComponentExtensions
    {
        public static T[] GetAssignedViewModels<T>(this Component value)
            where T : IViewModel
        {
            IView[] views;
            if (value is IView self)
                views = value.GetAssignedObjects<IView>()
                             .Where(x => !x.Equals(self))
                             .ToArray();
            else
                views = value.GetAssignedObjects<IView>();

            return FindViewModels<T>(views);
        }

        public static T? GetAssignedViewModel<T>(this Component value)
            where T : IViewModel
        {
            IView[] views;
            if (value is IView self)
                views = value.GetAssignedObjects<IView>()
                             .Where(x => !x.Equals(self))
                             .ToArray();
            else
                views = value.GetAssignedObjects<IView>();

            return FindViewModel<T>(views);
        }

        public static T[] GetAssignedModels<T>(this Component value)
        {
            var viewModels = value.GetAssignedViewModels<IViewModel>();

            return FindModels<T>(viewModels);
        }

        public static T? GetAssignedModel<T>(this Component value)
        {
            var viewModels = value.GetAssignedViewModels<IViewModel>();

            return FindModel<T>(viewModels);
        }

        /// <exception cref="ArgumentNullException"></exception>
        public static T[] GetAssignedViewModelsInChildren<T>(this Component value,
                                                             bool includeInactive = false)
            where T : IViewModel
        {
            IView[] views;
            if (value is IView self)
                views = value.GetAssignedObjectsInChildren<IView>(includeInactive)
                             .Where(x => !x.Equals(self))
                             .ToArray();
            else
                views = value.GetAssignedObjects<IView>();

            return FindViewModels<T>(views);
        }

        public static T? GetAssignedViewModelInChildren<T>(this Component value,
                                                           bool includeInactive = false)
            where T : IViewModel
        {
            IView[] views;
            if (value is IView self)
                views = value.GetAssignedObjectsInChildren<IView>(includeInactive)
                             .Where(x => !x.Equals(self))
                             .ToArray();
            else
                views = value.GetAssignedObjects<IView>();

            return FindViewModel<T>(views);
        }

        public static T[] GetAssignedModelsInChildren<T>(this Component value,
                                                 bool includeInactive = false)
        {
            var viewModels = value.GetAssignedViewModelsInChildren<IViewModel>(includeInactive);

            return FindModels<T>(viewModels);
        }

        public static T? GetAssignedModelInChidlren<T>(this Component value,
                                                bool includeInactive = false)
        {
            var viewModels = value.GetAssignedViewModelsInChildren<IViewModel>(includeInactive);

            return FindModel<T>(viewModels);
        }

        private static T[] FindViewModels<T>(IView[] views)
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

        private static T? FindViewModel<T>(IView[] views)
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

        private static T[] FindModels<T>(IViewModel[] viewModels)
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

        private static T? FindModel<T>(IViewModel[] viewModels)
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
