using System.Linq;
using UnityEngine;
using UTIRLib.Utils;

#nullable enable
namespace UTIRLib.UI.MVVM
{
    public static class ViewHelper
    {
        public static T[] FindViewsByType<T>(
            FindObjectsInactive findObjectsInactive = FindObjectsInactive.Exclude,
            FindObjectsSortMode findObjectsSortMode = FindObjectsSortMode.None)
            where T : IView
        {
            return UnityObjectHelper.FindObjectsByType<T>(findObjectsInactive,
                                                          findObjectsSortMode);
        }

        public static T? FindAnyViewByType<T>(
            FindObjectsInactive findObjectsInactive = FindObjectsInactive.Exclude,
            FindObjectsSortMode findObjectsSortMode = FindObjectsSortMode.None)
            where T : IView
        {
            return UnityObjectHelper.FindObjectsByType<T>(findObjectsInactive,
                                                          findObjectsSortMode)
                                    .SingleOrDefault();
        }
    }
}
