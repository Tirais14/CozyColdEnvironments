using System.Collections.Generic;
using UnityEngine;

#nullable enable
namespace CCEnvs.Unity.UI.MVVM
{
    public static class Model
    {
        public static T[] FindModelsByType<T>(
            FindObjectsInactive findObjectsInactive = FindObjectsInactive.Exclude,
            FindObjectsSortMode findObjectsSortMode = FindObjectsSortMode.None)
        {
            var viewModels = ViewModel.FindViewModelsByType<IViewModel>(
                findObjectsInactive,
                findObjectsSortMode);

            var models = new List<T>();
            int count = viewModels.Length;
            for (int i = 0; i < count; i++)
            {
                if (viewModels[i].GetModel() is T typed)
                    models.Add(typed);
            }

            return models.ToArray();
        }

        public static T? FindAnyModelByType<T>(
            FindObjectsInactive findObjectsInactive = FindObjectsInactive.Exclude)
        {
            var viewModels = ViewModel.FindViewModelsByType<IViewModel>(
                findObjectsInactive);

            int count = viewModels.Length;
            for (int i = 0; i < count; i++)
            {
                if (viewModels[i].GetModel() is T typed)
                    return typed;
            }

            return default;
        }
    }
}
