using CCEnvs.UnityEditor;
using System.Linq;
using UnityEditor;
using UnityEditor.Build;

#nullable enable
namespace CCEnvs.Unity.EditorC
{
    public static class DebugModeToggle
    {
        [MenuItem(EditorHelper.BUILD_TAB_NAME + "/" + EditorHelper.MAIN_TAB_NAME + "/Enabled Debug Mode")]
        public static void EnableDebugMode()
        {
            var targetGroup = EditorUserBuildSettings.selectedBuildTargetGroup;

            string defines = PlayerSettings.GetScriptingDefineSymbols(
                NamedBuildTarget.FromBuildTargetGroup(targetGroup)
                );

            if (defines.ContainsOrdinal("CC_DEBUG_ENABLED"))
                return;

            defines += ";CC_DEBUG_ENABLED";

            PlayerSettings.SetScriptingDefineSymbols(
                NamedBuildTarget.FromBuildTargetGroup(targetGroup), 
                defines
                );
        }

        [MenuItem(EditorHelper.BUILD_TAB_NAME + "/" + EditorHelper.MAIN_TAB_NAME + "/Disabled Debug Mode")]
        public static void DisableDebugMode()
        {
            var targetGroup = EditorUserBuildSettings.selectedBuildTargetGroup;

            string defines = PlayerSettings.GetScriptingDefineSymbols(
                NamedBuildTarget.FromBuildTargetGroup(targetGroup)
                );

            if (!defines.ContainsOrdinal("CC_DEBUG_ENABLED"))
                return;

            defines = string.Join(';', defines.Split(';').Except(Range.From("CC_DEBUG_ENABLED")));

            PlayerSettings.SetScriptingDefineSymbols(
                NamedBuildTarget.FromBuildTargetGroup(targetGroup),
                defines
                );
        }
    }
}
