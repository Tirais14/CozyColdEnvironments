using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UTIRLib.Diagnostics;
using UTIRLib.Unity.TypeMatching;

#nullable enable
#pragma warning disable S101
namespace UTIRLib.UI.MVVM
{
    public static class MVVMHelper
    {
        public static T[] FindViewModelsByType<T>(
            FindObjectsInactive findObjectsInactive = FindObjectsInactive.Include,
            FindObjectsSortMode findObjectsSortMode = FindObjectsSortMode.None)
            where T : IViewModel
        {
            var sceneComponents = Object.FindObjectsByType<Component>(
                findObjectsInactive,
                findObjectsSortMode);

            var viewModels = new ConcurrentBag<T>();
            Parallel.ForEach(sceneComponents, (x) =>
            {
                if (x.Is<T>(out var typed))
                    viewModels.Add(typed);
            });

            return viewModels.ToArray();
        }

        public static T? FindAnyViewModelByType<T>(
            FindObjectsInactive findObjectsInactive = FindObjectsInactive.Include)
            where T : IViewModel
        {
            var sceneComponents = Object.FindObjectsByType<Component>(
                findObjectsInactive,
                FindObjectsSortMode.None);

            T? found = default;
            var lockObject = new object();
            Parallel.ForEach(sceneComponents, (x, loopState) =>
            {
                if (x.Is<T>(out var typed))
                {
                    lock (lockObject)
                    {
                        if (found.IsDefault())
                        {
                            found = typed;

                            loopState.Break();
                        }
                    }
                }
            });

            return found;
        }

        public static T[] FindModelsByType<T>(
            FindObjectsInactive findObjectsInactive = FindObjectsInactive.Include,
            FindObjectsSortMode findObjectsSortMode = FindObjectsSortMode.None)
        {
            var viewModels = FindViewModelsByType<IViewModel>(findObjectsInactive,
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
            FindObjectsInactive findObjectsInactive = FindObjectsInactive.Include)
        {
            var viewModels = FindViewModelsByType<IViewModel>(findObjectsInactive,
                                                              FindObjectsSortMode.None);

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
