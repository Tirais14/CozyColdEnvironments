using CCEnvs.Diagnostics;
using UnityEditor;

#nullable enable
namespace CCEnvs.Unity.Editor
{
    public static class ForceReloadDomain
    {
        private static int requestReloadSubscriptionCount;

        [MenuItem("Editor/Compiling/Reload Domain &r")]
        public static void Main()
        {
            if (EditorApplication.isPlaying || EditorApplication.isCompiling)
                return;

            CCDebug.PrintLog("Reloading domain initiated.",
                             typeof(ForceReloadDomain));

            if (requestReloadSubscriptionCount < 1)
            {
                EditorApplication.delayCall += RequestReload;
                requestReloadSubscriptionCount++;
            }
        }

        private static void RequestReload()
        {
            AssetDatabase.Refresh();
            EditorUtility.RequestScriptReload();

            EditorApplication.delayCall -= RequestReload;
            requestReloadSubscriptionCount--;
        }
    }
}
