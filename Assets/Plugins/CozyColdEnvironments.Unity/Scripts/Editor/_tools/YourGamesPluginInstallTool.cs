#if UNITY_EDITOR
using System.Linq;
using CCEnvs.UnityEditor;
using UnityEditor;
using UnityEditor.Build;

#nullable enable
namespace CCEnvs.Unity.EditorC
{
    public static class YourGamesPluginInstallTool
    {
        [MenuItem(EditorHelper.TOOLS_TAB_NAME + "/" + EditorHelper.CCEnvs + "/YG2/Uninstall")]
        public static void Disable()
        {
            var defineSymbols = PlayerSettingsHelper.GetNamedBuildTargets()
                .SelectMany(bTarget => PlayerSettingsHelper.GetScriptingDefineSymbols(bTarget))
                .Where(symbol => symbol.EndsWith("_yg"))
                .Distinct()
                .Append("PLUGIN_YG_2")
                .Append("TMP_YG2")
                .Append("NJSON_YG2");

            PlayerSettingsHelper.RemoveScriptingDefineSymbols(
                null,
                defineSymbols.Append("YOUR_GAMES_PLUGIN_ENABLED").ToArray()
                );
        }

        [MenuItem(EditorHelper.TOOLS_TAB_NAME + "/" + EditorHelper.CCEnvs + "/YG2/Install")]
        public static void Enable()
        {
            PlayerSettingsHelper.AddScriptingDefineSymbols(
                Range.From(NamedBuildTarget.WebGL),
                "YOUR_GAMES_PLUGIN_ENABLED"
                );
        }
    }
}
#endif