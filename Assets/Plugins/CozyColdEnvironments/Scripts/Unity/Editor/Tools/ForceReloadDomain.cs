using CCEnvs.Diagnostics;
using UnityEditor;

#nullable enable
namespace CCEnvs.Unity.Editor
{
    public static class ForceReloadDomain
    {
        [MenuItem("Editor/Compiling/Reload Domain &r")]
        public static void Main()
        {
            if (EditorApplication.isPlaying || EditorApplication.isCompiling)
                return;

            RequestReload();
            CCDebug.PrintLog("Reloading domain initiated.",
                             typeof(ForceReloadDomain));
        }

        private static void RequestReload()
        {
            AssetDatabase.Refresh();
            EditorUtility.RequestScriptReload();
        }
    }
}
