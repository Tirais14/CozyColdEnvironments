using System;
using System.Linq;
using UnityEditor;
using UnityEngine;

#nullable enable

namespace UTIRLib.Editor
{
    public static class ResourcesEditorUtility
    {
        //public static T[] LoadAssetsAt<T>(string path, bool includeSubfolders = true)
        //    where T : UnityEngine.Object
        //{
        //    if (includeSubfolders)
        //    {
        //        string[] subFolders = Directory.GetDirectories(path, "*", SearchOption.AllDirectories);
        //        List<T> loadedObjs = new();
        //        AssetDatabase.LoadAllAssetsAtPath(path).Select(obj => obj as T)
        //            .Where(obj => obj != null).ToArray();
        //    }

        //    return AssetDatabase.LoadAllAssetsAtPath(path).Select(obj => obj as T)
        //        .Where(obj => obj != null).ToArray();
        //}

        public static AssetInfo<T>[] LoadAssets<T>(string[] searchInFolders = null, string fileName = null)
            where T : UnityEngine.Object
        {
            string[] foundAssetGUIDs = FindAssets<T>(searchInFolders, fileName);
            if (foundAssetGUIDs.Length == 0)
            {
                return Array.Empty<AssetInfo<T>>();
            }

            string[] foundAssetPaths = ConvertAssetGUIDsToPaths(foundAssetGUIDs);

            int foundAssetPathsCount = foundAssetPaths.Length;
            AssetInfo<T>[] foundAssetsInfo = new AssetInfo<T>[foundAssetPathsCount];
            for (int i = 0; i < foundAssetPathsCount; i++)
            {
                foundAssetsInfo[i] = new AssetInfo<T>(
                    AssetDatabase.LoadAssetAtPath<T>(foundAssetPaths[i]),
                    foundAssetPaths[i], foundAssetGUIDs[i]);
            }

            return foundAssetsInfo;
        }

        public static AssetInfo<T>[] LoadAssets<T>(string path, string fileName)
            where T : UnityEngine.Object
        {
            string[] searchInFolders = new string[] { path };
            return LoadAssets<T>(searchInFolders, fileName);
        }

        public static AssetInfo<T>[] LoadAssets<T>(string path)
            where T : UnityEngine.Object
        {
            string[] searchInFolders = new string[] { path };
            return LoadAssets<T>(searchInFolders);
        }

        public static AssetInfo<T> LoadAsset<T>(string assetPath)
            where T : UnityEngine.Object
        {
            T loadedAsset = AssetDatabase.LoadAssetAtPath<T>(assetPath);
            if (loadedAsset == null)
            {
                Debug.LogWarning($"Load asset operation failed! Path = {assetPath}");
                return null;
            }

            return new AssetInfo<T>(loadedAsset, assetPath);
        }

        /// <returns>Found asset guids or empty array</returns>
        private static string[] FindAssets<T>(string[] searchInFolders = null, string fileName = null)
            where T : UnityEngine.Object
        {
            string filter = $"t:{typeof(T).Name}";
            if (!string.IsNullOrEmpty(fileName))
            {
                filter += $" n:{fileName}";
            }

            string[] foundAssetGUIDs;
            if (searchInFolders != null && searchInFolders.Length > 0)
            {
                foundAssetGUIDs = AssetDatabase.FindAssets(filter, searchInFolders) ?? Array.Empty<string>();
            }
            else
            {
                foundAssetGUIDs = AssetDatabase.FindAssets(filter) ?? Array.Empty<string>();
            }

            if (foundAssetGUIDs.Length == 0)
            {
                Debug.LogWarning($"Assets with search options: {nameof(searchInFolders)} =" +
                    $" {searchInFolders.Aggregate((resultString, folder) => resultString += folder)}," +
                    $" {nameof(fileName)} = {fileName}, {nameof(filter)} = {filter}");
            }

            return foundAssetGUIDs;
        }

        private static string[] ConvertAssetGUIDsToPaths(string[] assetGUIDs) =>
            assetGUIDs.Select(
                    assetGUID => AssetDatabase.GUIDToAssetPath(assetGUID)).
                    ToArray();
    }
}