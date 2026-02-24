using CCEnvs.UnityEditor;
using CCEnvs.Utils;
using System.Linq;
using UnityEditor;
using UnityEditor.Build;

#nullable enable
namespace CCEnvs.Unity.EditorC
{
    public static class DebugModeToggle
    {
        [MenuItem(EditorHelper.COMPILING_TAB_NAME + "/" + EditorHelper.CC_TAB + "/" + EditorHelper.DEBUG_TAB + "/Enable")]
        public static void EnableDebugMode()
        {
            var targetGroups = EnumCache<BuildTargetGroup>.Values.Select(
                static targetGroup =>
                {
                    try
                    {
                        return NamedBuildTarget.FromBuildTargetGroup(targetGroup);
                    }
                    catch (System.Exception)
                    {
                        return NamedBuildTarget.Unknown;
                    }
                })
                .Where(targetGroup =>
                {
                    return targetGroup != NamedBuildTarget.Unknown;
                });

            foreach (var targetGroup in targetGroups)
            {
                try
                {
                    string defines = PlayerSettings.GetScriptingDefineSymbols(targetGroup);

                    if (defines.ContainsOrdinal("CC_DEBUG_ENABLED"))
                        continue;

                    defines += ";CC_DEBUG_ENABLED";

                    PlayerSettings.SetScriptingDefineSymbols(targetGroup, defines);
                }
                catch (System.Exception ex)
                {
                    typeof(DebugModeToggle).PrintExceptionAsLog(ex);
                }
            }

        }

        [MenuItem(EditorHelper.COMPILING_TAB_NAME + "/" + EditorHelper.CC_TAB + "/" + EditorHelper.DEBUG_TAB +  "/Disable")]
        public static void DisableDebugMode()
        {
            var targetGroups = EnumCache<BuildTargetGroup>.Values.Select(
                static targetGroup =>
                {
                    try
                    {
                        return NamedBuildTarget.FromBuildTargetGroup(targetGroup);
                    }
                    catch (System.Exception)
                    {
                        return NamedBuildTarget.Unknown;
                    }
                })
                .Where(targetGroup =>
                {
                    return targetGroup != NamedBuildTarget.Unknown;
                });

            foreach (var targetGroup in targetGroups)
            {
                try
                {
                    string defines = PlayerSettings.GetScriptingDefineSymbols(targetGroup);

                    if (!defines.ContainsOrdinal("CC_DEBUG_ENABLED"))
                        continue;

                    defines = string.Join(';', defines.Split(';').Except(Range.From("CC_DEBUG_ENABLED")));

                    PlayerSettings.SetScriptingDefineSymbols(targetGroup, defines);
                }
                catch (System.Exception ex)
                {
                    typeof(DebugModeToggle).PrintExceptionAsLog(ex);
                }
            }
        }
    }
}
