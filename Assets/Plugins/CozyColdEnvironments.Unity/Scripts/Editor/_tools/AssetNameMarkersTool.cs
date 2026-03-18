using CCEnvs.Collections;
using CCEnvs.Reflection;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

#nullable enable
namespace CCEnvs.Unity.UnityEditor
{
    [InitializeOnLoad]
    public static class AssetNameMarkersTool
    {
        static AssetNameMarkersTool()
        {
        }

        [MenuItem(EditorHelper.ASSETS_TAB + "/" + EditorHelper.CCENVS_TAB + "/Add Asset Prefixes")]
        public static void AddPrefixes()
        {
            if (Selection.objects.IsNullOrEmpty())
                return;

            foreach (var asset in Selection.objects.ToArray())
                AddPrefix(asset);
        }

        [MenuItem(EditorHelper.ASSETS_TAB + "/" + EditorHelper.CCENVS_TAB + "/Remove Asset Prefixes")]
        public static void RemovePrefixes()
        {
            if (Selection.objects.IsNullOrEmpty())
                return;

            foreach (var asset in Selection.objects.ToArray())
                RemovePrefix(asset);
        }

        private static string? GetPrefix(Object asset)
        {
            return asset switch
            {
                Texture or Texture2D => "TX_",
                GameObject => "PFB_",
                Material => "MAT_",
                TerrainLayer => "TER_LR_",
                AudioClip => "ACL_",
                ScriptableObject => "CFG_",
                _ => null
            };
        }

        private static void AddPrefix(Object asset)
        {
            if (AssetDatabase.GetAssetPath(asset).IsNull(out var assetPath))
                return;

            string? prefix = GetPrefix(asset);

            if (prefix is null)
                return;

            if (asset.name.StartsWith(prefix))
                return;

            RenameAsset(asset, assetPath, $"{prefix}{asset.name}");
        }

        private static void RemovePrefix(Object asset)
        {
            if (AssetDatabase.GetAssetPath(asset).IsNull(out var assetPath))
                return;

            string? prefix = GetPrefix(asset);

            if (prefix is null)
                return;

            if (!asset.name.StartsWith(prefix))
                return;

            RenameAsset(asset, assetPath, asset.name[prefix.Length..]);
        }

        private static void RenameAsset(Object asset, string assetPath, string newName)
        {
            var assetPathParts = assetPath.Split('\\', '/');

            var fileExtension = Path.GetExtension(assetPath);

            var newPath = Path.Combine(assetPathParts[..^1].JoinStrings('/'), $"{newName}{(Path.HasExtension(newName) ? string.Empty : fileExtension)}");

            if (AssetDatabase.MoveAsset(assetPath, newPath) == assetPath)
                typeof(AssetNameMarkersTool).PrintError($"Rename asset failured. Asset: {asset}; path: {assetPath}");
        }
    }
}
