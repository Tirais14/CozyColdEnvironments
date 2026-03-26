#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEngine;

#nullable enable
namespace CCEnvs.Unity.Editr
{
    public static class AssetDatabaseHelper
    {
        public static void AddTypePrefix(Object asset)
        {
            if (AssetDatabase.GetAssetPath(asset).IsNull(out var assetPath))
                return;

            string? prefix = AssetNameHelper.ResolvePrefix(asset);

            if (prefix is null)
                return;

            if (asset.name.StartsWith(prefix))
                return;

            RenameAsset(asset, assetPath, $"{prefix}{asset.name}");
        }

        public static void RemoveTypePrefix(Object asset)
        {
            if (AssetDatabase.GetAssetPath(asset).IsNull(out var assetPath))
                return;

            string? prefix = AssetNameHelper.ResolvePrefix(asset);

            if (prefix is null)
                return;

            if (!asset.name.StartsWith(prefix))
                return;

            RenameAsset(asset, assetPath, asset.name[prefix.Length..]);
        }

        public static void RenameAsset(Object asset, string assetPath, string newName)
        {
            var assetPathParts = assetPath.Split('\\', '/');

            var fileExtension = Path.GetExtension(assetPath);

            var newPath = Path.Combine(assetPathParts[..^1].JoinStrings('/'), $"{newName}{(Path.HasExtension(newName) ? string.Empty : fileExtension)}");

            if (AssetDatabase.MoveAsset(assetPath, newPath) == assetPath)
                typeof(AssetNameMarkerTool).PrintError($"Rename asset failured. Asset: {asset}; path: {assetPath}");
        }
    }
}
#endif
