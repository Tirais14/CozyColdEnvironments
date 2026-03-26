using CCEnvs.Collections;
using System.Linq;
using UnityEditor;

#nullable enable
namespace CCEnvs.Unity.Editr
{
    [InitializeOnLoad]
    public static class AssetNameMarkerTool
    {
        static AssetNameMarkerTool()
        {
        }

        [MenuItem(EditorHelper.ASSETS_TAB + "/" + EditorHelper.CCENVS_TAB + "/Add Asset Prefixes")]
        public static void AddPrefixes()
        {
            if (Selection.objects.IsNullOrEmpty())
                return;

            foreach (var asset in Selection.objects.ToArray())
                AssetDatabaseHelper.AddTypePrefix(asset);
        }

        [MenuItem(EditorHelper.ASSETS_TAB + "/" + EditorHelper.CCENVS_TAB + "/Remove Asset Prefixes")]
        public static void RemovePrefixes()
        {
            if (Selection.objects.IsNullOrEmpty())
                return;

            foreach (var asset in Selection.objects.ToArray())
                AssetDatabaseHelper.RemoveTypePrefix(asset);
        }
    }
}
