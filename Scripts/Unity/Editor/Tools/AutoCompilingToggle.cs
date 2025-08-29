using System;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

#nullable enable
namespace CozyColdEnvironments.Unity.Editor
{
    public static class AutoCompilingToggle
    {
        [MenuItem("Editor Scripts/Compiling/Force Compile And Disable &r")]
        public static void ForceCompileAndDisableCompiling()
        {
            EditorApplication.UnlockReloadAssemblies();
            AssetDatabase.Refresh();
            EditorUtility.RequestScriptReload();

            _ = AfterCompilation();
            Debug.Log($"{nameof(AutoCompilingToggle)}: Editor force compiling initiated.");
        }

        [MenuItem("Editor Scripts/Compiling/Enable &e")]
        public static void EnableAutoCompiling()
        {
            EditorApplication.UnlockReloadAssemblies();
            AssetDatabase.Refresh();
            EditorUtility.RequestScriptReload();

            Debug.Log($"{nameof(AutoCompilingToggle)}: Auto compiling enabled.");
        }

        [MenuItem("Editor Scripts/Compiling/Disable &d")]
        public static void DisableAutoCompiling()
        {
            EditorApplication.LockReloadAssemblies();
            Debug.Log($"{nameof(AutoCompilingToggle)}: Auto compiling disabled.");
        }

        private static async Task AfterCompilation()
        {
            while(EditorApplication.isCompiling || EditorApplication.isUpdating)
                await Task.Yield();

            DisableAutoCompiling();
            Debug.Log($"{nameof(AutoCompilingToggle)}: Editor force compiling ended.");
        }
    }
}
