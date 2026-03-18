using CCEnvs.Collections;
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

        [MenuItem(EditorHelper.ASSETS_TAB + "/" + EditorHelper.CCENVS_TAB + "/Set Asset Prefixes")]
        public static void Execute()
        {
            if (Selection.objects.IsNullOrEmpty())
                return;

            foreach (var obj in Selection.objects.ToArray())
                SetPrefix(obj);
        }

        private static void SetPrefix(Object asset)
        {
            if (AssetDatabase.GetAssetPath(asset).IsNull(out var assetPath))
                return;

            string? prefix = asset switch
            {
                Texture or Texture2D => "TX_",
                GameObject => "PFB_",
                Material => "MAT_",
                TerrainLayer => "TER_LR_",
                AudioClip => "ACL_",
                _ => null
            };

            if (prefix is null)
                return;

            if (asset.name.StartsWith(prefix))
                return;

            var assetPathParts = assetPath.Split('\\', '/');

            var fileExtension = Path.GetExtension(assetPath);

            var newPath = Path.Combine(assetPathParts[..^1].JoinStrings('/'), $"{prefix}{asset.name}{fileExtension}");

            if (AssetDatabase.MoveAsset(assetPath, newPath) == assetPath)
                typeof(AssetNameMarkersTool).PrintError($"Rename asset failured. Asset: {asset}; path: {assetPath}");
        }
    }
}
