using System.Collections.Generic;
using UnityEngine;
using CozyColdEnvironments.Unity.TypeMatching;

#nullable enable
namespace CozyColdEnvironments.UI.MVVM
{
    public static class ViewModelHelper
    {
        public static T[] FindViewModelsByType<T>(
            FindObjectsInactive findObjectsInactive = FindObjectsInactive.Exclude,
            FindObjectsSortMode findObjectsSortMode = FindObjectsSortMode.None)
            where T : IViewModel
        {
            IView[] views = ViewHelper.FindViewsByType<IView>(findObjectsInactive,
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
            IView[] views = ViewHelper.FindViewsByType<IView>(findObjectsInactive);

            int count = views.Length;
            for (int i = 0; i < count; i++)
            {
                if (views[i].GetViewModel().Is<T>(out var typed))
                    return typed;

            }

            return default;
        }
    }
}
