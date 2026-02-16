using CCEnvs.Diagnostics;
using CCEnvs.UnityEditor;
using UnityEditor;

#nullable enable
namespace CCEnvs.Unity.Editor
{
    public static class ForceReloadDomain
    {
        [MenuItem(EditorHelper.EDITOR_TAB_NAME + "/" + EditorHelper.MAIN_TAB_NAME + "/" + EditorHelper.COMPILING_TAB_NAME + "/ Relaod Domain &r")]
        public static void Main()
        {
            if (EditorApplication.isPlaying || EditorApplication.isCompiling)
                return;

            RequestReload();
            CCDebug.Instance.PrintLog("Reloading domain initiated.",
                             typeof(ForceReloadDomain));
        }

        private static void RequestReload()
        {
            AssetDatabase.Refresh();
            EditorUtility.RequestScriptReload();
        }
    }
}
