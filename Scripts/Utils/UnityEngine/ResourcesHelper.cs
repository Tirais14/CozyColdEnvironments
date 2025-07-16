using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEngine;
using UTIRLib.Diagnostics;

namespace UTIRLib.Utils
{
    public static class ResourcesHelper
    {
        public static bool IsInResourcesDirectory(string path)
        {
            return path.Contains("assets", StringComparison.InvariantCultureIgnoreCase) &&
                 path.Contains("resources", StringComparison.InvariantCultureIgnoreCase);
        }

        public static string GetRelativePath(string path)
        {
            if (!IsInResourcesDirectory(path))
                return path;

            string resourcesDirectory = Regex.Match(path, @"^(.*)(Resources)").Value;

            return Path.GetRelativePath(resourcesDirectory, path);
        }

        /// <summary>
        /// Load all assets with specified type. Also search in subfolders
        /// </summary>
        public static T[] LoadAll<T>(string fullPath) where T : UnityEngine.Object
        {
            if (string.IsNullOrEmpty(fullPath))
            {
                throw new StringArgumentException(fullPath);
            }

            List<T> loadedObjectsList = new();
            T[] loadedObjects = Resources.LoadAll<T>(fullPath);
            if (loadedObjects != null && loadedObjects.Length > 0) { loadedObjectsList.AddRange(loadedObjects); }

            string[] childDirectories = Directory.GetDirectories(fullPath, "*", SearchOption.AllDirectories);
            for (int i = 0; i < childDirectories.Length; i++)
            {
                loadedObjects = Resources.LoadAll<T>(childDirectories[i]);
                if (loadedObjects != null && loadedObjects.Length > 0) { loadedObjectsList.AddRange(loadedObjects); }
            }

            return loadedObjectsList.ToArray();
        }
    }
}